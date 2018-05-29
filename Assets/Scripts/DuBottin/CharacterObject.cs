using Assets.Scripts.Shared;
using Client.Authentication;
using Client.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (Exchange.characters.Length > 0)
        {
            var tarSprite = Resources.Load<Sprite>("CharacterObjectTargeted");
            Exchange.SelectedCharacter = Exchange.characters[0];
            Image realmSelect = UnityEngine.GameObject.Find(Exchange.characters[0].Name).GetComponent<Image>();
            realmSelect.sprite = tarSprite;
        }

        }
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void selectCharacter(UnityEngine.GameObject gameObject)
    {
        var regSprite = Resources.Load<Sprite>("CharacterObjectBG");
        var tarSprite = Resources.Load<Sprite>("CharacterObjectTargeted");

        foreach (Character rl in Exchange.characters)
        {            
            Image realmSelect = UnityEngine.GameObject.Find(rl.Name).GetComponent<Image>();
            realmSelect.sprite = regSprite;
        }

        foreach (Character rl in Exchange.characters)
        {
            if (rl.Name == gameObject.name)
            {
                Exchange.SelectedCharacter = rl;

                if (Exchange.CurrentRealm.wOnline == 1)
                {                   
                    Image realmSelect = UnityEngine.GameObject.Find(rl.Name).GetComponent<Image>();
                    realmSelect.sprite = tarSprite;
                }
            }
        }
    }

    public void OnMouseOver()
    {
        var tarSprite = Resources.Load<Sprite>("CharacterObjectTargeted");

        Image realmSelect = UnityEngine.GameObject.Find(gameObject.name).GetComponent<Image>();
        realmSelect.sprite = tarSprite;
    }

    public void OnMouseExit()
    {
        var regSprite = Resources.Load<Sprite>("CharacterObjectBG");
        var tarSprite = Resources.Load<Sprite>("CharacterObjectTargeted");

        Image realmSelect = UnityEngine.GameObject.Find(gameObject.name).GetComponent<Image>();

        if(gameObject.name != Exchange.SelectedCharacter.Name)
        { 
            realmSelect.sprite = regSprite;
        }
    }

    public void SetCharacter()
    {
        selectCharacter(gameObject);
    }
}
