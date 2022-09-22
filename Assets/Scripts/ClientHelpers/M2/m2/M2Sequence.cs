using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

    public class M2Sequence : IMarshalable
    {
        [Flags]
        public enum SequenceFlags
        {
            RuntimeBlended = 0x01,
            LowPriority = 0x10,
            Looped = 0x20,
            HasNext = 0x40,
            Blended = 0x80,
            Stored = 0x100
        }

        private ushort _padding;
        public ushort AnimationId { get; set; }
        public ushort SubAnimationId { get; set; }
        public uint Length { get; set; } = 3333;
        public float MovingSpeed { get; set; }
        public SequenceFlags Flags { get; set; } = SequenceFlags.Looped;
        public short Probability { get; set; } = short.MaxValue;
        public uint MinimumRepetitions { get; set; }
        public uint MaximumRepetitions { get; set; }
        public ushort BlendTimeStart { get; set; } = 150;
        public ushort BlendTimeEnd { get; set; } = 150;
        public CAaBox Bounds { get; set; }
        public float BoundRadius { get; set; }
        public short NextAnimation { get; set; } = -1;
        public ushort AliasNext { get; set; }

        public string Name => AnimationData.IdToName[AnimationId];
        public uint TimeEnd => TimeStart + Length;

        public bool IsExtern => (Flags & (SequenceFlags.Looped | SequenceFlags.LowPriority | SequenceFlags.Stored)) == 0
            ;

        public bool IsAlias => (Flags & SequenceFlags.HasNext) != 0;

        /// <summary>
        ///     Used to convert to one-timeline animation style
        /// </summary>
        public uint TimeStart { get; set; }

        public BinaryReader ReadingAnimFile { get; set; }
        public BinaryWriter WritingAnimFile { get; set; }

        public void Load(BinaryReader stream, M2.Format version)
        {
            Debug.Assert(version != M2.Format.Useless);
            AnimationId = stream.ReadUInt16();
            SubAnimationId = stream.ReadUInt16();
            if (version >= M2.Format.LichKing)
            {
                Length = stream.ReadUInt32();
            }
            else
            {
                TimeStart = stream.ReadUInt32();
                var timeEnd = stream.ReadUInt32();
                Length = timeEnd - TimeStart;
            }
            MovingSpeed = stream.ReadSingle();
            Flags = (SequenceFlags) stream.ReadUInt32();
            Probability = stream.ReadInt16();
            _padding = stream.ReadUInt16();
            MinimumRepetitions = stream.ReadUInt32();
            MaximumRepetitions = stream.ReadUInt32();
            if (version >= M2.Format.Legion)
            {
                BlendTimeStart = stream.ReadUInt16();
                BlendTimeEnd = stream.ReadUInt16();
            }
            else
            {
                var blendTime = stream.ReadUInt32();
                BlendTimeStart = (ushort) blendTime;
                BlendTimeEnd = (ushort) blendTime;
            }
            Bounds = stream.ReadCAaBox();
            BoundRadius = stream.ReadSingle();
            NextAnimation = stream.ReadInt16();
            AliasNext = stream.ReadUInt16();
        }

        public void Save(BinaryWriter stream, M2.Format version)
        {
            Debug.Assert(version != M2.Format.Useless);
            stream.Write(AnimationId);
            stream.Write(SubAnimationId);
            if (version >= M2.Format.LichKing)
            {
                stream.Write(Length);
            }
            else
            {
                stream.Write(TimeStart);
                stream.Write(TimeStart + Length);
            }
            stream.Write(MovingSpeed);
            stream.Write((uint) Flags);
            stream.Write(Probability);
            stream.Write(_padding);
            stream.Write(MinimumRepetitions);
            stream.Write(MaximumRepetitions);
            if (version >= M2.Format.Legion)
            {
                stream.Write(BlendTimeStart);
                stream.Write(BlendTimeEnd);
            }
            else
            {
                stream.Write((BlendTimeStart + BlendTimeEnd)/2);
            }
            stream.Write(Bounds);
            stream.Write(BoundRadius);
            stream.Write(NextAnimation);
            stream.Write(AliasNext);
        }

        public string GetAnimFilePath(string path)
        {
            var pathDirectory = Path.GetDirectoryName(path);
            Debug.Assert(pathDirectory != null, "pathDirectory != null");
            var pathWithoutExt = Path.GetFileNameWithoutExtension(path);
            string animFileName = $"{pathWithoutExt}{AnimationId:0000}-{SubAnimationId:00}.anim";
            return Path.Combine(pathDirectory, animFileName);
        }

        /// <summary>
        ///     Return the animation that's really implemented (handle aliases).
        /// </summary>
        /// <param name="sequences">List of sequences to navigate in.</param>
        /// <returns></returns>
        public M2Sequence GetRealSequence(IReadOnlyList<M2Sequence> sequences)
        {
            return IsAlias ? sequences[AliasNext].GetRealSequence(sequences) : this;
        }

        // ANIMATION LOOKUP

        public static M2Array<short> GenerateLookup(M2Array<M2Sequence> sequences)
        {
            var lookup = new M2Array<short>();
            var maxId = sequences.Max(x => x.AnimationId);
            for (short i = 0; i <= maxId; i++) lookup.Add(-1);
            for (short i = 0; i < sequences.Count; i++)
            {
                var id = sequences[i].AnimationId;
                if (lookup[id] == -1) lookup[id] = i;
            }
            return lookup;
        }

        // PLAYABLE ANIMATION LOOKUP

        private static ushort GetRealId(ushort id, IReadOnlyList<short> animLookup)
        {
            /* Original tail recursive version :
            if (id < animLookup.Count && (animLookup[id] > -1)) return id;
            return GetRealId(AnimationData.Fallback[id], animLookup);
            */
            while (true)
            {
                if (id < animLookup.Count && (animLookup[id] > -1)) return id;
                id = AnimationData.Fallback[id];
            }
        }

        public static M2Array<PlayableRecord> GeneratePlayableLookup(IReadOnlyList<short> animLookup)
        {
            const int numberOfActions = 226; // From 2.4.3 DB/AnimationData
            var lookup = new M2Array<PlayableRecord>();
            for (ushort i = 0; i <= numberOfActions; i++)
            {
                var record = new PlayableRecord(GetRealId(i, animLookup), 0);
                if (record.FallbackId != i)
                {
                    if (AnimationData.PlayThenStop.Contains(i)) record.Flags = PlayableRecord.PlayFlags.Freeze;
                    else if (AnimationData.PlayBackwards.Contains(i)) record.Flags = PlayableRecord.PlayFlags.Backwards;
                }
                lookup.Add(record);
            }
            return lookup;
        }
    }
