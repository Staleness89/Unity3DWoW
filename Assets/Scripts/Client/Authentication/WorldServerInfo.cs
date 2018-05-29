using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Client.Authentication
{
    public class WorldServerInfo
    {
        public byte Type { get; private set; }
        private byte locked;
        public byte Flags { get; private set; }
        public uint wOnline { get; private set; }        
        public string Name { get; private set; }
        public string Address { get; private set; }
        public int Port { get; private set; }
        public float Population { get; private set; }
        public byte load;
        private byte timezone;
        private byte version_major;
        private byte version_minor;
        private byte version_bugfix;
        private ushort build;
        public uint ID { get; private set; }
        //private ushort unk2;

        public WorldServerInfo(BinaryReader reader)
        {
            Type = reader.ReadByte();
            locked = reader.ReadByte();
            Flags = reader.ReadByte();
            Name = reader.ReadCString();
            string address = reader.ReadCString();
            string[] tokens = address.Split(':');
            Address = tokens[0];
            Port = tokens.Length > 1 ? int.Parse(tokens[1]) : 8085;
            Population = reader.ReadSingle();
            load = reader.ReadByte();
            timezone = reader.ReadByte();
            ID = reader.ReadByte();

            wOnline = 0;
            

            if (CheckAvailableServerPort(address))
            {
                wOnline = 1;
            }

            if ((Flags & 4) != 0)
            {
                version_major = reader.ReadByte();
                version_minor = reader.ReadByte();
                version_bugfix = reader.ReadByte();
                build = reader.ReadUInt16();
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
                Socket testSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                testSocket.Connect(ep);
                testSocket.Close();

                return true;
            }
            catch (SocketException ex)
            {
                return false;
            }
        }
    }
}
