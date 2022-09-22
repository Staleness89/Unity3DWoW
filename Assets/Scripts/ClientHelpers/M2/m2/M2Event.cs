using System.Collections.Generic;
using System.IO;
using System.Text;

    public class M2Event : IAnimated
    {
        public string Identifier { get; set; }

        public override string ToString()
        {
            return $"Identifier: {Identifier}, Data: {Data}, Bone: {Bone}, Position: {Position}";
        }

        public int Data { get; set; }
        public ushort Bone { get; set; }
        public ushort Unknown { get; set; }//Only use in early vanilla models. See BogBeast.m2 from model.MPQ
        public C3Vector Position { get; set; }
        public M2TrackBase Enabled { get; set; } = new M2TrackBase();

        public void Load(BinaryReader stream, M2.Format version)
        {
            Identifier = Encoding.UTF8.GetString(stream.ReadBytes(4));
            Data = stream.ReadInt32();
            Bone = stream.ReadUInt16();
            Unknown = stream.ReadUInt16();
            Position = stream.ReadC3Vector();
            Enabled.Load(stream, version);
        }

        public void Save(BinaryWriter stream, M2.Format version)
        {
            if (Identifier.Length > 4)
                Identifier = Identifier.Substring(0, 4);
            stream.Write(Encoding.UTF8.GetBytes(Identifier));
            stream.Write(Data);
            stream.Write(Bone);
            stream.Write(Unknown);
            stream.Write(Position);
            Enabled.Save(stream, version);
        }

        public void LoadContent(BinaryReader stream, M2.Format version)
        {
            Enabled.LoadContent(stream, version);
        }

        public void SaveContent(BinaryWriter stream, M2.Format version)
        {
            Enabled.SaveContent(stream, version);
        }

        public void SetSequences(IReadOnlyList<M2Sequence> sequences)
        {
            Enabled.Sequences = sequences;
        }
    }
