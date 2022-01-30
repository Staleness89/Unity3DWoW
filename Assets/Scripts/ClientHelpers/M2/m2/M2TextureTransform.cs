using System.Collections.Generic;
using System.IO;

    public class M2TextureTransform : IAnimated
    {
        public M2Track<C3Vector> Translation { get; set; } = new M2Track<C3Vector>();
        public M2Track<C4Quaternion> Rotation { get; set; } = new M2Track<C4Quaternion>(new C4Quaternion(0, 0, 0, 1));
        public M2Track<C3Vector> Scale { get; set; } = new M2Track<C3Vector>(new C3Vector(1, 1, 1));

        public void Load(BinaryReader stream, M2.Format version)
        {
            Translation.Load(stream, version);
            Rotation.Load(stream, version);
            Scale.Load(stream, version);
        }

        public void Save(BinaryWriter stream, M2.Format version)
        {
            Translation.Save(stream, version);
            Rotation.Save(stream, version);
            Scale.Save(stream, version);
        }

        public void LoadContent(BinaryReader stream, M2.Format version)
        {
            Translation.LoadContent(stream, version);
            Rotation.LoadContent(stream, version);
            Scale.LoadContent(stream, version);
        }

        public void SaveContent(BinaryWriter stream, M2.Format version)
        {
            Translation.SaveContent(stream, version);
            Rotation.SaveContent(stream, version);
            Scale.SaveContent(stream, version);
        }

        /// <summary>
        ///     Pass the sequences reference to Tracks so they can : switch between 1 timeline & multiple timelines, open .anim
        ///     files...
        /// </summary>
        /// <param name="sequences"></param>
        public void SetSequences(IReadOnlyList<M2Sequence> sequences)
        {
            Translation.Sequences = sequences;
            Rotation.Sequences = sequences;
            Scale.Sequences = sequences;
        }
    }
