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
    Button EnterWorld;
    // Use this for initialization
    void Start () {

        Realms = GameObject.Find("RealmsButton").GetComponent<Button>();
        Realms.onClick.AddListener(RealmsClick);
        /*createCharacter = GameObject.Find("RealmsButton").GetComponent<Button>();
        createCharacter.onClick.AddListener(RealmsClick);
        deleteCharacter = GameObject.Find("RealmsButton").GetComponent<Button>();
        deleteCharacter.onClick.AddListener(RealmsClick);
        back = GameObject.Find("RealmsButton").GetComponent<Button>();
        back.onClick.AddListener(RealmsClick);
        EnterWorld = GameObject.Find("RealmsButton").GetComponent<Button>();
        EnterWorld.onClick.AddListener(RealmsClick);*/

        animator = GameObject.Find("HumanMale").GetComponent<Animator>();
        animator.SetBool("Grounded", true);
        animator.Play("Stand");
	}

    // Update is called once per frame
    void Update()
    {

    }

    void RealmsClick()
    {
        Global.closeCharList();
        Global.showRealmList(Exchange.realms);
    }
}
