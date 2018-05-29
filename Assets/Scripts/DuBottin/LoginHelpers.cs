using Assets.Scripts.Shared;
using Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginHelpers : MonoBehaviour {

    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    public static bool tryingToLogin = false;
    public static string REALM_LIST_ADDRESS = "186.2.164.192";
    public static string LAST_KNOWN_REALM_LIST = " ";
    public static bool ConnectToWorld = false;
    public static List<WorldObject> objects;
    public Texture2D[] Pointers = new Texture2D[5];
    public AudioClip[] Sounds = new AudioClip[5];
    // Use this for initialization
    void Start () {
        objects = new List<WorldObject>();
        setPointer();

        Exchange.Sounds = Sounds;

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

    }


    void setPointer()
    {
        //Switch Case for type on objects.
        Cursor.SetCursor(Pointers[0], hotSpot, cursorMode);
    }
            
    // Update is called once per frame
    void Update () {

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (UnityEngine.GameObject.Find("targetFrame"))
            {
                Exchange.authClient.ThreadHelper.removeTarget(Exchange.authClient.Player.Target);
                return;
            }
        }
    }
}
