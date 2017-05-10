using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;


public class AuthSocket
{
    readonly string mHost;
    readonly int mPort;
    public readonly string mUsername;
    public readonly string mPassword;
    public bool Connected;
    public volatile bool listenSocket = true;
    byte[] DataBuffer;
    public Realm[] Realmlist = new Realm[0];
    public Char[] Charlist;
    Socket testSocket = null;
    private Srp6 srp;
    private BigInteger A;
    private BigInteger B;
    private BigInteger a;
    private byte[] I;
    private BigInteger M;
    private byte[] N;
    private byte[] g;
    public byte[] mKey;
    public Realm LastKnownRealm;
    private BigInteger Salt;
    private byte[] crcsalt;

    public Socket mSocket = null;
    public TextWriter tw;

    public AuthSocket(string username, string password, string realm)
    {
        mHost = realm;
        mPort = 3724;
        mUsername = username.ToUpper();
        mPassword = password.ToUpper();
    }

    public void Login()
    {
        Regex DnsMatch = new Regex("[a-zA-Z]");
        IPAddress ASAddr;

        try
        {
            if (DnsMatch.IsMatch(mHost))
                ASAddr = Dns.GetHostEntry(mHost).AddressList[0];
            else
                ASAddr = System.Net.IPAddress.Parse(mHost);

            IPEndPoint ASDest = new IPEndPoint(ASAddr, 3724);

            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            mSocket.Connect(ASDest);

        }
        catch (Exception ex)
        {
            Global.showNotifyBox("Unable To Connect", "Okay");
            return;
        }
        
        Authenticate();
    }

    public void Authenticate()
    {
        PacketWriter packet = new PacketWriter(LogonServerOpCode.AUTH_LOGON_CHALLENGE);

        packet.Write((byte)3);
        packet.Write((UInt16)(30 + mUsername.Length));
        packet.Write((byte)'W'); packet.Write((byte)'o'); packet.Write((byte)'W'); packet.Write((byte)'\0');        // WoW

        packet.Write((byte)1);
        packet.Write((byte)12);
        packet.Write((byte)1);
        packet.Write((UInt16)5875);

        packet.Write((byte)'6'); packet.Write((byte)'8'); packet.Write((byte)'x'); packet.Write((byte)'\0');     // 68x
        packet.Write((byte)'n'); packet.Write((byte)'i'); packet.Write((byte)'W'); packet.Write((byte)'\0');     // niW

        packet.Write((byte)'B'); packet.Write((byte)'G'); packet.Write((byte)'n'); packet.Write((byte)'e');  // SUne

        packet.Write(1);

        packet.Write((byte)127); packet.Write((byte)0); packet.Write((byte)0); packet.Write((byte)1);       // Interestingly, mac sends IPs in reverse order.

        packet.Write((byte)mUsername.Length);
        packet.Write(Encoding.Default.GetBytes(mUsername)); // Name - NOT null terminated
        Send(packet);

    }

    public void OnConnect()
    {
        if (mSocket != null)
        {
            if (mSocket.Connected)
            {
                while (mSocket.Available > 0)
                {

                    DataBuffer = new byte[mSocket.Available];
                    mSocket.Receive(DataBuffer, DataBuffer.Length, SocketFlags.None);

                    HandleData(DataBuffer);
                }
            }
        }
        else
        {
            // ConnectedToAuthSocket = false;
        }
    }

    void HandleData(byte[] data)
    {
        PacketReader reader = new PacketReader(data, true);
        LogonServerOpCode cmd = reader.LogonOpcode;

        switch (cmd)
        {
            case LogonServerOpCode.AUTH_LOGON_CHALLENGE:
            case LogonServerOpCode.AUTH_RECONNECT_CHALLENGE:
                Global.showNotifyBox("Authenticating", "Cancel");
                AuthChallangeRequest(reader);
                break;
            case LogonServerOpCode.AUTH_LOGON_PROOF:
            case LogonServerOpCode.AUTH_RECONNECT_PROOF:
                Global.showNotifyBox("Shaking Hands...", "Cancel");
                HandleLogonProof(reader);
                break;
            case LogonServerOpCode.REALM_LIST:
                Global.showNotifyBox("Retrieving Realm List...", "Cancel");
                HandleRealmlist(reader);
                break;
            default:
                //Log.Message(LogType.NORMAL, "Received unknown ClientLink: {0}", cmd);
                break;
        }
    }

    public void AuthChallangeRequest(PacketReader packetIn)
    {
        packetIn.ReadByte();

        AuthResult result = (AuthResult)packetIn.ReadByte();

        switch (result)
        {
            case AuthResult.WOW_SUCCESS:
                break;
            case AuthResult.WOW_FAIL_BANNED:
                Global.showNotifyBox("This account has been closed and is no longer available for use.", "Okay");
                return;
                break;
            case AuthResult.WOW_FAIL_UNKNOWN_ACCOUNT:
                Global.showNotifyBox("Unknown Account/Invalid Password.", "Okay");
                return;
                break;
            case AuthResult.WOW_FAIL_INCORRECT_PASSWORD:
                Global.showNotifyBox("Unknown Account/Invalid Password.", "Okay");
                return;
                break;
            default:
                break;
        }

        B = new BigInteger(packetIn.ReadBytes(32));
        byte glen = packetIn.ReadByte();
        g = packetIn.ReadBytes(glen);
        byte Nlen = packetIn.ReadByte();
        N = packetIn.ReadBytes(Nlen);
        Salt = new BigInteger(packetIn.ReadBytes(32));
        crcsalt = packetIn.ReadBytes(16);

        BigInteger S;
        srp = new Srp6(new BigInteger(N), new BigInteger(g));

        do
        {
            a = BigInteger.Random(19 * 8);
            A = srp.GetA(a);

            I = Srp6.GetLogonHash(mUsername, mPassword);

            BigInteger x = Srp6.Getx(Salt, I);
            BigInteger u = Srp6.Getu(A, B);
            S = srp.ClientGetS(a, B, x, u);
        }
        while (S < 0);

        mKey = Srp6.ShaInterleave(S);
        M = srp.GetM(mUsername, Salt, A, B, new BigInteger(mKey));

        packetIn.ReadByte();

        Sha1Hash sha;
        byte[] files_crc;

        files_crc = new byte[20];

        sha = new Sha1Hash();
        sha.Update(A);
        sha.Update(files_crc);
        byte[] crc_hash = sha.Final();

        PacketWriter packet = new PacketWriter(LogonServerOpCode.AUTH_LOGON_PROOF);
        packet.Write(A);
        packet.Write(M);
        packet.Write(crc_hash);
        packet.Write((byte)0);
        packet.Write((byte)0);
        Send(packet);
    }

    public void HandleLogonProof(PacketReader packetIn)
    {
        AuthResult result = (AuthResult)packetIn.ReadByte();

        switch (result)
        {
            case AuthResult.WOW_SUCCESS:
                RequestRealmlist();
                break;
            case AuthResult.WOW_FAIL_BANNED:
                Global.showNotifyBox("This account is banned.", "Okay");
                break;
            case AuthResult.WOW_FAIL_UNKNOWN_ACCOUNT:
                Global.showNotifyBox("Unknown Account.", "Okay");
                break;
            case AuthResult.WOW_FAIL_INCORRECT_PASSWORD:
                Global.showNotifyBox("Invald Password.", "Okay");
                break;
            case AuthResult.WOW_FAIL_ALREADY_ONLINE:
                Global.showNotifyBox("Account is already logged in.", "Okay");
                break;
            default:
                break;
        }
    }

    public void RequestRealmlist()
    {
        PacketWriter packet = new PacketWriter(LogonServerOpCode.REALM_LIST);
        packet.Write(0x00);
        Send(packet);
    }

    public void HandleRealmlist(PacketReader packetIn)
    {
        ushort Length = packetIn.ReadUInt16();
        uint Request = packetIn.ReadUInt32();
        byte realmscount = packetIn.ReadByte();

        Realm[] realms = new Realm[realmscount];
        try
        {
            for (int i = 0; i < realmscount; i++)
            {
                realms[i].ID = i;
                realms[i].Type = (byte)packetIn.ReadUInt32();
                realms[i].Color = packetIn.ReadByte();
                realms[i].Name = packetIn.ReadString();
                realms[i].Address = packetIn.ReadString();
                realms[i].Population = packetIn.ReadFloat();
                realms[i].NumChars = packetIn.ReadByte();
                realms[i].Language = packetIn.ReadByte();
                packetIn.ReadByte();
                if (realms[i].Name.Contains(Main.LAST_KNOWN_REALM_LIST) && realms[i].Name.Length == Main.LAST_KNOWN_REALM_LIST.Length)
                {
                    if (CheckAvailableServerPort(realms[i].Address))
                    {
                        realms[i].wOnline = 1;
                    }

                    LastKnownRealm = realms[i];
                }

                if (CheckAvailableServerPort(realms[i].Address))
                {
                    realms[i].wOnline = 1;
                }

            }

            packetIn.ReadUInt16();

            Realmlist = realms;
            Exchange.realms = realms;

            if (LastKnownRealm.Name == Main.LAST_KNOWN_REALM_LIST)
            {
                if (LastKnownRealm.wOnline == 1)
                {
                    
                    Exchange.currRealm = LastKnownRealm;
                    Exchange.worldClient = new World(mUsername, LastKnownRealm, mKey);
                    Exchange.worldClient.Connect();
                }
                else
                {
                    Global.showRealmList(Realmlist);
                }
            }
            else
            {
                Global.showRealmList(Realmlist);
            }

        }

        catch (Exception ex)
        {
            //Exchange.Disconnect();
        }

    }
    
    private bool CheckAvailableServerPort(string port)
    {
        string[] address = port.Split(':');
        IPAddress WSAddr = Dns.GetHostAddresses(address[0])[0];
        int WSPort = Int32.Parse(address[1]);
        IPEndPoint ep = new IPEndPoint(WSAddr, WSPort);

        try
        {
            testSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            testSocket.Connect(ep);
            testSocket.Close();

            return true;
        }
        catch (SocketException ex)
        {
            return false;
        }
    }

    public void Send(PacketWriter packet)
    {
        if (mSocket.Connected)
        {
            Byte[] Data = packet.ToArray();
            mSocket.Send(Data);
        }
    }
}
