using System.IO;
using System.Text;

    public class M2FakeTrack<T> : IReferencer where T: new()
    {
        public readonly M2Array<short> Timestamps = new M2Array<short>(); 
        public readonly M2Array<T> Values = new M2Array<T>(); 
        public void Load(BinaryReader stream, M2.Format version)
        {
            Timestamps.Load(stream, version);
            Values.Load(stream, version);
        }

        public void Save(BinaryWriter stream, M2.Format version)
        {
            Timestamps.Save(stream, version);
            Values.Save(stream, version);
        }

        public void LoadContent(BinaryReader stream, M2.Format version)
        {
            Timestamps.LoadContent(stream, version);
            Values.LoadContent(stream, version);
        }

        public void SaveContent(BinaryWriter stream, M2.Format version)
        {
            Timestamps.SaveContent(stream, version);
            Values.SaveContent(stream, version);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("Time\tValue\n");
            for (var i = 0; i < Timestamps.Count; i++)
                builder.Append(Timestamps[i] + "\t" + Values[i] + "\n");
            return builder.ToString();
        }

    }

