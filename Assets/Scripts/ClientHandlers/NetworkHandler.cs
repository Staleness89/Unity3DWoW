using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using UnityEngine;

public class NetworkHandler : MonoBehaviour
{
    public static NetworkHandler mainNetwork;
    public Socket mSocket = null;
    public byte[] DataBuffer;

    // Start is called before the first frame update
    void Start()
    {
        if (mainNetwork != null)
            return;

        mainNetwork = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (mSocket != null)
        {
            while (mSocket.Available > 0)
            {
                DataBuffer = new byte[mSocket.Available];
                mSocket.Receive(DataBuffer, DataBuffer.Length, SocketFlags.None);

                //HandleData(DataBuffer);
            }

        }
    }

    public bool ConnectToSocket(string address, int port)
    {
        Regex DnsMatch = new Regex("[a-zA-Z]");
        IPAddress ASAddr;

        try
        {
            if (DnsMatch.IsMatch(address))
                ASAddr = Dns.GetHostEntry(address).AddressList[0];
            else
                ASAddr = IPAddress.Parse(address);

            IPEndPoint ASDest = new IPEndPoint(ASAddr, port);

            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            mSocket.Connect(ASDest);
            return true;
        }
        catch
        {
            //AppHandler.Instance.SpawnInfoUI("Unable To Connect.", "Okay", AppHandler.Instance.CloseInfoPanel);
            return false;
        }
    }
}
