using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

    public class M2Track<T> : IReferencer where T : new()
    {
        public enum InterpolationTypes : ushort
        {
            Instant = 0,
            Linear = 1,
            Hermite = 2,
            Bezier = 3
        }

    public readonly T _defaultValue;
    public readonly bool _defaultValueSet;

        private M2Array<Range> _legacyRanges;
        private M2Array<uint> _legacyTimestamps;
        private M2Array<T> _legacyValues;
        public bool IsGlobalSequence => GlobalSequence >= 0;

        public M2Track()
        {
        }

        public M2Track(T defaultValue) : this()
        {
            _defaultValue = defaultValue;
            _defaultValueSet = true;
        }

        public InterpolationTypes InterpolationType { get; set; }
        public short GlobalSequence { get; set; } = -1;
        public M2Array<M2Array<uint>> Timestamps { get; } = new M2Array<M2Array<uint>>();

        public M2Array<M2Array<T>> Values { get; } = new M2Array<M2Array<T>>();

        // Used only to read 1 timeline formats and to open correct .anim files when needed.
        // Legacy fields are automatically converted to standard ones in methods.
        public IReadOnlyList<M2Sequence> Sequences { set; get; }

        public void Load(BinaryReader stream, M2.Format version)
        {
            Debug.Assert(version != M2.Format.Useless);
            InterpolationType = (InterpolationTypes) stream.ReadUInt16();
            GlobalSequence = stream.ReadInt16();
            if (version >= M2.Format.LichKing)
            {
                Timestamps.Load(stream, version);
                Values.Load(stream, version);
            }
            else
            {
                _legacyRanges = new M2Array<Range>();
                _legacyTimestamps = new M2Array<uint>();
                _legacyValues = new M2Array<T>();
                LegacyLoad(stream, version);
            }
        }

        public void Save(BinaryWriter stream, M2.Format version)
        {
            Debug.Assert(version != M2.Format.Useless);
            stream.Write((ushort) InterpolationType);
            stream.Write(GlobalSequence);
            if (version >= M2.Format.LichKing)
            {
                Timestamps.Save(stream, version);
                Values.Save(stream, version);
            }
            else
            {
                _legacyRanges = new M2Array<Range>();
                _legacyTimestamps = new M2Array<uint>();
                _legacyValues = new M2Array<T>();
                LegacySave(stream, version);
            }
        }

        public void LoadContent(BinaryReader stream, M2.Format version)
        {
            Debug.Assert(version != M2.Format.Useless);
            if (version >= M2.Format.LichKing)
            {
                Timestamps.LoadContent(stream, version);
                Values.LoadContent(stream, version);
                for (var i = 0; i < Timestamps.Count; i++)
                {
                    //TODO Should we check if GlobalSequence before accessing sequence flags ?
                    if (Sequences[i].IsAlias)
                    {
                        var realIndex = i;
                        while (Sequences[realIndex].IsAlias)
                            realIndex = Sequences[realIndex].AliasNext;
                        Timestamps[i] = Timestamps[realIndex];
                        Values[i] = Values[realIndex];
                        continue;
                    }
                    if (Sequences[i].IsExtern)
                    {
                        Timestamps[i].LoadContent(Sequences[i].ReadingAnimFile, version);
                        Values[i].LoadContent(Sequences[i].ReadingAnimFile, version);
                    }
                    else
                    {
                        Timestamps[i].LoadContent(stream, version);
                        Values[i].LoadContent(stream, version);
                    }
                }
            }
            else
            {
                LegacyLoadContent(stream, version);
                _legacyRanges = null;
                _legacyTimestamps = null;
                _legacyValues = null;
            }
        }

        public void SaveContent(BinaryWriter stream, M2.Format version)
        {
            Debug.Assert(version != M2.Format.Useless);
            if (version >= M2.Format.LichKing)
            {
                Timestamps.SaveContent(stream, version);
                Values.SaveContent(stream, version);
                for (var i = 0; i < Timestamps.Count; i++)
                {
                    //TODO Should we check if GlobalSequence before accessing sequence flags ?
                    if (Sequences[i].IsAlias)
                    {
                        var realIndex = i;
                        while (Sequences[realIndex].IsAlias)
                            realIndex = Sequences[realIndex].AliasNext;
                        Timestamps[i] = Timestamps[realIndex];
                        Values[i] = Values[realIndex];
                        continue;
                    }
                    if (Sequences[i].IsExtern)
                    {
                        //Cannot use SaveContent() as it Rewrites header only in the same stream
                        if (Timestamps[i].Count <= 0) continue;
                        Timestamps[i].StoredOffset = (uint) Sequences[i].WritingAnimFile.BaseStream.Position;
                        for (var j = 0; j < Timestamps[i].Count; j++)
                            Sequences[i].WritingAnimFile.Write(Timestamps[i][j]);
                        Timestamps[i].RewriteHeader(stream, version);
                        Values[i].StoredOffset = (uint) Sequences[i].WritingAnimFile.BaseStream.Position;
                        for (var j = 0; j < Values[i].Count; j++)
                            Sequences[i].WritingAnimFile.WriteGeneric(version, Values[i][j]);
                        Values[i].RewriteHeader(stream, version);
                    }
                    else
                    {
                        Timestamps[i].SaveContent(stream, version);
                        Values[i].SaveContent(stream, version);
                    }
                }
            }
            else
            {
                LegacySaveContent(stream, version);
                _legacyRanges = null;
                _legacyTimestamps = null;
                _legacyValues = null;
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("Interpolation type : " + InterpolationType+"\n");
            builder.Append("GlobalSequence Index : " + GlobalSequence+"\n");
            if (Timestamps.Count == 0)
            {
                builder.Append("<No keyframe>\n");
                return builder.ToString();
            }
            builder.Append("\tTime\tValue\n");
            for (var i = 0; i < Timestamps.Count; i++)
            {
                builder.Append("[" + i + "]\n");
                for (var j = 0; j < Timestamps[i].Count; j++)
                {
                    builder.Append("\t" + Timestamps[i][j] + "\t" + Values[i][j]+"\n");
                }
                builder.AppendLine();
            }
            return builder.ToString();
        }

        /// <summary>
        ///     Pre : Sequences != null
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="version"></param>
        private void LegacySave(BinaryWriter stream, M2.Format version)
        {
            if (Timestamps.Count == 0)
            {
                /*TODO*/
            }
            else if (IsGlobalSequence)
            {
                _legacyTimestamps.AddRange(Timestamps[0]);
                _legacyValues.AddRange(Values[0]);
            }
            else if (Timestamps.Count == Sequences.Count)
            {
                for (var i = 0; i < Timestamps.Count; i++)
                {
                    if (Timestamps[i].Count == 1)// Constant for animation i
                    {
                        _legacyTimestamps.Add(Timestamps[i][0] + Sequences[i].TimeStart);
                        _legacyValues.Add(Values[i][0]);
                        _legacyTimestamps.Add(Timestamps[i][0] + Sequences[i].TimeEnd);
                        _legacyValues.Add(Values[i][0]);
                    }
                    else if (Timestamps[i].Count > 1)// Standard case
                    {
                        for (var j = 0; j < Timestamps[i].Count; j++)
                        {
                            _legacyTimestamps.Add(Timestamps[i][j] + Sequences[i].TimeStart);
                            _legacyValues.Add(Values[i][j]);
                        }
                    }
                    else // No value for animation i
                    {
                        //Purpose : Not adding (1,1,1) as default scaling value, f.e., would lead older clients to believe it's (0,0,0). 
                        //It would result in disappearing parts of the model. Try with Jaina.m2 to make her jawless.
                        _legacyTimestamps.Add(Sequences[i].TimeStart);
                        _legacyValues.Add(_defaultValueSet ? _defaultValue : new T());
                        _legacyTimestamps.Add(Sequences[i].TimeEnd);
                        _legacyValues.Add(_defaultValueSet ? _defaultValue : new T());
                    }
                }
            }
            else if (Timestamps.Count == 1)
            {
                _legacyTimestamps.AddRange(Timestamps[0]);
                _legacyValues.AddRange(Values[0]);
            }
            OldGenerateLegacyRanges();
            _legacyRanges.Save(stream, version);
            _legacyTimestamps.Save(stream, version);
            _legacyValues.Save(stream, version);
        }

        private void LegacySaveContent(BinaryWriter stream, M2.Format version)
        {
            _legacyRanges.SaveContent(stream, version);
            _legacyTimestamps.SaveContent(stream, version);
            _legacyValues.SaveContent(stream, version);
        }

        /// <summary>
        ///     Pre : Sequences set with TimeStart, Sequences set, LegacyTimestamps computed
        /// </summary>
        private void GenerateLegacyRanges()
        {
            if (_legacyTimestamps.Count < 2) return;
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < Sequences.Count; index++)
            {
                var seq = Sequences[index];
                var indexesPrevious =
                    Enumerable.Range(0, _legacyTimestamps.Count) // Indexes of times <= to the beginning of sequence.
                        .Where(i => _legacyTimestamps[i] <= seq.TimeStart)
                        .ToList();
                var indexesNext =
                    Enumerable.Range(0, _legacyTimestamps.Count) // Indexes of times >= to the end of sequence.
                        .Where(i => _legacyTimestamps[i] >= seq.TimeEnd)
                        .ToList();

                uint startIndex;
                uint endIndex;
                if (indexesPrevious.Count == 0) startIndex = 0;
                else startIndex = (uint) indexesPrevious[indexesPrevious.Count - 1]; // Maximum

                if (indexesNext.Count == 0)
                    endIndex = (uint) (_legacyTimestamps.Count - 1);
                // We know there more than 1 element (see line 1) so it's >= 0
                else endIndex = (uint) indexesNext[0]; // Minimum

                _legacyRanges.Add(new Range(startIndex, endIndex));
            }
            _legacyRanges.Add(new Range());
        }

        private void LegacyLoad(BinaryReader stream, M2.Format version)
        {
            Debug.Assert(Sequences != null, "Sequences is null in M2Track<" + typeof (T) + ">");
            _legacyRanges.Load(stream, version);
            _legacyTimestamps.Load(stream, version);
            _legacyValues.Load(stream, version);
        }

        private void LegacyLoadContent(BinaryReader stream, M2.Format version)
        {
            _legacyRanges.LoadContent(stream, version);
            _legacyTimestamps.LoadContent(stream, version);
            _legacyValues.LoadContent(stream, version);
            if (_legacyTimestamps.Count == 0) return;
            if (GlobalSequence >= 0)
            {
                Timestamps.Add(_legacyTimestamps);
                Values.Add(_legacyValues);
            }
            else 
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var index = 0; index < Sequences.Count; index++)
                {
                    var animTimes = new M2Array<uint>();
                    var animValues = new M2Array<T>();
                    var seq = Sequences[index];
                    var validIndexes = Enumerable.Range(0, _legacyTimestamps.Count)
                        .Where(
                            i =>
                                _legacyTimestamps[i] >= seq.TimeStart &&
                                _legacyTimestamps[i] <= seq.TimeEnd)
                        .ToList();
                    if (validIndexes.Count > 0)
                    {
                        var firstIndex = validIndexes[0];
                        var lastIndex = validIndexes[validIndexes.Count - 1];
                        animTimes.AddRange(_legacyTimestamps.GetRange(firstIndex, lastIndex - firstIndex + 1));
                        animValues.AddRange(_legacyValues.GetRange(firstIndex, lastIndex - firstIndex + 1));
                    }
                    Timestamps.Add(animTimes);
                    Values.Add(animValues);
                }
            }
        }

        /// <summary>
        /// This version dates back to the Java and even to the C converters.
        /// </summary>
        private void OldGenerateLegacyRanges()
        {
            if (IsGlobalSequence || Timestamps.Count == 0 || Timestamps.Count != Sequences.Count)
            {
                return;
            }
            Debug.Assert(Timestamps.Count == Sequences.Count);
            uint rangeTime = 0;
            foreach(var times in Timestamps)
            {
                var x = rangeTime;
                if (times.Count == 1)
                {
                    rangeTime++;
                }
                else if (times.Count > 1)
                {
                    rangeTime += (uint) times.Count - 1;
                }
                else
                {
                    rangeTime++;
                }
                var y = rangeTime;
                _legacyRanges.Add(new Range(x, y));
                rangeTime++;
            }
            _legacyRanges.Add(new Range());
        }
    }

    public class Range : IMarshalable
    {
        public Range(uint p1, uint p2)
        {
            StartIndex = p1;
            EndIndex = p2;
        }

        public Range() : this(0, 0)
        {
        }

        public uint StartIndex { get; set; }
        public uint EndIndex { get; set; }

        public void Load(BinaryReader stream, M2.Format version)
        {
            StartIndex = stream.ReadUInt32();
            EndIndex = stream.ReadUInt32();
        }

        public void Save(BinaryWriter stream, M2.Format version)
        {
            stream.Write(StartIndex);
            stream.Write(EndIndex);
        }
    }

    public static class M2TrackExtensions
    {
        public static void Compress(this M2Track<C4Quaternion> track, M2Track<M2CompQuat> target)
        {
            target.Timestamps.Clear();
            target.Values.Clear();
            target.InterpolationType = (M2Track<M2CompQuat>.InterpolationTypes) track.InterpolationType;
            target.GlobalSequence = track.GlobalSequence;
            target.Sequences = track.Sequences;
            for (var i = 0; i < track.Timestamps.Count; i++) target.Timestamps.Add(track.Timestamps[i]);
            for (var i = 0; i < track.Values.Count; i++)
            {
                var newArray = new M2Array<M2CompQuat>();
                newArray.AddRange(track.Values[i].Select(value => (M2CompQuat) value));
                target.Values.Add(newArray);
            }
        }

        public static void Decompress(this M2Track<M2CompQuat> track, M2Track<C4Quaternion> target)
        {
            target.Timestamps.Clear();
            target.Values.Clear();
            target.InterpolationType = (M2Track<C4Quaternion>.InterpolationTypes) track.InterpolationType;
            target.GlobalSequence = track.GlobalSequence;
            target.Sequences = track.Sequences;
            for (var i = 0; i < track.Timestamps.Count; i++) target.Timestamps.Add(track.Timestamps[i]);
            for (var i = 0; i < track.Values.Count; i++)
            {
                var newArray = new M2Array<C4Quaternion>();
                newArray.AddRange(track.Values[i].Select(value => (C4Quaternion) value));
                target.Values.Add(newArray);
            }
        }
    }
