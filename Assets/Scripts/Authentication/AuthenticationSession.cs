using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AuthenticationSession
{
    NetworkHandler mainNetwork;
    byte[] version = new byte[] { 3, 3, 5 };
    ushort build = 12340;
    public string mUsername;
    public string mPassword;
    private Srp6 srp;
    private BigInteger A;
    private BigInteger B;
    private BigInteger a;
    private byte[] I;
    private BigInteger M;
    private byte[] N;
    private byte[] g;
    public byte[] mKey;
    private BigInteger Salt;
    private byte[] crcsalt;

    public AuthenticationSession(NetworkHandler _mainNetwork, string Username, string Password)
    {
        mainNetwork = _mainNetwork;
        mUsername = Username.ToUpper();
        mPassword = Password.ToUpper();

        LoginUIHandler.LoginUIInstance.DisplayDialogUI("Connecting...", "Cancel", "", CloseAuthAttempt);

        if (mainNetwork.ConnectToSocket(AppHandler.Instance.REALM_LIST_ADDRESS, AppHandler.Instance.LAST_KNOWN_REALM_PORT))
        {
            Authenticate();
        }

    }
    public void CloseAuthAttempt()
    {
        mainNetwork.Disconnect();
        LoginUIHandler.LoginUIInstance.HideDialogUI();
    }
    public void Authenticate()
    {
        PacketWriter packet = new PacketWriter(AuthOpcode.AUTH_LOGON_CHALLENGE);

        packet.Write((byte)6);
        packet.Write((UInt16)(30 + mUsername.Length));
        packet.Write((byte)'W');
        packet.Write((byte)'o');
        packet.Write((byte)'W');
        packet.Write((byte)'\0');
        packet.Write(version);
        packet.Write(build);
        packet.Write((byte)'6');
        packet.Write((byte)'8');
        packet.Write((byte)'x');
        packet.Write((byte)'\0');
        packet.Write((byte)'n');
        packet.Write((byte)'i');
        packet.Write((byte)'W');
        packet.Write((byte)'\0');
        packet.Write((byte)'S');
        packet.Write((byte)'U');
        packet.Write((byte)'n');
        packet.Write((byte)'e');
        packet.Write((uint)0x3c);
        packet.Write(BitConverter.ToUInt32((mainNetwork.mSocket.LocalEndPoint as IPEndPoint).Address.GetAddressBytes(), 0));

        packet.Write((byte)mUsername.Length);
        packet.Write(Encoding.Default.GetBytes(mUsername));
        mainNetwork.Send(packet.ToArray());

    }
    public void SocketResponse(byte[] data)
    {
        PacketReader reader = new PacketReader(data, true);
        AuthOpcode cmd = reader.LogonOpcode;

        switch (cmd)
        {
            case AuthOpcode.AUTH_LOGON_CHALLENGE:
            case AuthOpcode.AUTH_RECONNECT_CHALLENGE:
                LoginUIHandler.LoginUIInstance.LoginGlueTexts.First(m => m.name == "InfoText").text = "Authenticating...";
                AuthChallangeRequest(reader);
                break;
            case AuthOpcode.AUTH_LOGON_PROOF:
            case AuthOpcode.AUTH_RECONNECT_PROOF:
                LoginUIHandler.LoginUIInstance.LoginGlueTexts.First(m => m.name == "InfoText").text = "Shaking Hands...";
                HandleLogonProof(reader);
                break;
            case AuthOpcode.REALM_LIST:
                LoginUIHandler.LoginUIInstance.LoginGlueTexts.First(m => m.name == "InfoText").text = "Retrieving Realm List...";
                HandleRealmlist(reader);
                break;
            default:
                LoginUIHandler.LoginUIInstance.LoginGlueTexts.First(m => m.name == "InfoText").text = "Received unknown ClientLink: {0} " + cmd.ToString();
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
                LoginUIHandler.LoginUIInstance.DisplayDialogUI("This account has been closed and is no longer available for use.", "Okay", "", CloseAuthAttempt);
                return;
            case AuthResult.WOW_FAIL_UNKNOWN_ACCOUNT:
            case AuthResult.WOW_FAIL_INCORRECT_PASSWORD:
                LoginUIHandler.LoginUIInstance.DisplayDialogUI("Unknown Account/Invalid Password.", "Okay", "", CloseAuthAttempt);
                return;
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

        PacketWriter packet = new PacketWriter(AuthOpcode.AUTH_LOGON_PROOF);
        packet.Write(A);
        packet.Write(M);
        packet.Write(crc_hash);
        packet.Write((byte)0);
        packet.Write((byte)0);
        mainNetwork.Send(packet.ToArray());
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
                LoginUIHandler.LoginUIInstance.DisplayDialogUI("This account is banned.", "Okay", "", CloseAuthAttempt);
                break;
            case AuthResult.WOW_FAIL_VERSION_INVALID:
                LoginUIHandler.LoginUIInstance.DisplayDialogUI("Unable to validate game version. This may be caused by file corruption or interference of another program.", "Okay", "", CloseAuthAttempt);
                break;
            case AuthResult.WOW_FAIL_UNKNOWN_ACCOUNT:
            case AuthResult.WOW_FAIL_INCORRECT_PASSWORD:
                LoginUIHandler.LoginUIInstance.DisplayDialogUI("Unknown Account or Invald Password.", "Okay", "", CloseAuthAttempt);
                break;
            case AuthResult.WOW_FAIL_ALREADY_ONLINE:
                LoginUIHandler.LoginUIInstance.DisplayDialogUI("Account is already logged in.", "Okay", "", CloseAuthAttempt);
                break;
            default:
                break;
        }
    }
    public void RequestRealmlist()
    {
        PacketWriter packet = new PacketWriter(AuthOpcode.REALM_LIST);
        packet.Write(0x00);
        mainNetwork.Send(packet.ToArray());
    }
    public void HandleRealmlist(PacketReader packetIn)
    {
        ushort Length = packetIn.ReadUInt16();
        uint Request = packetIn.ReadUInt32();
        byte realmscount = packetIn.ReadByte();

        Realm[] realms = new Realm[realmscount];
        Realm LastKnownRealm = new Realm();
        try
        {
            for (int i = 0; i < realmscount; i++)
            {
                //realms[i].ID = i;
                realms[i].Type = (byte)packetIn.ReadUInt16();
                realms[i].Color = packetIn.ReadByte();
                realms[i].Flags = packetIn.ReadByte();
                realms[i].Name = packetIn.ReadString();
                realms[i].Address = packetIn.ReadString();
                realms[i].Population = packetIn.ReadFloat();
                realms[i].NumChars = packetIn.ReadByte();
                realms[i].Language = packetIn.ReadByte();
                realms[i].ID = packetIn.ReadByte();

                if (CheckAvailableServerPort(realms[i].Address))
                {
                    realms[i].wOnline = 1;
                }

                if ((realms[i].Flags & 4) != 0)
                {
                    realms[i].version_major = packetIn.ReadByte();
                    realms[i].version_minor = packetIn.ReadByte();
                    realms[i].version_bugfix = packetIn.ReadByte();
                    realms[i].build = packetIn.ReadUInt16();
                }

                if (realms[i].Name.Contains(AppHandler.Instance.LAST_KNOWN_REALMNAME) && realms[i].Name.Length == AppHandler.Instance.LAST_KNOWN_REALMNAME.Length)
                {
                    LastKnownRealm = realms[i];
                }
            }
        }

        catch (Exception ex)
        {
            LoginUIHandler.LoginUIInstance.DisplayDialogUI("Error Retrieving Realm List.", "Okay", "", CloseAuthAttempt);
        }

        if (LastKnownRealm.Name == AppHandler.Instance.LAST_KNOWN_REALMNAME && LastKnownRealm.Name.Length == AppHandler.Instance.LAST_KNOWN_REALMNAME.Length)
        {
            if (LastKnownRealm.wOnline == 1)
            {
                NetworkHandler.mainNetwork._worldSession = new WorldSession(mainNetwork, LastKnownRealm);
            }
        }
        else
        {
            LoginUIHandler.LoginUIInstance.DisplayRealmUI(realms);
        }
    }
    private bool CheckAvailableServerPort(string port)
    {
        Socket testSocket;
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
        catch
        {
            return false;
        }
    }
}

