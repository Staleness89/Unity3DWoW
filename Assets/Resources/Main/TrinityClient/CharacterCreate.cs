using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreate : MonoBehaviour {

    GameObject currentSelectedModel;
    Button back;
    Button CreateAccept;
    InputField newCharName;
    // Use this for initialization
    void Start () {

        Exchange.newCharacter = new Character();
        newCharName = GameObject.Find("characterName").GetComponent<InputField>();
        back = GameObject.Find("CreateBack").GetComponent<Button>();
        back.onClick.AddListener(backFunc);
        CreateAccept = GameObject.Find("createAccept").GetComponent<Button>();
        CreateAccept.onClick.AddListener(createAccept);

        GameObject modelLocation;

        if (!GameObject.Find("RaceSelected"))
        {

            GameObject Race = GameObject.Find("Human1");

            if (GameObject.Find("RaceSelected"))
            {
                Destroy(GameObject.Find("RaceSelected"));
            }

            Global.Race = (byte)Convert.ToInt32(Race.name.Substring(Race.name.Length - 1));
            Global.selectedRace = Race.name;

            GameObject tempHigh = Instantiate(Race, new Vector3(Race.transform.position.x, Race.transform.transform.position.y, Race.transform.position.z), Quaternion.identity);
            tempHigh.transform.parent = GameObject.Find("slot").gameObject.transform;
            tempHigh.name = "RaceSelected";
            Image selected = GameObject.Find("RaceSelected").GetComponent<Image>();
            selected.sprite = Global.selected;
            selected.rectTransform.sizeDelta = new Vector2(55, 55);
        }

        if (!GameObject.Find("ClassSelected"))
        {
            GameObject Class = GameObject.Find("class1");

            GameObject SelectedClass = Instantiate(Class, new Vector3(Class.transform.position.x, Class.transform.position.y, transform.position.z), Quaternion.identity);
            SelectedClass.transform.parent = GameObject.Find("slot").gameObject.transform;
            SelectedClass.name = "ClassSelected";

            Image selected = GameObject.Find("ClassSelected").GetComponent<Image>();
            selected.sprite = Global.selected;
            selected.rectTransform.sizeDelta = new Vector2(45, 45);

            Global.Class = (byte)Convert.ToInt32(Class.name.Substring(Class.name.Length - 1));
            Global.selectedClass = Class.name;

        }

        if (!GameObject.Find("GenderSelected"))
        {
            GameObject Gender = GameObject.Find("Male0");

            GameObject SelectedGender = Instantiate(Gender, new Vector3(Gender.transform.position.x, Gender.transform.position.y, transform.position.z), Quaternion.identity);
            SelectedGender.transform.parent = GameObject.Find("slot").gameObject.transform;
            SelectedGender.name = "GenderSelected";

            Image selected = GameObject.Find("GenderSelected").GetComponent<Image>();
            selected.sprite = Global.selected;
            selected.rectTransform.sizeDelta = new Vector2(45, 45);

            Global.Gender = (byte)Convert.ToInt32(Gender.name.Substring(Gender.name.Length - 1));
            Global.selectedGender = Gender.name;

        }
        
        if (!GameObject.Find("CreatingMale"))
        {
            modelLocation = GameObject.Find("targetLocation");

            currentSelectedModel = Instantiate(Global.maleModel, new Vector3(modelLocation.transform.position.x, modelLocation.transform.transform.position.y, modelLocation.transform.position.z), Quaternion.identity);
            currentSelectedModel.transform.parent = GameObject.Find("targetLocation").gameObject.transform;
            currentSelectedModel.transform.localScale = new Vector3(2, 2, 2);//15.58, -18.61,-4.68
            currentSelectedModel.name = "CreatingMale";
            currentSelectedModel.transform.Rotate(0, -90, 0);

            Animator animator = GameObject.Find("CreatingMale").GetComponent<Animator>();
            animator.SetBool("Grounded", true);
            animator.Play("Stand");
        }
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void createAccept()
    {
        Global.showNotifyBox("Creating Character.", "Okay");
        if (newCharName.text.Length > 4)
        {
            Exchange.newCharacter.Name = newCharName.text;
            Exchange.newCharacter.Race = Global.Race;
            Exchange.newCharacter.Gender = Global.Gender;
            Exchange.newCharacter.Class = Global.Class;
            Exchange.newCharacter.Skin = 1;
            Exchange.newCharacter.Face = 1;
            Exchange.newCharacter.HairStyle = 1;
            Exchange.newCharacter.FacialHair = 1;
            Exchange.newCharacter.OutfitID = 1;

            AuthHandler.HandleCharCreate(Exchange.newCharacter, Exchange.worldClient);
        }
        else
        {
            Global.showNotifyBox("Character Name Too Short.", "Okay");
        }
    }


    void backFunc()
    {
        Global.closeCharCreate();
        Global.showNotifyBox("Retrieving Character List...", "Okay");
        PacketWriter outpacket = new PacketWriter(WorldServerOpCode.CMSG_CHAR_ENUM);
        Exchange.worldClient.Send(outpacket);
    }
}
