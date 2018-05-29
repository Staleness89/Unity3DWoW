
using Assets.Scripts.Shared;
using Client;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainLogin : MonoBehaviour {
        
    public string Account = "";
    public string Password = "";
    Text lastRealm;

    Button Login;
    Button Cancel;
    Button Quit;


    // Use this for initialization
    void Start()
    {
        Login = UnityEngine.GameObject.Find("Login").GetComponent<Button>();
        Login.onClick.AddListener(loginClick);
        Quit = UnityEngine.GameObject.Find("Quit").GetComponent<Button>();
        Quit.onClick.AddListener(quitClick);

        lastRealm = UnityEngine.GameObject.Find("LastRealm").GetComponent<Text>();
        lastRealm.text = LoginHelpers.LAST_KNOWN_REALM_LIST;

        Exchange.authClient = null;
    }

    public static UnityEngine.GameObject LoadPrefab(string i)
    {
        return Resources.Load(i) as UnityEngine.GameObject;
    }

    public UnityEngine.GameObject Find(string obj)
    {
        return UnityEngine.GameObject.Find(obj);
    }

    public void loginClick()
    {
        if (Account.Length < 3 || Password.Length < 3)
        {
            UnityEngine.GameObject tempAuth = Instantiate(LoadPrefab("AuthFrame"), new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity);
            tempAuth.transform.SetParent(UnityEngine.GameObject.Find("Canvas").gameObject.transform);
            tempAuth.transform.localScale = new Vector3(1, 1, 1);
            tempAuth.name = "AuthFrame";
            Exchange.AuthMessage = "Username or password too short.";
            Password = "";
        }
        else
        {
            LoginHelpers.tryingToLogin = true;
            UnityEngine.GameObject tempAuth =  Instantiate(LoadPrefab("AuthFrame"), new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity);
            tempAuth.transform.SetParent(UnityEngine.GameObject.Find("Canvas").gameObject.transform);
            tempAuth.transform.localScale = new Vector3(1, 1, 1);
            tempAuth.name = "AuthFrame";
            Exchange.Username = Account;
            Exchange.Password = Password;
            
            Exchange.authClient = new AutomatedGame("127.0.0.1", 3724, Exchange.Username, Exchange.Password);
            Exchange.authClient.Start();
        }
    }
 
    public void quitClick()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    
    void OnGUI()
    {
        if(Event.current.Equals(Event.KeyboardEvent("return")))
        {
            loginClick();
        }

        if (!LoginHelpers.tryingToLogin)
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
}
