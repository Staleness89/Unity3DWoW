using Assets.Scripts.Shared;
using Client.Authentication;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RealmListFrame : MonoBehaviour {

    static Text realmName;
    static Text realmTypetext;
    static Text realmCharacterCount;
    static Text realmPop;
    Button Cancel;
    Button Accept;

    // Use this for initialization
    void Start () {
        showRealmList(Exchange.gameClient.RealmServerList);
        Cancel = UnityEngine.GameObject.Find("CancelRealmList").GetComponent<Button>();
        Cancel.onClick.AddListener(cancelRealm);

        Accept = UnityEngine.GameObject.Find("OkayRealmList").GetComponent<Button>();
        Accept.onClick.AddListener(acceptRealm);


    }

    // Update is called once per frame
    void Update () {
		
	}

    public static void showRealmList(WorldServerInfo[] realm)
    {
        int ID = 0;
        if (realm != null)
        {
            foreach (WorldServerInfo rl in realm)
            {
                if (rl != null)
                {
                    UnityEngine.GameObject newRealm = Instantiate(Resources.Load("Realm") as UnityEngine.GameObject);
                    newRealm.transform.parent = UnityEngine.GameObject.Find("Content").gameObject.transform;
                    newRealm.transform.localScale = new Vector3(1, 1, 1);
                    newRealm.name = rl.Name;

                    Transform[] ts = newRealm.transform.GetComponentsInChildren<Transform>(true);
                    foreach (Transform t in ts)
                    {
                        if (t.gameObject.name == "RealmRealmName")
                        {
                            t.gameObject.name = rl.Name + "RealmRealmName";
                        }

                        if (t.gameObject.name == "RealmType")
                        {
                            t.gameObject.name = rl.Name + "RealmType";
                        }

                        if (t.gameObject.name == "RealmCharacters")
                        {
                            t.gameObject.name = rl.Name + "RealmCharacters";
                        }

                        if (t.gameObject.name == "RealmPopulation")
                        {
                            t.gameObject.name = rl.Name + "RealmPopulation";
                        }

                        if (t.gameObject.name == "RealmSelect")
                        {
                            t.gameObject.name = rl.Name + "RealmSelect" + rl.ID;
                        }
                    }

                    realmName = UnityEngine.GameObject.Find(rl.Name + "RealmRealmName").GetComponent<Text>();
                    realmName.text = rl.Name;
                    if (rl.wOnline == 0)
                    { realmName.color = Color.gray; }
                    else { realmName.color = Color.green; }

                    string realmType = "";
                    realmTypetext = UnityEngine.GameObject.Find(rl.Name + "RealmType").GetComponent<Text>();

                    switch (rl.Type)
                    {
                        case 1:
                            realmType = "PvP";
                            break;
                        case 4:
                            realmType = "Normal";
                            break;
                        case 6:
                            realmType = "RP";
                            break;
                        case 8:
                            realmType = "RPPvP";
                            break;
                        case 16:
                            realmType = "FFa_PvP";
                            break;
                        default:
                            realmType = "Normal";
                            break;
                    }

                    realmTypetext.text = realmType;
                    if (rl.wOnline == 0)
                    { realmTypetext.color = Color.gray; }
                    else { realmTypetext.color = Color.green; }

                    realmCharacterCount = UnityEngine.GameObject.Find(rl.Name + "RealmCharacters").GetComponent<Text>();
                    realmCharacterCount.text = "(" + rl.load.ToString() + ")";
                    if (rl.wOnline == 0)
                    { realmCharacterCount.color = Color.gray; }
                    else { realmCharacterCount.color = Color.green; }

                    realmPop = UnityEngine.GameObject.Find(rl.Name + "RealmPopulation").GetComponent<Text>();

                    string popLevel = "";

                    switch (Convert.ToInt32(rl.Population))
                    {
                        case 0:
                            popLevel = "Low";
                            break;
                        case 1:
                            popLevel = "Medium";
                            break;
                        case 2:
                            popLevel = "High";
                            break;
                    }


                    realmPop.text = popLevel;
                    if (rl.wOnline == 0)
                    {
                        realmPop.color = Color.gray;
                        realmPop.text = "Offline";
                    }
                    else { realmPop.color = Color.green; }

                    if (ID == 0 && rl.wOnline == 1)
                    {
                        var sprite = Resources.Load<Sprite>("list_Focus");
                        Image targeted = UnityEngine.GameObject.Find(rl.Name + "RealmSelect" + rl.ID).GetComponent<Image>();
                        targeted.sprite = sprite;

                        Exchange.CurrentRealm = rl;
                    }

                    ID++;
                }
            }
        }
    }
    
    public void acceptRealm()
    {
        if (System.IO.File.Exists(Application.dataPath + "/RealmList.txt"))
        {
            File.Delete(Application.dataPath + "/RealmList.txt");
            File.Create(Application.dataPath + "/RealmList.txt").Close();

            using (StreamWriter w = File.AppendText(Application.dataPath + "/RealmList.txt"))
            {
                w.WriteLine("REALM_LIST_ADDRESS " + LoginHelpers.REALM_LIST_ADDRESS);
                w.WriteLine("LAST_KNOWN_REALM_LIST " + Exchange.CurrentRealm.Name);
                w.Close();
            }
        }
        UnityEngine.GameObject MainLoginUI = Instantiate(Resources.Load<UnityEngine.GameObject>("AuthFrame"), new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity);
        MainLoginUI.transform.SetParent(UnityEngine.GameObject.Find("Canvas").gameObject.transform);
        MainLoginUI.transform.localScale = new Vector3(1, 1, 1);
        MainLoginUI.name = "AuthFrame";

        Exchange.gameClient.ConnectTo(Exchange.CurrentRealm);
        Destroy(UnityEngine.GameObject.Find("RealmList"));
    }

    public void cancelRealm()
    {
        Exchange.gameClient.Exit();
        UnityEngine.GameObject MainLoginUI = Instantiate(Resources.Load<UnityEngine.GameObject>("MainUI"), new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity);
        MainLoginUI.transform.SetParent(UnityEngine.GameObject.Find("Canvas").gameObject.transform);
        MainLoginUI.transform.localScale = new Vector3(1, 1, 1);
        MainLoginUI.name = "MainUI";

        LoginHelpers.tryingToLogin = false;
        Destroy(UnityEngine.GameObject.Find("RealmList"));
    }

}
