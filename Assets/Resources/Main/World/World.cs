using Assets.Scripts.World.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World
{

    [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
    public static extern uint MM_GetTime();

    public string mUsername;
    public byte[] mKey;
    public bool Connected;
    public static bool HasWorldLogin = false;
    public bool inWorld = false;
    public bool LoadingDone = false;

    public UInt32 ServerSeed;
    public UInt32 ClientSeed;
    public System.Random random = new System.Random();
    public PacketCrypt mCrypt;
    public Character[] Charlist = new Character[0];
    public Character curChar;

    Queue PacketQueue = null;


    public Socket mSocket = null;

    public Realm realm;

    public System.Timers.Timer Ping = new System.Timers.Timer();
    public UInt32 Ping_Seq;
    public UInt32 Ping_Req_Time;
    public UInt32 Ping_Res_Time;
    public UInt32 Latency;
    WorldServerOpCode code;

    public ObjectMgr objectMgr = null;
    public MovementMgr movementMgr = null;
    public CombatMgr combatMgr = null;
    public TerrainMgr terrainMgr = null;
    public Thread packetThread = null;

    public byte[] DataBuffer;


    public World(string user, Realm rl, byte[] key)
    {
        mUsername = user.ToUpper();
        objectMgr = new ObjectMgr();
        movementMgr = new MovementMgr(this);
        combatMgr = new CombatMgr(this);
        terrainMgr = new TerrainMgr();
        realm = rl;
        mKey = key;
        PacketQueue = new Queue();
    }

    public void Connect()
    {
        string[] address = realm.Address.Split(':');
        byte[] test = new byte[1];
        test[0] = 10;
        mCrypt = new PacketCrypt(test);
        IPAddress WSAddr = Dns.GetHostAddresses(address[0])[0];
        int WSPort = Int32.Parse(address[1]);
        IPEndPoint ep = new IPEndPoint(WSAddr, WSPort);
        Thread.Sleep(1000);
        try
        {
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            mSocket.Connect(ep);
            Debug.LogWarning("Successfully connected to WorldServer at: " + realm.Address);
        }
        catch (SocketException ex)
        {
            Debug.LogWarning("Failed to connect to realm: " + ex.Message);
            //Exchange.Disconnect();
            return;
        }

        byte[] nullA = new byte[24];
        mCrypt = new PacketCrypt(nullA);
        Global.showNotifyBox("Connected.", "Cancel");
        HandlerDefinitions.InitializePacketHandler();
        Ping.Elapsed += new ElapsedEventHandler(pingLoop);
        Ping.Interval = 15000;
        Ping.Enabled = true;
        Ping_Seq = 1;
        Latency = 1;
    }

    public void pingLoop(object source, ElapsedEventArgs e)
    {
        Ping_Req_Time = MM_GetTime();

        PacketWriter ping = new PacketWriter(WorldServerOpCode.CMSG_PING);
        ping.Write(Ping_Seq);
        ping.Write(Latency);
        Send(ping);
    }

    public void Loop()
    {
        if (mSocket.Connected)
        {

            while (mSocket.Available > 0)
            {
                Thread.Sleep(100);

                DataBuffer = new byte[mSocket.Available];
                mSocket.Receive(DataBuffer, DataBuffer.Length, SocketFlags.None);

                OnData();
            }
        }
        else
        {
            //Exchange.Disconnect();
        }
    }

    public void OnData()
    {
        try
        {
            for (int index = 0; index < DataBuffer.Length; index++)
            {
                byte[] headerData = new byte[4];
                Array.Copy(DataBuffer, index, headerData, 0, 4);
                this.Decode(headerData);
                Array.Copy(headerData, 0, DataBuffer, index, 4);

                ushort opcode = BitConverter.ToUInt16(headerData, 2);
                int length = BitConverter.ToInt16(headerData, 0);

                WorldServerOpCode code = (WorldServerOpCode)opcode;

                byte[] packetData = new byte[length + 2];

                Array.Copy(DataBuffer, index, packetData, 0, length + 2);

                Debug.LogWarning("<---GOTCHA---<< [" + code + "] Packet Length: " + length);

                if (Enum.IsDefined(typeof(WorldServerOpCode), code))
                {
                    PacketReader pkt = null;
                    PacketQueue.Enqueue(new PacketReader(packetData));

                    if (PacketQueue.Count > 0)
                    {
                        pkt = (PacketReader)PacketQueue.Dequeue();
                        pkt.Opcode = code;
                        pkt.Size = (ushort)length;
                    }
                    else
                    {
                        pkt = new PacketReader(packetData);
                        pkt.Opcode = code;
                        pkt.Size = (ushort)length;
                    }

                    PacketManager.InvokeHandler(pkt, this, code);
                }
                else
                {
                    Debug.LogWarning("UNKNOWN OPCODE");
                }

                index += 2 + (length - 1);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.ToString() + " " + e.InnerException);
        }

    }

    public void Decode(byte[] header)
    {
        if (mCrypt._initialized == true)
        {
            mCrypt.decrypt(header, 4);
        }

        ushort length;
        short opcode;

        if (mCrypt._initialized == false)
        {
            length = BitConverter.ToUInt16(new byte[] { header[1], header[0] }, 0);
            opcode = BitConverter.ToInt16(header, 2);
        }
        else
        {
            length = BitConverter.ToUInt16(new byte[] { header[1], header[0] }, 0);
            opcode = BitConverter.ToInt16(new byte[] { header[2], header[3] }, 0);
        }

        header[0] = BitConverter.GetBytes(length)[0];
        header[1] = BitConverter.GetBytes(length)[1];

        header[2] = BitConverter.GetBytes(opcode)[0];
        header[3] = BitConverter.GetBytes(opcode)[1];

    }

    public void Send(PacketWriter packet)
    {
        try
        {
            BinaryWriter StarPacket = new BinaryWriter(new MemoryStream());
            BinaryWriter endPacket = new BinaryWriter(new MemoryStream());

            StarPacket.Write((uint)packet.Opcode);

            Byte[] Data = (StarPacket.BaseStream as MemoryStream).ToArray();

            int Length = Data.Length + packet.Length;
            byte[] Packet = new byte[2 + Data.Length];
            Packet[0] = (byte)(Length >> 8);
            Packet[1] = (byte)(Length & 0xff);
            Data.CopyTo(Packet, 2);

            this.mCrypt.encrypt(Packet);

            endPacket.Write(Packet.ToArray());
            endPacket.Write(packet.PacketData);

            byte[] data = (endPacket.BaseStream as MemoryStream).ToArray();
            Debug.LogWarning(">>---BYE--->: [" + packet.Opcode + "] Packet Length: " + packet.Length);

            mSocket.Send(data);
        }
        catch (Exception ex)
        {
            //Exchange.Disconnect();
        }
    }
}
    