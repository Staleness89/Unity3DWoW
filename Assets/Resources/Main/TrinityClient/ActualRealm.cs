using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActualRealm : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void targetRealm()
    {
        Global.selectRealm(gameObject);
    }
}
