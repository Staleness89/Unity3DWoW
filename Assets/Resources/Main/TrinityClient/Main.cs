using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Main : MonoBehaviour {

    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    public Texture2D[] Pointers = new Texture2D[5];
    public GameObject login;
    public GameObject notifyBox;
    public GameObject realmBox;
    public GameObject Realm;
    public GameObject characterList;
    public GameObject characterCreate;
    public GameObject characters;
    public GameObject deleteNotify;
    public GameObject maleModel;
    public GameObject femaleModel;
    //public GameObject Loading;

    public Sprite realmListHighlight;
    public Sprite realmListClear;
    public Sprite selected;

    public static string REALM_LIST_ADDRESS = "127.0.0.1";
    public static string LAST_KNOWN_REALM_LIST = " ";

    // Use this for initialization
    void Start()
    {
        Global.selected = selected;
        Global.femaleModel = femaleModel;
        Global.maleModel = maleModel;
        Global.deleteNotify = deleteNotify;
        Global.characters = characters;
        Global.login = login;
        Global.notifyBox = notifyBox;
        Global.realmBox = realmBox;
        Global.Realm = Realm;
        Global.realmListHighlight = realmListHighlight;
        Global.realmListClear = realmListClear;
        Global.characterList = characterList;
        Global.characterCreate = characterCreate;
        //Global.Loading = Loading;

        setPointer();

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
        mainLogin.transform.localScale = new Vector3(1, 1, 1);
        mainLogin.name = "Login";
    }

    // Update is called once per frame
    void Update()
    {
        if(Exchange.authClient != null)
        {
            Exchange.authClient.OnConnect();
        }

        if (Exchange.worldClient != null)
        {
            Exchange.worldClient.Loop();
        }
    }



    void setPointer()
    {
        //Switch Case for type on objects.
        Cursor.SetCursor(Pointers[0], hotSpot, cursorMode);
    }

}
