using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

public class PacketReader : BinaryReader
{
    public WorldOpcode Opcode { get; set; }
    public AuthOpcode LogonOpcode { get; set; }
    public ushort Size { get; set; }
    public byte[] packetData;

    public PacketReader(byte[] data) : base(new MemoryStream(data))
    {
        this.ReadUInt16();
        this.ReadUInt16();
        packetData = data;

    }


    public PacketReader(byte[] data, WorldOpcode _Opcode)
        : base(new MemoryStream(data))
    {
        Opcode = _Opcode;
        packetData = data;
    }

    public PacketReader(byte[] data, bool logon)
        : base(new MemoryStream(data))
    {
        packetData = data;
        LogonOpcode = (AuthOpcode)ReadByte();
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
    public ulong ReadPackedGuid()
    {
        var mask = ReadByte();

            if (mask == 0)
                return (ulong)0;

            ulong res = 0;

            var i = 0;
            while (i < 8)
            {
                if ((mask & 1 << i) != 0)
                    res += (ulong)ReadByte() << (i * 8);

                i++;
            }

            return res;
    }
    public static byte[] Decompress(int Length, byte[] Data)
    {
        byte[] Output = new byte[Length];
        Stream s = new InflaterInputStream(new MemoryStream(Data));
        int Offset = 0;
        while (true)
        {
            int size = s.Read(Output, Offset, Length);
            if (size == Length) break;
            Offset += size;
            Length -= size;
        }
        return Output;
    }
    public string ReadCString()
    {
        StringBuilder builder = new StringBuilder();

        while (true)
        {
            byte letter = ReadByte();
            if (letter == 0)
                break;

            builder.Append((char)letter);
        }

        return builder.ToString();
    }

    public float ReadFloat() => ReadSingle();


    public byte[] ToArray()
    {
        return ((MemoryStream)BaseStream).ToArray();
    }
}