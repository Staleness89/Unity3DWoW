using System.IO;

    /// <summary>
    ///     Called M2Batch in the WoW source
    /// </summary>
    public class M2Batch : IMarshalable
    {
        public byte Flags { get; set; } = 0x10;
        public byte Flags2 { get; set; }
        public short ShaderId { get; set; }
        public ushort SubmeshIndex { get; set; }
        public ushort SubmeshIndex2 { get; set; }
        public short ColorIndex { get; set; } = -1;
        public ushort RenderFlags { get; set; }
        public ushort Layer { get; set; }
        public ushort OpCount { get; set; } = 1;
        public ushort Texture { get; set; }
        public ushort TexUnitNumber2 { get; set; }
        public ushort Transparency { get; set; }
        public ushort TextureAnim { get; set; }

        public void Load(BinaryReader stream, M2.Format version = M2.Format.Useless)
        {
            Flags = stream.ReadByte();
            Flags2 = stream.ReadByte();
            ShaderId = stream.ReadInt16();
            SubmeshIndex = stream.ReadUInt16();
            SubmeshIndex2 = stream.ReadUInt16();
            ColorIndex = stream.ReadInt16();
            RenderFlags = stream.ReadUInt16();
            Layer = stream.ReadUInt16();
            OpCount = stream.ReadUInt16();
            Texture = stream.ReadUInt16();
            TexUnitNumber2 = stream.ReadUInt16();
            Transparency = stream.ReadUInt16();
            TextureAnim = stream.ReadUInt16();
        }

        public void Save(BinaryWriter stream, M2.Format version = M2.Format.Useless)
        {
            if (version < M2.Format.Cataclysm) //@author Morfium
            {
                if (!(Flags == 0 || Flags == 16))
                {
                    OpCount = 1;
                    Flags = 16;
                }
                if (ShaderId > 50) ShaderId = 1;
            }
            stream.Write(Flags);
            stream.Write(Flags2);
            stream.Write(ShaderId);
            stream.Write(SubmeshIndex);
            stream.Write(SubmeshIndex2);
            stream.Write(ColorIndex);
            stream.Write(RenderFlags);
            stream.Write(Layer);
            stream.Write(OpCount);
            stream.Write(Texture);
            stream.Write(TexUnitNumber2);
            stream.Write(Transparency);
            stream.Write(TextureAnim);
        }
    }
