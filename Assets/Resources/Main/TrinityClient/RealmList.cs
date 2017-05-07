using Assets.Scripts.World;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RealmList : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void acceptRealm()
    {
        if (System.IO.File.Exists(Application.dataPath + "/RealmList.txt"))
        {
            File.Delete(Application.dataPath + "/RealmList.txt");
            File.Create(Application.dataPath + "/RealmList.txt").Close();

            using (StreamWriter w = File.AppendText(Application.dataPath + "/RealmList.txt"))
            {
                w.WriteLine("REALM_LIST_ADDRESS " + Main.REALM_LIST_ADDRESS);
                w.WriteLine("LAST_KNOWN_REALM_LIST " + Exchange.currRealm.Name);
                w.Close();
            }
        }
        Exchange.worldClient = new World(Exchange.authClient.mUsername, Exchange.currRealm, Exchange.authClient.mKey);
        Exchange.worldClient.Connect();
    }

    public void cancelRealm()
    {

    }
}
