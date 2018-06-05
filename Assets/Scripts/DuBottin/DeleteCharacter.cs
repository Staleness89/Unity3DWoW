using Assets.Scripts.Shared;
using Client.World;
using Client.World.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteCharacter : MonoBehaviour {

    Button Okay;
    Button Back;
    Text delField;
    // Use this for initialization
    void Start () {
        Back = UnityEngine.GameObject.Find("deleteCancel").GetComponent<Button>();
        Back.onClick.AddListener(BackFun);
        Okay = UnityEngine.GameObject.Find("deleteOk").GetComponent<Button>();
        Okay.onClick.AddListener(OkayFun);
        delField = UnityEngine.GameObject.Find("deleteMeText").GetComponent<Text>();
    }

    void BackFun()
    {
        Destroy(gameObject);
    }

    void OkayFun()
    {
        OutPacket result = new OutPacket(WorldCommand.CMSG_CHAR_DELETE);
        result.Write(Exchange.SelectedCharacter.GUID);
        Exchange.gameClient.SendPacket(result);
    }

    // Update is called once per frame
    public static bool deleteSuccessful = false;

    void Update () {
		if(delField.text == "delete" || delField.text == "DELETE")
        {
            Okay.enabled = true;
        }
        else
        {
            Okay.enabled = false;
        }

        if(deleteSuccessful)
        {
            deleteSuccessful = false;
            OutPacket request = new OutPacket(WorldCommand.CMSG_CHAR_ENUM);
            Exchange.gameClient.SendPacket(request);
            Destroy(UnityEngine.GameObject.Find("CharacterUI"));
            UnityEngine.GameObject CharacterUI = Instantiate(MainLogin.LoadPrefab("CharacterUI"), new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity);
            CharacterUI.transform.SetParent(UnityEngine.GameObject.Find("Canvas").gameObject.transform);
            CharacterUI.transform.localScale = new Vector3(1, 1, 1);
            CharacterUI.name = "CharacterUI";
            Destroy(gameObject);
        }
	}
}
