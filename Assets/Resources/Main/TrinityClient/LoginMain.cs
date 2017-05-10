using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginMain : MonoBehaviour {

    // Use this for initialization
    string Account = "";
    string Password = "";
    public static bool tryingToLogin = false;
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
        tryingToLogin = true;

        if (Account.Length < 3 || Password.Length < 3)
        {
            Global.showNotifyBox("Account Name Length Too Short", "Okay");
        }
        else
        {

            Global.showNotifyBox("Connecting...", "Cancel");

            AuthSocket newLogin = new AuthSocket(Account, Password, Main.REALM_LIST_ADDRESS);
            newLogin.Login();
            Exchange.authClient = newLogin;
        }
    }

    void OnGUI()
    {
        if (Event.current.Equals(Event.KeyboardEvent("return")))
        {
            loginClick();
        }

        if (!tryingToLogin)
        {
            if (Account.Length < 1 && Password.Length < 1)
            {
                GUI.FocusControl("AccountBox");
            }

            GUI.SetNextControlName("AccountBox");
            Account = GUI.TextField(ResizeGUI(new Rect(325, 315, 150, 20)), Account, 20);

            GUI.SetNextControlName("PasswordBox");
            Password = GUI.PasswordField(ResizeGUI(new Rect(325, 360, 150, 20)), Password, '*');
        }
    }

    Rect ResizeGUI(Rect _rect)
    {
        float FilScreenWidth = _rect.width / 800;
        float rectWidth = FilScreenWidth * Screen.width;
        float FilScreenHeight = _rect.height / 600;
        float rectHeight = FilScreenHeight * Screen.height;
        float rectX = (_rect.x / 800) * Screen.width;
        float rectY = (_rect.y / 600) * Screen.height;

        return new Rect(rectX, rectY, rectWidth, rectHeight);
    }

    void quitClick()
    {

    }
}
