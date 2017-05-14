using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActualWorld : MonoBehaviour {

    public GameObject Loading;
    public GameObject femaleCharacter;
    public GameObject maleCharacter;
    TextMesh textObject;
    // Use this for initialization
    void Start () {

        Cursor.SetCursor(Exchange.Pointers[0], Vector2.zero, CursorMode.Auto);
        Exchange.maleCharacter = maleCharacter;
        GameObject loadingScreen = Instantiate(Loading, new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity);
        loadingScreen.transform.parent = transform;
        loadingScreen.transform.localScale = new Vector3(1, 1, 1);
        loadingScreen.name = "loadingScreen";

        if (Exchange.worldClient.curChar.Gender == 0)
        {
            GameObject chara = Instantiate(maleCharacter, new Vector3(Exchange.worldClient.curChar.X, Exchange.worldClient.curChar.Y, Exchange.worldClient.curChar.Z), Quaternion.identity);
            chara.transform.parent = transform;
            chara.transform.localScale = new Vector3(1, 1, 1);
            chara.name = Exchange.worldClient.curChar.Name + "Object";
            chara.transform.Rotate(0, -90, 0);

            Transform[] ts = chara.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in ts)
            {
                if (t.gameObject.name == "HumanMale")
                {
                    t.gameObject.name = Exchange.worldClient.curChar.Name;
                }
            }
        }

        if (Exchange.worldClient.curChar.Gender == 1)
        {
            GameObject chara = Instantiate(femaleCharacter, new Vector3(Exchange.worldClient.curChar.X, Exchange.worldClient.curChar.Y, Exchange.worldClient.curChar.Z), Quaternion.identity);
            chara.transform.parent = transform;
            chara.transform.localScale = new Vector3(1, 1, 1);
            chara.name = Exchange.worldClient.curChar.Name;
        }

        textObject = GameObject.Find("CharacterNameText").GetComponent<TextMesh>();
        textObject.name = Exchange.worldClient.curChar.Name + "Text";
        textObject.text = Exchange.worldClient.curChar.Name;
    }

    // Update is called once per frame
    void Update () {
		
        if(LoadingScreen.Loaded == 1)
        {
            Destroy(GameObject.Find("loadingScreen"));
            LoadingScreen.Loaded = 0;
        }
	}
}
