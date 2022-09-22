using System.Collections.Generic;
using System.IO;

    public class M2TextureWeight : IAnimated
    {
        public M2Track<FixedPoint_0_15> Weight { get; set; } = new M2Track<FixedPoint_0_15>(new FixedPoint_0_15(0x7FFF));

        public void Load(BinaryReader stream, M2.Format version)
        {
            Weight.Load(stream, version);
        }

        public void Save(BinaryWriter stream, M2.Format version)
        {
            Weight.Save(stream, version);
        }

        public void LoadContent(BinaryReader stream, M2.Format version)
        {
            Weight.LoadContent(stream, version);
        }

        public void SaveContent(BinaryWriter stream, M2.Format version)
        {
            Weight.SaveContent(stream, version);
        }

        public void SetSequences(IReadOnlyList<M2Sequence> sequences)
        {
            Weight.Sequences = sequences;
        }
    }
