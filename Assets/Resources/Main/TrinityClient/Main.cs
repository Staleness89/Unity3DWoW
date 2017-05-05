using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Main : MonoBehaviour {

    public GameObject login;
    public static string REALM_LIST_ADDRESS = "127.0.0.1";
    public static string LAST_KNOWN_REALM_LIST = " ";

    // Use this for initialization
    void Start()
    {
        if (!System.IO.File.Exists(Application.dataPath + "/RealmList.txt"))
        {
            File.Create(Application.dataPath + "/RealmList.txt").Close();

            using (StreamWriter w = File.AppendText(Application.dataPath + "/RealmList.txt"))
            {
                w.WriteLine("REALM_LIST_ADDRESS " + REALM_LIST_ADDRESS);
                w.WriteLine("LAST_KNOWN_REALM_LIST " + LAST_KNOWN_REALM_LIST);
            }
        }

        string[] Config = System.IO.File.ReadAllLines(Application.dataPath + "/RealmList.txt");

        foreach (string line in Config)
        {
            if (line.Contains("REALM_LIST_ADDRESS "))
            {
                REALM_LIST_ADDRESS = line.Substring(19);
            }

            if (line.Contains("LAST_KNOWN_REALM_LIST "))
            {
                LAST_KNOWN_REALM_LIST = line.Substring(22);
            }
        }


        GameObject mainLogin = Instantiate(login, new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity);
        mainLogin.transform.parent = transform;
        mainLogin.name = "Login";
    }

    // Update is called once per frame
    void Update()
    {
        if(Exchange.authClient != null)
        {
            Exchange.authClient.OnConnect();
        }
    }
}
