using System.Collections.Generic;
using System.IO;

    public class M2Ribbon : IAnimated
    {
        public int Unknown1 { get; set; } = -1;
        public uint Bone { get; set; }
        public C3Vector Position { get; set; }
        public M2Array<ushort> TextureRefs { get; set; } = new M2Array<ushort>();
        public M2Array<ushort> BlendRefs { get; set; } = new M2Array<ushort>();
        public M2Track<C3Vector> Color { get; set; } = new M2Track<C3Vector>(); //TODO check default values here
        public M2Track<FixedPoint_0_15> Opacity { get; set; } = new M2Track<FixedPoint_0_15>(new FixedPoint_0_15(0x7FFF));
        public M2Track<float> HeightAbove { get; set; } = new M2Track<float>();
        public M2Track<float> HeightBelow { get; set; } = new M2Track<float>();
        public float EdgesPerSec { get; set; }
        public float EdgeLifeSpanInSec { get; set; }
        public float Gravity { get; set; }
        public ushort MRows { get; set; }
        public ushort MCols { get; set; }
        public M2Track<ushort> TexSlot { get; set; } = new M2Track<ushort>();
        public M2Track<bool> DataEnabled { get; set; } = new M2Track<bool>(true);
        public uint Unknown2 { get; set; }

        public void Load(BinaryReader stream, M2.Format version)
        {
            Unknown1 = stream.ReadInt32();
            Bone = stream.ReadUInt32();
            Position = stream.ReadC3Vector();
            TextureRefs.Load(stream, version);
            BlendRefs.Load(stream, version);
            Color.Load(stream, version);
            Opacity.Load(stream, version);
            HeightAbove.Load(stream, version);
            HeightBelow.Load(stream, version);
            EdgesPerSec = stream.ReadSingle();
            EdgeLifeSpanInSec = stream.ReadSingle();
            Gravity = stream.ReadSingle();
            MRows = stream.ReadUInt16();
            MCols = stream.ReadUInt16();
            TexSlot.Load(stream, version);
            DataEnabled.Load(stream, version);
            if (version < M2.Format.LichKing) return;
            Unknown2 = stream.ReadUInt32();
        }

        public void Save(BinaryWriter stream, M2.Format version)
        {
            stream.Write(Unknown1);
            stream.Write(Bone);
            stream.Write(Position);
            TextureRefs.Save(stream, version);
            BlendRefs.Save(stream, version);
            Color.Save(stream, version);
            Opacity.Save(stream, version);
            HeightAbove.Save(stream, version);
            HeightBelow.Save(stream, version);
            stream.Write(EdgesPerSec);
            stream.Write(EdgeLifeSpanInSec);
            stream.Write(Gravity);
            stream.Write(MRows);
            stream.Write(MCols);
            TexSlot.Save(stream, version);
            if (version < M2.Format.LichKing && DataEnabled.Timestamps.Count == 0)
            {
                DataEnabled.Timestamps.Add(new M2Array<uint> {0});
                DataEnabled.Values.Add(new M2Array<bool> {true});
            }
            DataEnabled.Save(stream, version);
            if (version < M2.Format.LichKing) return;
            stream.Write(Unknown2);
        }

        public void LoadContent(BinaryReader stream, M2.Format version)
        {
            TextureRefs.LoadContent(stream, version);
            BlendRefs.LoadContent(stream, version);
            Color.LoadContent(stream, version);
            Opacity.LoadContent(stream, version);
            HeightAbove.LoadContent(stream, version);
            HeightBelow.LoadContent(stream, version);
            TexSlot.LoadContent(stream, version);
            DataEnabled.LoadContent(stream, version);
        }

        public void SaveContent(BinaryWriter stream, M2.Format version)
        {
            TextureRefs.SaveContent(stream, version);
            BlendRefs.SaveContent(stream, version);
            Color.SaveContent(stream, version);
            Opacity.SaveContent(stream, version);
            HeightAbove.SaveContent(stream, version);
            HeightBelow.SaveContent(stream, version);
            TexSlot.SaveContent(stream, version);
            DataEnabled.SaveContent(stream, version);
        }

        public void SetSequences(IReadOnlyList<M2Sequence> sequences)
        {
            Color.Sequences = sequences;
            Opacity.Sequences = sequences;
            HeightAbove.Sequences = sequences;
            HeightBelow.Sequences = sequences;
            TexSlot.Sequences = sequences;
            DataEnabled.Sequences = sequences;
        }
    }
