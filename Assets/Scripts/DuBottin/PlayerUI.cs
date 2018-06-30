using Assets.Scripts.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {

    Image playerHealth;
    Image targetHealth;
    // Use this for initialization
    void Start () {
        playerHealth = UnityEngine.GameObject.Find("healthBar").GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {

        if (Exchange.gameClient.Player != null)
        {
            uint healthPercent = (Exchange.gameClient.Player.Health * 200 + Exchange.gameClient.Player.MaxHealth) / (Exchange.gameClient.Player.MaxHealth * 2);
            playerHealth.fillAmount = healthPercent / 100f;
        }

        switch(Exchange.gameClient.Player.Target)
        {
            targetHealth = UnityEngine.GameObject.Find("targethealthBar").GetComponent<Image>();

		case Unit:
                	Unit unit = Exchange.gameClient.Objects[Exchange.gameClient.Player.Target.GUID] as Unit;
                	uint healthPercent = (unit.Health * 200 + unit.MaxHealth) / (unit.MaxHealth * 2);
                	targetHealth.fillAmount = healthPercent / 100f;
			break;			
		case Player:
                	Player player = Exchange.gameClient.Objects[Exchange.gameClient.Player.Target.GUID] as Player;
                	uint healthPercent = (player.Health * 200 + player.MaxHealth) / (player.MaxHealth * 2);
                	targetHealth.fillAmount = healthPercent / 100f;
			break;
		default:
			break;
           
        }
    }
}
