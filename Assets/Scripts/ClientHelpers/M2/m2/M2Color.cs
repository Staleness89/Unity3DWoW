using System.Collections.Generic;
using System.IO;

    public class M2Color : IAnimated
    {
        public M2Track<C3Vector> Color { get; set; } = new M2Track<C3Vector>();
        public M2Track<FixedPoint_0_15> Alpha { get; set; } = new M2Track<FixedPoint_0_15>(new FixedPoint_0_15(0x7FFF));

        public void Load(BinaryReader stream, M2.Format version)
        {
            Color.Load(stream, version);
            Alpha.Load(stream, version);
        }

        public void Save(BinaryWriter stream, M2.Format version)
        {
            Color.Save(stream, version);
            Alpha.Save(stream, version);
        }

        public void LoadContent(BinaryReader stream, M2.Format version)
        {
            Color.LoadContent(stream, version);
            Alpha.LoadContent(stream, version);
        }

        public void SaveContent(BinaryWriter stream, M2.Format version)
        {
            Color.SaveContent(stream, version);
            Alpha.SaveContent(stream, version);
        }

        public void SetSequences(IReadOnlyList<M2Sequence> sequences)
        {
            Color.Sequences = sequences;
            Alpha.Sequences = sequences;
        }
    }
