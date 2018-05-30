using Assets.Scripts.Shared;
using Client.World;
using Client.World.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour {

    Button Realms;
    Button Back;
    Button Create;
    Button Delete;
    Button EnterWorld;

    public static bool EnumResult = false;
    // Use this for initialization
    void Start () {

        Realms = UnityEngine.GameObject.Find("RealmsButton").GetComponent<Button>();
        Realms.onClick.AddListener(ListRealms);
        Back = UnityEngine.GameObject.Find("BackButton").GetComponent<Button>();
        Back.onClick.AddListener(BackFun);
        Create = UnityEngine.GameObject.Find("CreateCharacter").GetComponent<Button>();
        Create.onClick.AddListener(CreateCharacter);
        Delete = UnityEngine.GameObject.Find("DeleteCharacter").GetComponent<Button>();
        Delete.onClick.AddListener(DeleteCharacter);
        EnterWorld = UnityEngine.GameObject.Find("EnterWorld").GetComponent<Button>();
        EnterWorld.onClick.AddListener(EnterWorldFun);
                

    }

    public void ListRealms()
    {
        UnityEngine.GameObject RealmList = Instantiate(MainLogin.LoadPrefab("RealmList"), new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity);
        RealmList.transform.SetParent(UnityEngine.GameObject.Find("Canvas").gameObject.transform);
        RealmList.transform.localScale = new Vector3(1, 1, 1);
        RealmList.name = "RealmList";
        Destroy(UnityEngine.GameObject.Find("CharacterUI"));
    }

    public void BackFun()
    {
        if (Exchange.gameClient != null)
            Exchange.gameClient.Exit();

        LoginHelpers.tryingToLogin = false;
        
        if (UnityEngine.GameObject.Find("CharacterUI"))
        {            
            UnityEngine.GameObject tempAuth = Instantiate(MainLogin.LoadPrefab("MainUI"), new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity);
            tempAuth.transform.SetParent(UnityEngine.GameObject.Find("Canvas").gameObject.transform);
            tempAuth.transform.localScale = new Vector3(1, 1, 1);
            tempAuth.name = "MainUI";
            Destroy(UnityEngine.GameObject.Find("CharacterUI"));
        }
    }

    public void CreateCharacter()
    {

    }

    public void DeleteCharacter()
    {

    }

    public void EnterWorldFun()
    {
        OutPacket packet = new OutPacket(WorldCommand.CMSG_PLAYER_LOGIN);
        packet.Write(Exchange.SelectedCharacter.GUID);
        Exchange.gameClient.SendPacket(packet);
        Exchange.playerIsInGame = true;
        UnityEngine.GameObject.Find("GameObject").name = Exchange.SelectedCharacter.GUID.ToString();
        UnityEngine.GameObject.Find(Exchange.SelectedCharacter.GUID.ToString()).GetComponent<Transform>().position = new Vector3(Exchange.SelectedCharacter.X, Exchange.SelectedCharacter.Z, Exchange.SelectedCharacter.Y);
        Destroy(UnityEngine.GameObject.Find("CharacterUI"));
        Destroy(UnityEngine.GameObject.Find("MainLogo"));

        var PlayerUIframe = Resources.Load("PlayerUI") as UnityEngine.GameObject;
        UnityEngine.GameObject PlayerUI = Instantiate(PlayerUIframe, new Vector3(PlayerUIframe.transform.position.x, PlayerUIframe.transform.position.y, PlayerUIframe.transform.position.z), Quaternion.identity);
        PlayerUI.transform.SetParent(UnityEngine.GameObject.Find("Canvas").gameObject.transform, false);
        PlayerUI.transform.localScale = new Vector3(1, 1, 1);
        PlayerUI.name = "PlayerUI";

    }

    // Update is called once per frame
    void Update () {
		
        if(EnumResult)
        {
            Destroy(UnityEngine.GameObject.Find("AuthFrame"));
            EnumResult = false;
            foreach (Character character in Exchange.characters)
            {
                UnityEngine.GameObject Character = Instantiate(Resources.Load("Character") as UnityEngine.GameObject, new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity);
                Character.transform.SetParent(UnityEngine.GameObject.Find("Content").gameObject.transform);
                Character.transform.localScale = new Vector3(1, 1, 1);
                Character.name = character.Name;

                Transform[] ts = Character.transform.GetComponentsInChildren<Transform>(true);
                foreach (Transform t in ts)
                {
                    if (t.gameObject.name == "Name")
                    {
                        t.gameObject.name = character.Name + "Name";
                        t.gameObject.GetComponent<Text>().text = character.Name;
                    }

                    if (t.gameObject.name == "Level")
                    {
                        t.gameObject.name = character.Name + "Level";
                        t.gameObject.GetComponent<Text>().text = character.Level.ToString();
                    }

                    if (t.gameObject.name == "Class")
                    {
                        t.gameObject.name = character.Name + "Class";
                        t.gameObject.GetComponent<Text>().text = character.Class.ToString();
                    }

                    if (t.gameObject.name == "Zone")
                    {
                        t.gameObject.name = character.Name + "Zone";
                        t.gameObject.GetComponent<Text>().text = character.ZoneId.ToString();
                    }
                }
            }

            ///
            /// Should Spawn Character Model Here.
            ///
        }
	}
}
