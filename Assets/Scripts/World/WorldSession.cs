using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSession
{
    NetworkHandler mainNetwork;
    Realm realm;
    public WorldSession(NetworkHandler _mainNetwork, Realm _realm)
    {
        mainNetwork = _mainNetwork;
        realm = _realm;

        string[] address = realm.Address.Split(':');

        if(mainNetwork.ConnectToSocket(address[0], int.Parse(address[1])))
        {

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
