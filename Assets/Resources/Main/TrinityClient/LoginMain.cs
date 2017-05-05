using Assets.Resources.Main.TrinityClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginMain : MonoBehaviour {

    // Use this for initialization
    public GameObject notifyBox;
    Button LoginButton;
    Button QuitButton;
    Button NotifyButton;
    Text AccountName;
    Text AccountPassword;

    // Use this for initialization
    void Start()
    {
        Global.notify = notifyBox;
        LoginButton = GameObject.Find("loginButton").GetComponent<Button>();
        LoginButton.onClick.AddListener(loginClick);

        QuitButton = GameObject.Find("QuitButton").GetComponent<Button>();
        QuitButton.onClick.AddListener(quitClick);
    }

    // Update is called once per frame
    void Update()
    {


    }

    void loginClick()
    {
        AccountName = GameObject.Find("AccountNameText").GetComponent<Text>();
        AccountPassword = GameObject.Find("AccountPassowrdText").GetComponent<Text>();

        if (AccountName.text.Length < 3)
        {
            Global.showNotifyBox("Account Name Length Too Short", "Okay");
        }
    }

    void quitClick()
    {

    }
}
