using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

    public class M2Array<T> : List<T>, IMarshalable where T : new()
    {
        private uint _n; // n&ofs are only used in loading. When writing the real number is used.

        private IReadOnlyList<M2Sequence> _sequencesBackRef; // Only used to give the reference to contained IAnimated.
        private long _startOffset = -1; // Where the n&offset are located
        public uint StoredOffset;

        public void Load(BinaryReader stream, M2.Format version = M2.Format.Useless)
        {
            _n = stream.ReadUInt32();
            StoredOffset = stream.ReadUInt32();
        }

        public void Save(BinaryWriter stream, M2.Format version = M2.Format.Useless)
        {
            _startOffset = stream.BaseStream.Position;
            stream.Write(Count);
            stream.Write(StoredOffset);
        }

        //TODO A bit of optimization would be nice.
        /// <summary>
        ///     Load referenced content. Instances are created, then loaded, then each of their referenced content is loaded.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="version"></param>
        public void LoadContent(BinaryReader stream, M2.Format version = M2.Format.Useless)
        {
            if (_n == 0) return;

            stream.BaseStream.Seek(StoredOffset, SeekOrigin.Begin);
            if (typeof (IAnimated).IsAssignableFrom(typeof (T)))
            {
                for (var i = 0; i < _n; i++)
                {
                    Add(new T());
                    ((IAnimated) this[i]).SetSequences(_sequencesBackRef);
                    ((IMarshalable) this[i]).Load(stream, version);
                }
            }
            else if (typeof (IMarshalable).IsAssignableFrom(typeof (T)))
            {
                for (var i = 0; i < _n; i++)
                {
                    Add(new T());
                    ((IMarshalable) this[i]).Load(stream, version);
                }
            }
            else
            {
                for (var i = 0; i < _n; i++)
                {
                    Debug.Assert(StreamExtensions.ReadFunctions.ContainsKey(typeof (T)), "Can't read " + typeof (T));
                    Add((T) StreamExtensions.ReadFunctions[typeof (T)](stream));
                }
            }
            if (!typeof (IReferencer).IsAssignableFrom(typeof (T))) return;

            for (var i = 0; i < _n; i++)
                ((IReferencer) this[i]).LoadContent(stream, version);
        }

        public void SaveContent(BinaryWriter stream, M2.Format version = M2.Format.Useless)
        {
            if (Count == 0) return;
            StoredOffset = (uint) stream.BaseStream.Position;
            if (typeof (IAnimated).IsAssignableFrom(typeof (T)))
            {
                for (var i = 0; i < Count; i++)
                {
                    ((IAnimated) this[i]).SetSequences(_sequencesBackRef);
                    ((IMarshalable) this[i]).Save(stream, version);
                }
            }
            else if (typeof (IMarshalable).IsAssignableFrom(typeof (T)))
            {
                for (var i = 0; i < Count; i++)
                    ((IMarshalable) this[i]).Save(stream, version);
            }
            else
            {
                for (var i = 0; i < Count; i++)
                {
                    Debug.Assert(StreamExtensions.WriteFunctions.ContainsKey(typeof (T)), "Can't write " + typeof (T));
                    StreamExtensions.WriteFunctions[typeof (T)](stream, this[i]);
                }
            }
            if (typeof (IReferencer).IsAssignableFrom(typeof (T)))
            {
                for (var i = 0; i < Count; i++)
                    ((IReferencer) this[i]).SaveContent(stream, version);
            }
            RewriteHeader(stream, version);
        }

        /// <summary>
        ///     M2Array may need sequences for just one thing : give it to contained M2Track.
        ///     Sometimes these are inside other classes, which are the IAnimated.
        /// </summary>
        /// <param name="sequences"></param>
        public void PassSequences(IReadOnlyList<M2Sequence> sequences)
        {
            Debug.Assert(typeof (IAnimated).IsAssignableFrom(typeof (T)),
                "M2Array<" + typeof (T) + "> while T does not implement IAnimated");
            _sequencesBackRef = sequences;
        }

        public void RewriteHeader(BinaryWriter stream, M2.Format version)
        {
            Debug.Assert(_startOffset > -1, "M2Array not saved before saving referenced content.");
            var currentOffset = (uint) stream.BaseStream.Position;
            stream.BaseStream.Seek(_startOffset, SeekOrigin.Begin);
            Save(stream, version);
            stream.BaseStream.Seek(currentOffset, SeekOrigin.Begin);
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append("[N: " + Count + "]");
            if (Count == 0) return result.ToString();
            result.Append("\r\n");
            for (var i = 0; i < Count; i++)
            {
                result.Append("["+i+"] " + this[i]);
                result.Append("\r\n");
            }
            result.Append("\r\n");
            return result.ToString();
        }
    }

    /// <summary>
    ///     Allows special behaviors for specific M2Arrays.
    /// </summary>
    public static class M2ArrayExtensions
    {
        public static string ToNameString(this M2Array<byte> array)
        {
            return Encoding.UTF8.GetString(array.ToArray()).Trim('\0');
        }

        public static void SetString(this M2Array<byte> array, string str)
        {
            array.Clear();
            array.AddRange(Encoding.UTF8.GetBytes(str + "\0"));
        }
    }
