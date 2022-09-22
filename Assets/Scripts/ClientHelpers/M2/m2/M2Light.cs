using System.Collections.Generic;
using System.IO;

    public class M2Light : IAnimated
    {
        public enum LightType : ushort
        {
            Directional = 0,
            Point = 1
        }

        public LightType Type { get; set; } = LightType.Directional;
        public short Bone { get; set; } = -1;
        public C3Vector Position { get; set; }
        public M2Track<C3Vector> AmbientColor { get; set; } = new M2Track<C3Vector>();
        public M2Track<float> AmbientIntensity { get; set; } = new M2Track<float>();
        public M2Track<C3Vector> DiffuseColor { get; set; } = new M2Track<C3Vector>();
        public M2Track<float> DiffuseIntensity { get; set; } = new M2Track<float>();
        public M2Track<float> AttenuationStart { get; set; } = new M2Track<float>();
        public M2Track<float> AttenuationEnd { get; set; } = new M2Track<float>();
        public M2Track<byte> Unknown { get; set; } = new M2Track<byte>();

        public void Load(BinaryReader stream, M2.Format version)
        {
            Type = (LightType) stream.ReadUInt16();
            Bone = stream.ReadInt16();
            Position = stream.ReadC3Vector();
            AmbientColor.Load(stream, version);
            AmbientIntensity.Load(stream, version);
            DiffuseColor.Load(stream, version);
            DiffuseIntensity.Load(stream, version);
            AttenuationStart.Load(stream, version);
            AttenuationEnd.Load(stream, version);
            Unknown.Load(stream, version);
        }

        public void Save(BinaryWriter stream, M2.Format version)
        {
            stream.Write((ushort) Type);
            stream.Write(Bone);
            stream.Write(Position);
            AmbientColor.Save(stream, version);
            AmbientIntensity.Save(stream, version);
            DiffuseColor.Save(stream, version);
            DiffuseIntensity.Save(stream, version);
            AttenuationStart.Save(stream, version);
            AttenuationEnd.Save(stream, version);
            Unknown.Save(stream, version);
        }

        public void LoadContent(BinaryReader stream, M2.Format version)
        {
            AmbientColor.LoadContent(stream, version);
            AmbientIntensity.LoadContent(stream, version);
            DiffuseColor.LoadContent(stream, version);
            DiffuseIntensity.LoadContent(stream, version);
            AttenuationStart.LoadContent(stream, version);
            AttenuationEnd.LoadContent(stream, version);
            Unknown.LoadContent(stream, version);
        }

        public void SaveContent(BinaryWriter stream, M2.Format version)
        {
            AmbientColor.SaveContent(stream, version);
            AmbientIntensity.SaveContent(stream, version);
            DiffuseColor.SaveContent(stream, version);
            DiffuseIntensity.SaveContent(stream, version);
            AttenuationStart.SaveContent(stream, version);
            AttenuationEnd.SaveContent(stream, version);
            Unknown.SaveContent(stream, version);
        }

        public void SetSequences(IReadOnlyList<M2Sequence> sequences)
        {
            AmbientColor.Sequences = sequences;
            AmbientIntensity.Sequences = sequences;
            DiffuseColor.Sequences = sequences;
            DiffuseIntensity.Sequences = sequences;
            AttenuationStart.Sequences = sequences;
            AttenuationEnd.Sequences = sequences;
            Unknown.Sequences = sequences;
        }
    }
