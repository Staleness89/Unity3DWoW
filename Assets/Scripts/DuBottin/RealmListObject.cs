using Assets.Scripts.Shared;
using Client.Authentication;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealmListObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public static void selectRealm(UnityEngine.GameObject gameObject)
    {
        foreach (WorldServerInfo rl in Exchange.authClient.RealmServerList)
        {
            var sprite = Resources.Load<Sprite>("Trans");
            Image realmSelect = UnityEngine.GameObject.Find(rl.Name + "RealmSelect" + rl.ID).GetComponent<Image>();
            realmSelect.sprite = sprite;
        }

        foreach (WorldServerInfo rl in Exchange.authClient.RealmServerList)
        {
            if (rl.Name == gameObject.name)
            {
                Exchange.CurrentRealm = rl;

                if (Exchange.CurrentRealm.wOnline == 1)
                {
                    var sprite = Resources.Load<Sprite>("list_Focus");
                    Image realmSelect = UnityEngine.GameObject.Find(rl.Name + "RealmSelect" + rl.ID).GetComponent<Image>();
                    realmSelect.sprite = sprite;
                }
            }
        }
    }


    public void SetRealm()
    {
        selectRealm(gameObject);
    }
}
