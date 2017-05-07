using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginMain : MonoBehaviour {

    // Use this for initialization
    
    Button LoginButton;
    Button QuitButton;
    InputField AccountName;
    InputField AccountPassword;
    Text LastKnownRealm;

    // Use this for initialization
    void Start()
    {
        LastKnownRealm = GameObject.Find("LastKnownRealm").GetComponent<Text>();
        LastKnownRealm.text = Main.LAST_KNOWN_REALM_LIST;

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
        AccountName = GameObject.Find("AccountName").GetComponent<InputField>();
        AccountPassword = GameObject.Find("AccountPassword").GetComponent<InputField>();

        if (AccountName.text.Length < 3 || AccountPassword.text.Length < 3)
        {
            Global.showNotifyBox("Account Name Length Too Short", "Okay");
        }
        else
        {

            Global.showNotifyBox("Connecting...", "Cancel");

            AuthSocket newLogin = new AuthSocket(AccountName.text, AccountPassword.text, Main.REALM_LIST_ADDRESS);
            newLogin.Login();
            Exchange.authClient = newLogin;
        }
    }

    void quitClick()
    {

    }
}
