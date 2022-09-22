using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

public class PacketWriter : BinaryWriter
{
    public WorldOpcode Opcode { get; set; }
    public int Length { get { return (int)BaseStream.Length; } }
    public short Opcode2;

    public PacketWriter(WorldOpcode opcode)
        : base(new MemoryStream())
    {
        Opcode = opcode;
        Opcode2 = (short)opcode;
        //this.Write((uint)opcode);
    }

    public PacketWriter()
        : base(new MemoryStream())
    {

    }

    public PacketWriter(AuthOpcode opcode)
        : base(new MemoryStream())
    {
        this.Write((byte)opcode);
    }

    public override void Write(string Text)
    {
        if (Text != null) Write(Encoding.Default.GetBytes(Text));
        Write((byte)0); // String terminator
    }

    public byte[] PacketData
    {
        get
        {
            return (this.BaseStream as MemoryStream).ToArray();
        }
    }

    public byte[] ToArray()
    {
        return ((MemoryStream)BaseStream).ToArray();
    }

    public long Lenght()
    {
        return base.BaseStream.Length;
    }

    public static implicit operator byte[] (PacketWriter packet)
    {
        return packet.ToArray();
    }

}