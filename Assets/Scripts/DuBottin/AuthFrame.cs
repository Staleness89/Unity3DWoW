using Assets.Scripts.Shared;
using Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuthFrame : MonoBehaviour {

    Button CancelButton;
    Text authMessage;
    public static bool ShowCharacterUI = false;
    public static bool ShowRealms = false;

    // Use this for initialization
    void Start () {

        authMessage =  UnityEngine.GameObject.Find("AuthMessage").GetComponent<Text>();
        CancelButton = UnityEngine.GameObject.Find("AuthCancel").GetComponent<Button>();
        CancelButton.onClick.AddListener(Cancel);       
    }
	    
    void Cancel()
    {
        if (Exchange.authClient != null)
            Exchange.authClient.Exit();

        LoginHelpers.tryingToLogin = false;

        if (UnityEngine.GameObject.Find("RealmListFrame"))
        {
            Destroy(UnityEngine.GameObject.Find("RealmListFrame"));
        }

        if (UnityEngine.GameObject.Find("CharacterUI"))
        {
            Destroy(UnityEngine.GameObject.Find("CharacterUI"));
            UnityEngine.GameObject tempAuth = Instantiate(MainLogin.LoadPrefab("MainUI"), new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity);
            tempAuth.transform.SetParent(UnityEngine.GameObject.Find("Canvas").gameObject.transform);
            tempAuth.transform.localScale = new Vector3(1, 1, 1);
            tempAuth.name = "MainUI";
        }

        Destroy(UnityEngine.GameObject.Find("AuthFrame"));

    }
	// Update is called once per frame
	void Update () {
        authMessage.text = Exchange.AuthMessage;

        if (ShowRealms)
        {
            ShowRealms = false;
            UnityEngine.GameObject RealmList = Instantiate(MainLogin.LoadPrefab("RealmList"), new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity);
            RealmList.transform.SetParent(UnityEngine.GameObject.Find("Canvas").gameObject.transform);
            RealmList.transform.localScale = new Vector3(1, 1, 1);
            RealmList.name = "RealmList";
            Destroy(UnityEngine.GameObject.Find("AuthFrame"));
            Destroy(UnityEngine.GameObject.Find("MainUI"));
        }

        if(ShowCharacterUI)
        {            
            ShowCharacterUI = false;
            UnityEngine.GameObject CharacterUI = Instantiate(MainLogin.LoadPrefab("CharacterUI"), new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity);
            CharacterUI.transform.SetParent(UnityEngine.GameObject.Find("Canvas").gameObject.transform);
            CharacterUI.transform.localScale = new Vector3(1, 1, 1);
            CharacterUI.name = "CharacterUI";

            Destroy(UnityEngine.GameObject.Find("MainUI"));
            Destroy(UnityEngine.GameObject.Find("Image"));
        }
    }
}
