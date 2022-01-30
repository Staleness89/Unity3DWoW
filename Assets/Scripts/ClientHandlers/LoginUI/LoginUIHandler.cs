using Foole.Mpq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoginUIHandler : MonoBehaviour
{
    // Interface\GLUES\MODELS\UI_MainMenu_Northrend\UI_MainMenu_Northrend.M2 //MainUIBG
    public List<Text> LoginGlueTexts;
    public List<Button> LoginGlueButtons;
    public List<InputField> LoginGlueInputFields;
    public List<Image> LoginUIImages;
    public AudioSource Audio;
    public Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        Audio.clip = WavUtility.ToAudioClip(AppHandler.Instance.SearchMPQ(@"Sound\Music\GlueScreenMusic\WotLK_main_title.mp3"));
        Audio.Play();
        BuildBackground();
        BuildLoginUI();
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
    void Options()
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

    }
    void Website()
    {

    }
    void Login()
    {
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
