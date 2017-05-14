using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Functions : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void mouseOut()
    {        
        Destroy(GameObject.Find("TempHigh"));
    }

    public void mouseIn()
    {
        GameObject tempHigh = Instantiate(gameObject, new Vector3(gameObject.transform.position.x, gameObject.transform.transform.position.y, gameObject.transform.position.z), Quaternion.identity);
        tempHigh.transform.parent = GameObject.Find("slot").gameObject.transform;
        tempHigh.name = "TempHigh";
        Image go = GameObject.Find("TempHigh").GetComponent<Image>();
        go.sprite = Global.selected;

        if (gameObject.name == "Male0" || gameObject.name == "Female1" || gameObject.name == "class1" || gameObject.name == "class2" || gameObject.name == "class3" || gameObject.name == "class4" || gameObject.name == "class5" || gameObject.name == "class6")
        {
            go.rectTransform.sizeDelta = new Vector2(45, 45);
        }
        else
        {
            go.rectTransform.sizeDelta = new Vector2(55, 55);
        }

    }

    public void selectRace(string race)
    {
        GameObject SelectedRace;

        GameObject Race = GameObject.Find(race);

        if (GameObject.Find("RaceSelected"))
        {
            Destroy(GameObject.Find("RaceSelected"));
        }

        Global.Race = (byte)Convert.ToInt32(Race.name.Substring(Race.name.Length - 1));
        Global.selectedRace = Race.name;

        SelectedRace = GameObject.Find("TempHigh");
        SelectedRace.name = "RaceSelected";
        
        Image selected = GameObject.Find("RaceSelected").GetComponent<Image>();
        selected.sprite = Global.selected;
        selected.rectTransform.sizeDelta = new Vector2(55, 55);
        
    }

    public void selectClass(string c)
    {
        GameObject SelectedClass;

        GameObject Class = GameObject.Find(c);

        if (GameObject.Find("ClassSelected"))
        {
            Destroy(GameObject.Find("ClassSelected"));
        }

        Global.Class = (byte)Convert.ToInt32(Class.name.Substring(Class.name.Length - 1));
        Global.selectedClass = Class.name;

        SelectedClass = GameObject.Find("TempHigh");
        SelectedClass.name = "ClassSelected";

        Image selected = GameObject.Find("ClassSelected").GetComponent<Image>();
        selected.sprite = Global.selected;
        selected.rectTransform.sizeDelta = new Vector2(45, 45);
        
    }

    public void selectGender(string gender)
    {
        GameObject SelectedGender;

        GameObject Gender = GameObject.Find(gender);

        if (GameObject.Find("GenderSelected"))
        {
            Destroy(GameObject.Find("GenderSelected"));
        }

        Global.Gender = (byte)Convert.ToInt32(Gender.name.Substring(Gender.name.Length - 1));
        Global.selectedGender = Gender.name;
        
        SelectedGender = GameObject.Find("TempHigh");
        SelectedGender.name = "GenderSelected";

        Image selected = GameObject.Find("GenderSelected").GetComponent<Image>();
        selected.sprite = Global.selected;
        selected.rectTransform.sizeDelta = new Vector2(45, 45);

    }

}
