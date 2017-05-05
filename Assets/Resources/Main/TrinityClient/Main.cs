using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    public GameObject login;
    //public GameObject login;
    // Use this for initialization
    void Start()
    {

        GameObject myBrick = Instantiate(login, new Vector3(Screen.width / 2, Screen.height / 2, 0), Quaternion.identity);
        myBrick.transform.parent = transform;
        myBrick.name = "Login";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
