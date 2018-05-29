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

        if (Exchange.authClient.Player != null)
        {
            uint healthPercent = (Exchange.authClient.Player.Health * 200 + Exchange.authClient.Player.MaxHealth) / (Exchange.authClient.Player.MaxHealth * 2);
            playerHealth.fillAmount = healthPercent / 100f;
        }

        if (Exchange.authClient.Player.Target != null)
        {
            targetHealth = UnityEngine.GameObject.Find("targethealthBar").GetComponent<Image>();

            if (Exchange.authClient.Player.Target is Unit)
            {
                Unit unit = Exchange.authClient.Objects[Exchange.authClient.Player.Target.GUID] as Unit;
                uint healthPercent = (unit.Health * 200 + unit.MaxHealth) / (unit.MaxHealth * 2);
                targetHealth.fillAmount = healthPercent / 100f;
            }

            if (Exchange.authClient.Player.Target is Player)
            {
                Player player = Exchange.authClient.Objects[Exchange.authClient.Player.Target.GUID] as Player;
                uint healthPercent = (player.Health * 200 + player.MaxHealth) / (player.MaxHealth * 2);
                targetHealth.fillAmount = healthPercent / 100f;
            }
        }
    }
}
