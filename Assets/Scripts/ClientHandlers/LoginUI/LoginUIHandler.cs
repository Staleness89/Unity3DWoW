using Foole.Mpq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoginUIHandler : MonoBehaviour
{
    public static LoginUIHandler LoginUIInstance;
    public List<TMP_Text> LoginGlueTexts;
    public List<Button> LoginGlueButtons;
    public List<InputField> LoginGlueInputFields;
    public List<Image> LoginUIImages;
    public List<Image> RealmUIImages;
    public List<GameObject> LoginUIPanels;
    public AudioSource Audio;
    public Camera camera;
    private bool wasRealmUIBuilt = false;
    private bool wasCharacterUIBuilt = false;

    // Start is called before the first frame update
    void Start()
    {
        if (LoginUIInstance != null)
            return;

        LoginUIInstance = this;

        Audio.clip = WavUtility.ToAudioClip(AppHandler.Instance.SearchMPQ(@"Sound\Music\GlueScreenMusic\WotLK_main_title.mp3"));
        Audio.Play();

        LoginGlueTexts.First(m => m.name == "RealmName").text = AppHandler.Instance.LAST_KNOWN_REALMNAME;

        BuildBackground();
        BuildLoginUI();
        BuildDialogUI();
    }
    void BuildBackground()
    {
        var model = new M2();
        MpqStream f = AppHandler.Instance.SearchMPQ(@"Interface\GLUES\MODELS\UI_MainMenu_Northrend\UI_MainMenu_Northrend.M2");
        model.Load(f);
        camera.gameObject.transform.position = new Vector3(-model.Cameras[0].PositionBase.X, model.Cameras[0].PositionBase.Z, -model.Cameras[0].PositionBase.Y);
        camera.farClipPlane = model.Cameras[0].FarClip;
        camera.nearClipPlane = model.Cameras[0].NearClip;
        float vfov = model.Cameras[0].FieldOfView.Values[0][0].X / Mathf.Sqrt(1.0f + Mathf.Pow(camera.aspect, 2.0f));
        camera.fieldOfView = model.Cameras[0].FieldOfView.Values[0][0].X;
        M2UnityHelper h = new M2UnityHelper(model);
        h.GenerateGameObject();
    }
    void BuildLoginUI()
    {
        Sprite Logo = BLPLoader.ToSprite(AppHandler.Instance.SearchMPQ(@"Interface\GLUES\COMMON\Glues-WoW-WotLKLogo.blp"), new Vector2(0, 0), new Vector2(0, 0));
        Sprite CompanyLogo = BLPLoader.ToSprite(AppHandler.Instance.SearchMPQ(@"Interface\GLUES\MainMenu\Glues-BlizzardLogo.blp"), new Vector2(0, 0), new Vector2(0, 0));
        Sprite Rating = BLPLoader.ToSprite(AppHandler.Instance.SearchMPQ(@"Interface\GLUES\LOGIN\Glues-ESRBRating.blp"), new Vector2(0,0), new Vector2(0,0));

        LoginUIImages.First(m => m.name == "MainLogo").sprite = Logo;
        LoginUIImages.First(m => m.name == "CompanyLogo").sprite = CompanyLogo;
        LoginUIImages.First(m => m.name == "Rating").sprite = Rating;

        Sprite DisableButton = BLPLoader.ToSprite(AppHandler.Instance.SearchMPQ(@"Interface\GLUES\COMMON\Glue-Panel-Button-Disabled.blp"), new Vector2(0, 20), new Vector2(144, 44));
        Sprite HighlightButton = BLPLoader.ToSprite(AppHandler.Instance.SearchMPQ(@"Interface\GLUES\COMMON\Glue-Panel-Button-Highlight-Blue.blp"), new Vector2(0, 20), new Vector2(144, 44));
        Sprite NormalButton = BLPLoader.ToSprite(AppHandler.Instance.SearchMPQ(@"Interface\GLUES\COMMON\Glue-Panel-Button-Up-Blue.blp"), new Vector2(0, 20), new Vector2(144, 44));
        Sprite PressedButton = BLPLoader.ToSprite(AppHandler.Instance.SearchMPQ(@"Interface\GLUES\COMMON\Glue-Panel-Button-Down-Blue.blp"), new Vector2(0, 20), new Vector2(144, 44));
        
        for (int i = 0; i < LoginGlueButtons.Count; i++)
        {
            LoginGlueButtons[i].image.sprite = NormalButton;

            SpriteState s = new SpriteState
            {
                disabledSprite = DisableButton,
                highlightedSprite = NormalButton, //HighlightButton, Some reason this bugs out? or maybe its a seprate image?
                pressedSprite = PressedButton
            };
            LoginGlueButtons[i].spriteState = s;

            switch(LoginGlueButtons[i].name)
            {
                case "Options":
                    LoginGlueButtons[i].onClick.AddListener(Options);
                    break;
                case "Cinematics":
                    LoginGlueButtons[i].onClick.AddListener(Cinematics);
                    break;
                case "Credits":
                    LoginGlueButtons[i].onClick.AddListener(Credits);
                    break;
                case "Terms of use":
                    LoginGlueButtons[i].onClick.AddListener(TOS);
                    break;
                case "Quit":
                    LoginGlueButtons[i].onClick.AddListener(Quit);
                    break;
                case "AccountManage":
                    LoginGlueButtons[i].onClick.AddListener(AccountManage);
                    break;
                case "Website":
                    LoginGlueButtons[i].onClick.AddListener(Website);
                    break;
                case "Login":
                    LoginGlueButtons[i].onClick.AddListener(Login);
                    break;
                case "RealmOK":
                    LoginGlueButtons[i].onClick.AddListener(Empty);
                    break;
                case "RealmCancel":
                    LoginGlueButtons[i].onClick.AddListener(Empty);
                    break;
            }
        }

        Texture2D TextinputFrame = BLPLoader.ToTex(AppHandler.Instance.SearchMPQ(@"Interface\GLUES\COMMON\TextPanel-Border.blp"));

        for (int i = 0; i < LoginGlueInputFields.Count; i++)
        {
            Transform[] _Children = LoginGlueInputFields[i].transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform _child in _Children)
            {
                if (_child.gameObject.name == "Center")
                {
                    _child.gameObject.GetComponent<Image>().sprite = Sprite.Create(TextinputFrame, new Rect(2, 0, 61, 32), new Vector2(0, 0));
                }
                if (_child.gameObject.name == "Right")
                {
                    _child.gameObject.GetComponent<Image>().sprite = Sprite.Create(TextinputFrame, new Rect(192, 0, 64, 32), new Vector2(0, 0));
                }
                if (_child.gameObject.name == "Left")
                {
                    _child.gameObject.GetComponent<Image>().sprite = Sprite.Create(TextinputFrame, new Rect(128, 0, 64, 32), new Vector2(0, 0));
                }
            }
        }
    }
    void BuildDialogUI()
    {
        Texture2D borderImage = BLPLoader.ToTex(AppHandler.Instance.SearchMPQ(@"Interface\DialogFrame\UI-DialogBox-Border.blp"));

        Sprite Top = Sprite.Create(borderImage, new Rect(48, 0, 15, 32), new Vector2(0, 0));
        Sprite Bottom = Sprite.Create(borderImage, new Rect(0, 0, 17, 32), new Vector2(0, 0));
        Sprite Left = Sprite.Create(borderImage, new Rect(65, 0, 17, 32), new Vector2(0, 0));
        Sprite Right = Sprite.Create(borderImage, new Rect(112, 0, 16, 32), new Vector2(0, 0));
        Sprite TLC = Sprite.Create(borderImage, new Rect(159, 0, 32, 31), new Vector2(0, 0));
        Sprite BLC = Sprite.Create(borderImage, new Rect(129, 0, 30, 31), new Vector2(0, 0));
        Sprite TRC = Sprite.Create(borderImage, new Rect(230, 1, 26, 31), new Vector2(0, 0));
        Sprite BRC = Sprite.Create(borderImage, new Rect(192, 0, 30, 32), new Vector2(0, 0));

        LoginUIImages.First(m => m.name == "Top").sprite = Top;
        LoginUIImages.First(m => m.name == "Bottom").sprite = Bottom;
        LoginUIImages.First(m => m.name == "Left").sprite = Left;
        LoginUIImages.First(m => m.name == "Right").sprite = Right;
        LoginUIImages.First(m => m.name == "TLC").sprite = TLC;
        LoginUIImages.First(m => m.name == "BLC").sprite = BLC;
        LoginUIImages.First(m => m.name == "TRC").sprite = TRC;
        LoginUIImages.First(m => m.name == "BRC").sprite = BRC;

        LoginGlueTexts.First(m => m.name == "InfoText").text = "";
    }
    void BuildRealmUI()
    {
        Sprite RealmTopLeft = BLPLoader.ToSprite(AppHandler.Instance.SearchMPQ(@"Interface\HelpFrame\HelpFrame-TopLeft.blp"), new Vector2(0, 0), new Vector2(0, 0));
        Sprite RealmTopRight = BLPLoader.ToSprite(AppHandler.Instance.SearchMPQ(@"Interface\HelpFrame\HelpFrame-TopRight.blp"), new Vector2(0, 0), new Vector2(0, 0));
        Sprite RealmTop = BLPLoader.ToSprite(AppHandler.Instance.SearchMPQ(@"Interface\HelpFrame\HelpFrame-Top.blp"), new Vector2(0, 0), new Vector2(0, 0));
        Sprite RealmBotLeft = BLPLoader.ToSprite(AppHandler.Instance.SearchMPQ(@"Interface\HelpFrame\HelpFrame-BotLeftBig.blp"), new Vector2(0, 0), new Vector2(0, 0));
        Sprite RealmBotRight = BLPLoader.ToSprite(AppHandler.Instance.SearchMPQ(@"Interface\HelpFrame\HelpFrame-BotRightBig.blp"), new Vector2(0, 0), new Vector2(0, 0));
        Sprite RealmBottom = BLPLoader.ToSprite(AppHandler.Instance.SearchMPQ(@"Interface\HelpFrame\HelpFrame-BottomBig.blp"), new Vector2(0, 0), new Vector2(0, 0));        
        Sprite Header = BLPLoader.ToSprite(AppHandler.Instance.SearchMPQ(@"Interface\DialogFrame\UI-DialogBox-Header.blp"), new Vector2(0, 0), new Vector2(0, 0));
        //Sprite CloseButton = BLPLoader.ToSprite(AppHandler.Instance.SearchMPQ(@"Interface\GLUES\COMMON\Glues-WoW-WotLKLogo.blp"), new Vector2(0, 0), new Vector2(0, 0));

        RealmUIImages.First(m => m.name == "RealmTopLeft").sprite = RealmTopLeft;
        RealmUIImages.First(m => m.name == "RealmTopRight").sprite = RealmTopRight;
        RealmUIImages.First(m => m.name == "RealmTop").sprite = RealmTop;
        RealmUIImages.First(m => m.name == "RealmBotLeft").sprite = RealmBotLeft;
        RealmUIImages.First(m => m.name == "RealmBotRight").sprite = RealmBotRight;
        RealmUIImages.First(m => m.name == "RealmBottom").sprite = RealmBottom;
        //RealmUIImages.First(m => m.name == "RealmTab0").sprite = Sprite.Create(RealmTab0, new Rect(0, 0, 0, 0), new Vector2(0, 0));
        //RealmUIImages.First(m => m.name == "RealmTab1").sprite = Sprite.Create(RealmTab0, new Rect(0, 0, 0, 0), new Vector2(0, 0));
        //RealmUIImages.First(m => m.name == "RealmTab2").sprite = Sprite.Create(RealmTab0, new Rect(0, 0, 0, 0), new Vector2(0, 0)); 
        RealmUIImages.First(m => m.name == "Header").sprite = Header;

        Texture2D RealmTab0 = BLPLoader.ToTex(AppHandler.Instance.SearchMPQ(@"Interface\HelpFrame\HelpFrameTab-Active.blp"));

        Sprite Left = Sprite.Create(RealmTab0, new Rect(0, 0, 17, 32), new Vector2(0, 0));
        Sprite Center = Sprite.Create(RealmTab0, new Rect(20, 0, 28, 32), new Vector2(0, 0));
        Sprite Right = Sprite.Create(RealmTab0, new Rect(50, 0, 14, 32), new Vector2(0, 0));


        RealmUIImages.First(m => m.name == "RealmNameBoxLeft").sprite = Left;
        RealmUIImages.First(m => m.name == "RealmNameBoxCenter").sprite = Center;
        RealmUIImages.First(m => m.name == "RealmNameBoxRight").sprite = Right;

        RealmUIImages.First(m => m.name == "TypeBoxLeft").sprite = Left;
        RealmUIImages.First(m => m.name == "TypeBoxCenter").sprite = Center;
        RealmUIImages.First(m => m.name == "TypeBoxRight").sprite = Right;

        RealmUIImages.First(m => m.name == "CharactersBoxLeft").sprite = Left;
        RealmUIImages.First(m => m.name == "CharactersBoxCenter").sprite = Center;
        RealmUIImages.First(m => m.name == "CharactersBoxRight").sprite = Right;

        RealmUIImages.First(m => m.name == "PopulationBoxLeft").sprite = Left;
        RealmUIImages.First(m => m.name == "PopulationBoxCenter").sprite = Center;
        RealmUIImages.First(m => m.name == "PopulationBoxRight").sprite = Right;

        //RealmUIImages.First(m => m.name == "CloseButton").sprite = CloseButton;
        wasRealmUIBuilt = true;
    }
    public void DisplayDialogUI(string textData = "", string buttononetext = "", string buttontwotext = "", UnityAction _buttonAction = null, UnityAction _buttonActiontwo = null, bool isDouble = false)
    {
        LoginGlueTexts.First(m => m.name == "InfoText").text = textData;

        GameObject buttonOneObject = LoginGlueButtons.First(m => m.name == "Button1").gameObject;
        GameObject buttonTwoObject = LoginGlueButtons.First(m => m.name == "Button2").gameObject;
        GameObject buttonThreeObject = LoginGlueButtons.First(m => m.name == "Button0").gameObject;
        buttonOneObject.SetActive(false);
        buttonTwoObject.SetActive(false);
        buttonThreeObject.SetActive(false);
        
        if (isDouble)
        {
            buttonOneObject.SetActive(true);
            buttonTwoObject.SetActive(true);

            Button buttonOne = buttonOneObject.GetComponent<Button>();
            Button buttonTwo = buttonTwoObject.GetComponent<Button>();
            buttonOne.onClick.RemoveAllListeners();
            buttonTwo.onClick.RemoveAllListeners();
            SceneHandler.GetChildByName(buttonOneObject, "Text").GetComponent<Text>().text = buttononetext;
            buttonOne.onClick.AddListener(_buttonAction);
            SceneHandler.GetChildByName(buttonTwoObject, "Text").GetComponent<Text>().text = buttontwotext;
            buttonTwo.onClick.AddListener(_buttonActiontwo);
        }
        else
        {
            buttonThreeObject.SetActive(true);

            Button buttonOne = buttonThreeObject.GetComponent<Button>();
            buttonOne.onClick.RemoveAllListeners();
            SceneHandler.GetChildByName(buttonThreeObject, "Text").GetComponent<Text>().text = buttononetext;
            buttonOne.onClick.AddListener(_buttonAction);
        }

        LoginUIPanels.First(m => m.name == "InfoPanel").SetActive(true);
    }
    public void HideDialogUI()
    {
        LoginUIPanels.First(m => m.name == "InfoPanel").SetActive(false);
    }
    public void DisplayRealmUI(Realm[] realms)
    {
        if(!wasRealmUIBuilt)
        {
            BuildRealmUI();
        }

        LoginUIPanels.ForEach(x => x.SetActive(false));

        GameObject realmListUI = LoginUIPanels.First(m => m.name == "RealmListPanel");
        GameObject contentList = SceneHandler.GetChildByName(realmListUI, "RealmContent");
        GameObject realmObject = SceneHandler.GetChildByName(realmListUI, "RealmObject");

        Transform[] ts1 = contentList.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts1)
        {
            if (t.gameObject.name == "RealmContent")
                continue;
            
            Destroy(t.gameObject);
        }

        for (int i = 0; i < realms.Length; i++)
        {
            Realm newRealm = realms[i];

            GameObject button = Instantiate(realmObject);

            button.transform.SetParent(contentList.transform);
            button.transform.localScale = realmObject.transform.localScale;
            button.transform.localPosition = new Vector3(realmObject.transform.localPosition.x, realmObject.transform.localPosition.y, 0);
            button.transform.localRotation = realmObject.transform.localRotation;

            button.name = i.ToString();

            Transform[] ts = button.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in ts)
            {
                if (t.gameObject.name == "RealmName")
                {
                    Text realmText = t.gameObject.GetComponent<Text>();
                    realmText.text = newRealm.Name;
                    if (newRealm.wOnline == 0)
                    { realmText.color = Color.gray; }
                    else { realmText.color = Color.white; }

                    realmText.enabled = true;
                }
                if (t.gameObject.name == "RealmType")
                {
                    Text typeText = t.gameObject.GetComponent<Text>();
                    string realmType = newRealm.Type.ToString();
                    switch (newRealm.Type)
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

                    typeText.text = realmType;
                    if (newRealm.wOnline == 0)
                    { typeText.color = Color.gray; }
                    else { typeText.color = Color.white; }

                    typeText.enabled = true;
                }
                if (t.gameObject.name == "RealmCharacters")
                {
                    Text charachterText = t.gameObject.GetComponent<Text>();
                    charachterText.text = "(" + newRealm.NumChars.ToString() + ")";
                    if (newRealm.wOnline == 0)
                    { charachterText.color = Color.gray; }
                    else { charachterText.color = Color.white; }

                    charachterText.enabled = true;
                }
                if (t.gameObject.name == "RealmPop")
                {
                    Text RealmPopText = t.gameObject.GetComponent<Text>();
                    t.gameObject.GetComponent<Text>().text = newRealm.Population.ToString();
                    string popLevel = "";

                    switch (Convert.ToInt32(newRealm.Population))
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


                    RealmPopText.text = popLevel;
                    if (newRealm.wOnline == 0)
                    {
                        RealmPopText.color = Color.gray;
                        RealmPopText.text = "Offline";
                    }
                    else { RealmPopText.color = Color.white; }

                    RealmPopText.enabled = true;
                }
            }

            //button.GetComponentInChildren<Button>().onClick.AddListener(() => SelectRealm(newRealm));
            button.gameObject.SetActive(true);
        }

        realmListUI.SetActive(true);
    }
    public void HideRealmUI()
    {

    }    
    void DisplayCharacterUI()
    {

    }
    void HideCharacterUI()
    {

    }
    public void Options()
    {

    }
    void Cinematics()
    {

    }
    void Credits()
    {

    }
    void TOS()
    {

    }
    void AccountManage()
    {
        Application.OpenURL(AppHandler.Instance.MANAGE_ACCOUNT_LINK);
    }
    void Website()
    {
        Application.OpenURL(AppHandler.Instance.WEBSITE_LINK);
    }
    void Login()
    {
        string accountName = LoginGlueInputFields.First(m => m.name == "AccountField").text;
        string password = LoginGlueInputFields.First(m => m.name == "PasswordField").text;

        if (accountName.Length < 1)
        {
            DisplayDialogUI("Please enter your account name.", "Okay", "", HideDialogUI);
            return;
        }
        if (password.Length < 1)
        {
            DisplayDialogUI("Please enter your password.", "Okay", "", HideDialogUI);
            return;
        }

        NetworkHandler.mainNetwork._authSession = new AuthenticationSession(NetworkHandler.mainNetwork, accountName, password);
        
        // Audio play Sound\Interface\iUIMainMenuButtonA.wav
    }
    void Quit()
    {
        for (int i = 0; i < AppHandler.Instance.LoadedMPQs.Count; i++)
        {
            AppHandler.Instance.LoadedMPQs[i].Dispose();
        }

        Application.Quit();
    }
    void Empty()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
