using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterList : MonoBehaviour {

    Animator animator;
    Button Realms;
    Button createCharacter;
    Button deleteCharacter;
    Button back;
    Button deleteBack;
    Button EnterWorld;
    public static bool updateDelete = false;
    // Use this for initialization
    void Start () {
        
        Realms = GameObject.Find("RealmsButton").GetComponent<Button>();
        Realms.onClick.AddListener(RealmsClick);
        /*createCharacter = GameObject.Find("RealmsButton").GetComponent<Button>();
        createCharacter.onClick.AddListener(RealmsClick);*/
        deleteCharacter = GameObject.Find("DeleteCharacter").GetComponent<Button>();
        deleteCharacter.onClick.AddListener(DeleteCharacterFunc);
        back = GameObject.Find("BackButton").GetComponent<Button>();
        back.onClick.AddListener(backFunc);
        /*EnterWorld = GameObject.Find("RealmsButton").GetComponent<Button>();
        EnterWorld.onClick.AddListener(RealmsClick);*/

        if (AuthHandler.CharacterCount > 0)
        {
            animator = GameObject.Find("HumanMale").GetComponent<Animator>();
            animator.SetBool("Grounded", true);
            animator.Play("Stand");
        }
	}
 
    // Update is called once per frame
    void Update()
    {
        if (updateDelete)
        {
            InputField deleteDelete = GameObject.Find("DeleteInput").GetComponent<InputField>();
            if (deleteDelete.text == "delete" || deleteDelete.text == "Delete" || deleteDelete.text == "DELETE")
            {
                Button deleteConfirm = GameObject.Find("deleteConfirm").GetComponent<Button>();
                deleteConfirm.enabled = true;
                deleteConfirm.onClick.AddListener(deleteConfirmfunc);

                Text DeleteCharacterButton = GameObject.Find("deleteConfirmText").GetComponent<Text>();
                DeleteCharacterButton.color = Color.yellow;
            }
            else
            {
                Button deleteConfirm = GameObject.Find("deleteConfirm").GetComponent<Button>();
                deleteConfirm.enabled = false;

                Text DeleteCharacterButton = GameObject.Find("deleteConfirmText").GetComponent<Text>();
                DeleteCharacterButton.color = Color.grey;
            }
        }
    }

    void deleteConfirmfunc()
    {
        AuthHandler.HandleCharDelete(Exchange.worldClient.curChar, Exchange.worldClient);        
    }

    public void selectCharacter()
    {
        foreach (Character c in Exchange.worldClient.Charlist)
        {
            if (GameObject.Find(c.Name + "SelectedCurrent"))
            {
                GameObject old = GameObject.Find(c.Name + "SelectedCurrent");
                old.name = c.Name + "Selected";
                Image go = GameObject.Find(c.Name + "Selected").GetComponent<Image>();
                go.sprite = Global.realmListClear;
            }

            if (gameObject.name == c.Name + "Selected")
            {
                Exchange.worldClient.curChar = c;
                gameObject.name = gameObject.name + "Current";
            }
        }
    }

    public void mouseOut()
    {
        if (gameObject.name == Exchange.worldClient.curChar.Name + "SelectedCurrent")
        {
            Image go = GameObject.Find(gameObject.name).GetComponent<Image>();
            go.sprite = Global.realmListHighlight;
            return;
        }
        else
        {
            Image go = GameObject.Find(gameObject.name).GetComponent<Image>();
            go.sprite = Global.realmListClear;
        }
    }

    public void mouseIn()
    {
        Image go = GameObject.Find(gameObject.name).GetComponent<Image>();
        go.sprite = Global.realmListHighlight;
    }

    void RealmsClick()
    {
        Global.closeCharList();
        Global.showRealmList(Exchange.realms);
    }

    void backFunc()
    {
        Global.closeCharList();
        Global.showLogin();
    }

    void DeleteCharacterFunc()
    {
        if (GameObject.Find("DeleteNotify"))
        {
            return;
        }

        Global.showDeleteNotify();

        deleteBack = GameObject.Find("deleteBack").GetComponent<Button>();
        deleteBack.onClick.AddListener(deleteBackbutton);
                
        updateDelete = true;
    }

    void deleteBackbutton()
    {
        updateDelete = false;
        Global.closeDeleteNotify();
    }
}
