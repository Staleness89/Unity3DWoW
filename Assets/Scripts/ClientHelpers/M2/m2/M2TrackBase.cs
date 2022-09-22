using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

    public class M2TrackBase : IReferencer
    {
        public enum InterpolationTypes : ushort
        {
            Instant = 0,
            Linear = 1,
            Hermite = 2,
            Bezier = 3
        }

        private M2Array<Range> _legacyRanges;
        private M2Array<uint> _legacyTimestamps;

        public InterpolationTypes InterpolationType { get; set; }
        public short GlobalSequence { get; set; } = -1;
        public M2Array<M2Array<uint>> Timestamps { get; } = new M2Array<M2Array<uint>>();

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
            }
            else
            {
                _legacyRanges = new M2Array<Range>();
                _legacyTimestamps = new M2Array<uint>();
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
            }
            else
            {
                _legacyRanges = new M2Array<Range>();
                _legacyTimestamps = new M2Array<uint>();
                LegacySave(stream, version);
            }
        }

        public void LoadContent(BinaryReader stream, M2.Format version)
        {
            Debug.Assert(version != M2.Format.Useless);
            if (version >= M2.Format.LichKing)
            {
                Timestamps.LoadContent(stream, version);
                for (var i = 0; i < Timestamps.Count; i++)
                {
                    //TODO Should we check if GlobalSequence before accessing sequence flags ?
                    if (Sequences[i].IsAlias)
                    {
                        var realIndex = i;
                        while (Sequences[realIndex].IsAlias)
                            realIndex = Sequences[realIndex].AliasNext;
                        Timestamps[i] = Timestamps[realIndex];
                        continue;
                    }
                    if (Sequences[i].IsExtern)
                    {
                        Timestamps[i].LoadContent(Sequences[i].ReadingAnimFile, version);
                    }
                    else
                    {
                        Timestamps[i].LoadContent(stream, version);
                    }
                }
            }
            else
            {
                LegacyLoadContent(stream, version);
                _legacyRanges = null;
                _legacyTimestamps = null;
            }
        }

        public void SaveContent(BinaryWriter stream, M2.Format version)
        {
            Debug.Assert(version != M2.Format.Useless);
            if (version >= M2.Format.LichKing)
            {
                Timestamps.SaveContent(stream, version);
                for (var i = 0; i < Timestamps.Count; i++)
                {
                    //TODO Should we check if GlobalSequence before accessing sequence flags ?
                    if (Sequences[i].IsAlias)
                    {
                        var realIndex = i;
                        while (Sequences[realIndex].IsAlias)
                            realIndex = Sequences[realIndex].AliasNext;
                        Timestamps[i] = Timestamps[realIndex];
                        continue;
                    }
                    if (Sequences[i].IsExtern)
                    {
                        if (Timestamps[i].Count <= 0) continue;
                        Timestamps[i].StoredOffset = (uint) Sequences[i].WritingAnimFile.BaseStream.Position;
                        for (var j = 0; j < Timestamps[i].Count; j++)
                            Sequences[i].WritingAnimFile.Write(Timestamps[i][j]);
                        Timestamps[i].RewriteHeader(stream, version);
                    }
                    else
                    {
                        Timestamps[i].SaveContent(stream, version);
                    }
                }
            }
            else
            {
                LegacySaveContent(stream, version);
                _legacyRanges = null;
                _legacyTimestamps = null;
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("Interpolation type : " + InterpolationType + "\n");
            builder.Append("GlobalSequence Index : " + GlobalSequence + "\n");
            builder.Append("\tTime\n");
            for (var i = 0; i < Timestamps.Count; i++)
            {
                builder.Append("[" + i + "]\n");
                for (var j = 0; j < Timestamps[i].Count; j++)
                {
                    builder.Append("\t" + Timestamps[i][j] + "\n");
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
            if (GlobalSequence >= 0)
            {
                _legacyTimestamps.AddRange(Timestamps[0]);
            }
            else if (Timestamps.Count == Sequences.Count)
            {
                for (var i = 0; i < Timestamps.Count; i++)
                {
                    if (Timestamps[i].Count == 0) continue;
                    for (var j = 0; j < Timestamps[i].Count; j++)
                    {
                        _legacyTimestamps.Add(Timestamps[i][j] + Sequences[i].TimeStart);
                    }
                }
            }
            else if (Timestamps.Count == 1)
            {
                _legacyTimestamps.AddRange(Timestamps[0]);
            }
            GenerateLegacyRanges();
            _legacyRanges.Save(stream, version);
            _legacyTimestamps.Save(stream, version);
        }

        private void LegacySaveContent(BinaryWriter stream, M2.Format version)
        {
            _legacyRanges.SaveContent(stream, version);
            _legacyTimestamps.SaveContent(stream, version);
        }

        /// <summary>
        ///     Pre : Sequences set with TimeStart, Sequences set, LegacyTimestamps computed
        /// </summary>
        private void GenerateLegacyRanges()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var index = 0; index < Sequences.Count; index++)
            {
                if (_legacyTimestamps.Count < 2)
                {
                    _legacyRanges.Add(new Range());
                    continue;
                }
                var seq = Sequences[index];
                var indexesPrevious =
                    Enumerable.Range(0, _legacyTimestamps.Count) // Indexes of times <= to the beginning of sequence.
                        .Where(i => _legacyTimestamps[i] <= seq.TimeStart)
                        .ToList();
                var indexesNext =
                    Enumerable.Range(0, _legacyTimestamps.Count) // Indexes of times >= to the end of sequence.
                        .Where(i => _legacyTimestamps[i] >= seq.TimeStart + seq.Length)
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
            Debug.Assert(Sequences != null, "Sequences is null in M2TrackBase");
            _legacyRanges.Load(stream, version);
            _legacyTimestamps.Load(stream, version);
        }

        private void LegacyLoadContent(BinaryReader stream, M2.Format version)
        {
            _legacyRanges.LoadContent(stream, version);
            _legacyTimestamps.LoadContent(stream, version);
            if (_legacyTimestamps.Count == 0) return;
            if (GlobalSequence >= 0)
            {
                Timestamps.Add(_legacyTimestamps);
            }
            else
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var index = 0; index < Sequences.Count; index++)
                {
                    var seq = Sequences[index];
                    var validIndexes = Enumerable.Range(0, _legacyTimestamps.Count)
                        .Where(
                            i =>
                                _legacyTimestamps[i] >= seq.TimeStart &&
                                _legacyTimestamps[i] <= seq.TimeStart + seq.Length)
                        .ToList();

                    var animTimes = new M2Array<uint>();
                    if (validIndexes.Count > 0)
                    {
                        var firstIndex = validIndexes[0];
                        var lastIndex = validIndexes[validIndexes.Count - 1];
                        animTimes.AddRange(_legacyTimestamps.GetRange(firstIndex, lastIndex - firstIndex + 1));
                    }
                    Timestamps.Add(animTimes);
                }
            }
        }
    }
