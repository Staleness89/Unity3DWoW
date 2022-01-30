using System.IO;

    public class M2ShadowBatch : IMarshalable
    {
        private ushort _unknown1;
        public byte Flags { get; set; }
        public byte Flags2 { get; set; }
        public ushort SubmeshId { get; set; }
        public ushort TextureId { get; set; }
        public ushort ColorId { get; set; }
        public ushort TransparencyId { get; set; }

        public void Load(BinaryReader stream, M2.Format version = M2.Format.Useless)
        {
            Flags = stream.ReadByte();
            Flags2 = stream.ReadByte();
            _unknown1 = stream.ReadUInt16();
            SubmeshId = stream.ReadUInt16();
            TextureId = stream.ReadUInt16();
            ColorId = stream.ReadUInt16();
            TransparencyId = stream.ReadUInt16();
        }

        public void Save(BinaryWriter stream, M2.Format version = M2.Format.Useless)
        {
            stream.Write(Flags);
            stream.Write(Flags2);
            stream.Write(_unknown1);
            stream.Write(SubmeshId);
            stream.Write(TextureId);
            stream.Write(ColorId);
            stream.Write(TransparencyId);
        }
    }
