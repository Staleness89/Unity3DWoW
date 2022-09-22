using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

    public class M2Bone : IAnimated
    {
        [Flags]
        public enum BoneFlags
        {
            SphericalBillboard = 0x8,
            CylindricalBillboardLockX = 0x10,
            CylindricalBillboardLockY = 0x20,
            CylindricalBillboardLockZ = 0x40,
            Transformed = 0x200,
            KinematicBone = 0x400, // MoP+: allow physics to influence this bone
            HelmetAnimScaled = 0x1000 // set blend_modificator to helmetAnimScalingRec.m_amount for this bone
        }

        private M2Track<M2CompQuat> _compressedRotation;

        private readonly ushort[] _unknown = new ushort[2];
        public KeyBone KeyBoneId { get; set; } = (KeyBone) (-1);
        public BoneFlags Flags { get; set; } = 0;
        public short ParentBone { get; set; } = -1;
        public ushort SubmeshId { get; set; }
        public M2Track<C3Vector> Translation { get; set; } = new M2Track<C3Vector>();
        public M2Track<C4Quaternion> Rotation { get; set; } = new M2Track<C4Quaternion>(new C4Quaternion(0,0,0,1));
        public M2Track<C3Vector> Scale { get; set; } = new M2Track<C3Vector>(new C3Vector(1, 1, 1));
        public C3Vector Pivot { get; set; }

        public void Load(BinaryReader stream, M2.Format version)
        {
            Debug.Assert(version != M2.Format.Useless);
            KeyBoneId = (KeyBone) stream.ReadInt32();
            Flags = (BoneFlags) stream.ReadUInt32();
            ParentBone = stream.ReadInt16();
            SubmeshId = stream.ReadUInt16();
            if (version > M2.Format.Classic)
            {
                _unknown[0] = stream.ReadUInt16();
                _unknown[1] = stream.ReadUInt16();
            }
            Translation.Load(stream, version);
            if (version > M2.Format.Classic)
            {
                _compressedRotation = new M2Track<M2CompQuat>(new M2CompQuat(32767, 32767, 32767, -1));
                _compressedRotation.Sequences = Rotation.Sequences;
                _compressedRotation.Load(stream, version);
            }
            else
                Rotation.Load(stream, version);
            Scale.Load(stream, version);
            Pivot = stream.ReadC3Vector();
        }

        public void Save(BinaryWriter stream, M2.Format version)
        {
            Debug.Assert(version != M2.Format.Useless);
            stream.Write((int) KeyBoneId);
            stream.Write((uint) Flags);
            stream.Write(ParentBone);
            stream.Write(SubmeshId);
            if (version > M2.Format.Classic)
            {
                stream.Write(_unknown[0]);
                stream.Write(_unknown[1]);
            }
            Translation.Save(stream, version);
            if (version > M2.Format.Classic)
            {
                _compressedRotation = new M2Track<M2CompQuat>(new M2CompQuat(32767, 32767, 32767, -1));
                _compressedRotation.Sequences = Rotation.Sequences;
                Rotation.Compress(_compressedRotation);
                _compressedRotation.Save(stream, version);
            }
            else
                Rotation.Save(stream, version);
            Scale.Save(stream, version);
            stream.Write(Pivot);
        }

        public void LoadContent(BinaryReader stream, M2.Format version)
        {
            Debug.Assert(version != M2.Format.Useless);
            Translation.LoadContent(stream, version);
            if (version > M2.Format.Classic)
            {
                _compressedRotation.Sequences = Rotation.Sequences;
                _compressedRotation.LoadContent(stream, version);
                _compressedRotation.Decompress(Rotation);
                _compressedRotation = null;
            }
            else
                Rotation.LoadContent(stream, version);
            Scale.LoadContent(stream, version);
        }

        public void SaveContent(BinaryWriter stream, M2.Format version)
        {
            Debug.Assert(version != M2.Format.Useless);
            Translation.SaveContent(stream, version);
            if (version > M2.Format.Classic)
            {
                _compressedRotation.Sequences = Rotation.Sequences;
                _compressedRotation.SaveContent(stream, version);
                _compressedRotation = null;
            }
            else
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
            Debug.Assert(sequences != null, "Tried to set null sequences.");
            Translation.Sequences = sequences;
            Rotation.Sequences = sequences;
            Scale.Sequences = sequences;
        }

        public override string ToString()
        {
            return $"KeyBoneId: {KeyBoneId}, Flags: {Flags}, ParentBone: {ParentBone}, SubmeshId: {SubmeshId}" +
                   $"\nTranslation: {Translation}, " +
                   $"\nRotation: {Rotation}, \n" +
                   $"\nScale: {Scale}, " +
                   $"\nPivot: {Pivot}";
        }

        public static M2Array<short> GenerateKeyBoneLookup(M2Array<M2Bone> bones)
        {
            var lookup = new M2Array<short>();
            var maxId = (int) bones.Max(x => x.KeyBoneId);
            for (short i = 0; i < maxId + 1; i++) lookup.Add(-1);
            for (short i = 0; i < bones.Count; i++)
            {
                var id = (int) bones[i].KeyBoneId;
                if (id >= 0 && lookup[id] == -1) lookup[id] = i;
            }
            return lookup;
        }

        public enum KeyBone
        {
            Other = -1,
            ArmL = 0,
            ArmR,
            ShoulderL,
            ShoulderR,
            SpineLow,
            Waist,
            Head,
            Jaw,
            IndexFingerR,
            MiddleFingerR,
            PinkyFingerR,
            RingFingerR,
            ThumbR,
            IndexFingerL,
            MiddleFingerL,
            PinkyFingerL,
            RingFingerL,
            ThumbL,
            Bth,
            Csr,
            Csl,
            Breath,
            Name,
            NameMount,
            Chd,
            Cch,
            Root,
            Wheel1,
            Wheel2,
            Wheel3,
            Wheel4,
            Wheel5,
            Wheel6,
            Wheel7,
            Wheel8
        }
    }
