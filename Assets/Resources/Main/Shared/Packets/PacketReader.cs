using Assets.Script.Shared.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public class PacketReader : BinaryReader
{
    public WorldServerOpCode Opcode { get; set; }
    public LogonServerOpCode LogonOpcode { get; set; }
    public ushort Size { get; set; }
    public byte[] packetData;

    public PacketReader(byte[] data) : base(new MemoryStream(data))
    {
        this.ReadUInt16();
        this.ReadUInt16();
        packetData = data;

    }


    public PacketReader(byte[] data, int i)
        : base(new MemoryStream(data))
    {
        packetData = data;
    }

    public PacketReader(byte[] data, bool logon)
        : base(new MemoryStream(data))
    {
        packetData = data;
        LogonOpcode = (LogonServerOpCode)ReadByte();
    }

    public override string ReadString()
    {
        StringBuilder sb = new StringBuilder();
        while (true)
        {
            byte b;
            //if (Remaining > 0)
            b = ReadByte();
            //else
            //   b = 0;

            if (b == 0) break;
            sb.Append((char)b);
        }
        return sb.ToString();
    }

    public byte[] ReadRemaining()
    {
        MemoryStream ms = (MemoryStream)BaseStream;
        int Remaining = (int)(ms.Length - ms.Position);
        return ReadBytes(Remaining);
    }

    public int Remaining
    {
        get
        {
            MemoryStream ms = (MemoryStream)BaseStream;
            return (int)(ms.Length - ms.Position);
        }
        set
        {
            MemoryStream ms = (MemoryStream)BaseStream;
            if (value <= (ms.Length - ms.Position))
                ms.Position = value;
        }
    }
    public float ReadFloat()
    {
        return System.BitConverter.ToSingle(ReadBytes(4), 0);
    }


    public byte[] ToArray()
    {
        return ((MemoryStream)BaseStream).ToArray();
    }
}