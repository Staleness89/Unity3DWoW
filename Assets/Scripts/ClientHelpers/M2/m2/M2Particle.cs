using System.Collections.Generic;
using System.IO;

    public class M2Particle : IAnimated
    {
        public int Unknown { get; set; } = -1;
        public uint Flags { get; set; }
        public C3Vector Position { get; set; }
        public ushort Bone { get; set; }
        public ushort Texture { get; set; }

        public string ModelFileName
        {
            get { return _modelFileName.ToNameString(); }
            set { _modelFileName.SetString(value); }
        }
        public string ChildEmitterFileName
        {
            get { return _childEmitterFileName.ToNameString(); }
            set { _childEmitterFileName.SetString(value); }
        }
        private readonly M2Array<byte> _modelFileName = new M2Array<byte>(); 
        private readonly M2Array<byte> _childEmitterFileName = new M2Array<byte>(); 

        public byte BlendingType { get; set; }
        public byte EmitterType { get; set; }
        public ushort ParticleColorIndex { get; set; }
        public byte ParticleType { get; set; }
        public byte HeadOrTail { get; set; }
        public ushort TextureTileRotation { get; set; }
        public ushort TextureDimensionsRows { get; set; }
        public ushort TextureDimensionsColumns { get; set; }
        public M2Track<float> EmissionSpeed { get; set; } = new M2Track<float>();
        public M2Track<float> SpeedVariation { get; set; } = new M2Track<float>();
        public M2Track<float> VerticalRange { get; set; } = new M2Track<float>();
        public M2Track<float> HorizontalRange { get; set; } = new M2Track<float>();
        public M2Track<float> Gravity { get; set; } = new M2Track<float>();
        public M2Track<float> Lifespan { get; set; } = new M2Track<float>();
        public float LifespanVary { get; set; }
        public M2Track<float> EmissionRate { get; set; } = new M2Track<float>();
        public float EmissionRateVary { get; set; }
        public M2Track<float> EmissionAreaLength { get; set; } = new M2Track<float>();
        public M2Track<float> EmissionAreaWidth { get; set; } = new M2Track<float>();
        public M2Track<float> ZSource { get; set; } = new M2Track<float>();
        public M2FakeTrack<C3Vector> ColorTrack { get; set; } = new M2FakeTrack<C3Vector>();
        public M2FakeTrack<FixedPoint_0_15> AlphaTrack { get; set; } = new M2FakeTrack<FixedPoint_0_15>();
        public M2FakeTrack<C2Vector> ScaleTrack { get; set; } = new M2FakeTrack<C2Vector>();
        public C2Vector ScaleVary { get; set; }
        public M2FakeTrack<ushort> HeadCellTrack { get; set; } = new M2FakeTrack<ushort>();
        public M2FakeTrack<ushort> TailCellTrack { get; set; } = new M2FakeTrack<ushort>();
        public float SomethingParticleStyle { get; set; }
        public C2Vector Spread { get; set; }
        public CRange TwinkleScale { get; set; }
        public float Blank1 { get; set; }
        public float Drag { get; set; }
        public float BaseSpin { get; set; }
        public float BaseSpinVary { get; set; }
        public float Spin { get; set; }
        public float SpinVary { get; set; }
        public float Blank2 { get; set; }
        public C3Vector Model1Rotation { get; set; }
        public C3Vector Model2Rotation { get; set; }
        public C3Vector ModelTranslation { get; set; }
        public C4Vector FollowParams { get; set; }
        public M2Array<C3Vector> UnknownReference { get; set; } = new M2Array<C3Vector>();
        public M2Track<bool> EnabledIn { get; set; } = new M2Track<bool>(true);
        public readonly FixedPoint_6_9[] MultiTextureParam0 = new FixedPoint_6_9[4];
        public readonly FixedPoint_6_9[] MultiTextureParam1 = new FixedPoint_6_9[4];
        public readonly FixedPoint_2_5[] MultiTextureParamX = new FixedPoint_2_5[2];
        public readonly float[] LegacyManyFloats = new float[10];
        public readonly short[] LegacyTiles = new short[4];


        public void Load(BinaryReader stream, M2.Format version)
        {
            Unknown = stream.ReadInt32();
            Flags = stream.ReadUInt32();
            Position = stream.ReadC3Vector();
            Bone = stream.ReadUInt16();
            Texture = stream.ReadUInt16();
            _modelFileName.Load(stream, version);
            _childEmitterFileName.Load(stream, version);
            if (version >= M2.Format.LateBurningCrusade)
            {
                BlendingType = stream.ReadByte();
                EmitterType = stream.ReadByte();
                ParticleColorIndex = stream.ReadUInt16();
            }
            else
            {
                BlendingType = (byte) stream.ReadUInt16();
                EmitterType = (byte) stream.ReadUInt16();
            }
            if (version >= M2.Format.Cataclysm)
            {
                for (var i = 0; i < MultiTextureParamX.Length; i++)
                    MultiTextureParamX[i] = stream.ReadFixedPoint_2_5();
            }
            else
            {
                ParticleType = stream.ReadByte();
                HeadOrTail = stream.ReadByte();
            }
            TextureTileRotation = stream.ReadUInt16();
            TextureDimensionsRows = stream.ReadUInt16();
            TextureDimensionsColumns = stream.ReadUInt16();
            EmissionSpeed.Load(stream, version);
            SpeedVariation.Load(stream, version);
            VerticalRange.Load(stream, version);
            HorizontalRange.Load(stream, version);
            Gravity.Load(stream, version);
            Lifespan.Load(stream, version);
            if(version >= M2.Format.LichKing) LifespanVary = stream.ReadSingle();
            EmissionRate.Load(stream, version);
            if (version >= M2.Format.LichKing) EmissionRateVary = stream.ReadSingle();
            EmissionAreaLength.Load(stream, version);
            EmissionAreaWidth.Load(stream, version);
            ZSource.Load(stream, version);
            if (version >= M2.Format.LichKing)
            {
                ColorTrack.Load(stream, version);
                AlphaTrack.Load(stream, version);
                ScaleTrack.Load(stream, version);
                ScaleVary = stream.ReadC2Vector();
                HeadCellTrack.Load(stream, version);
                TailCellTrack.Load(stream, version);
            }
            else
            {
                var midPoint = stream.ReadSingle();
                var colorTrack = new []{stream.ReadCArgb(), stream.ReadCArgb(), stream.ReadCArgb()};
                var scaleTrack = new []{stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle()};
                var headCellTrack1 = new [] {stream.ReadUInt16(), stream.ReadUInt16()};
                stream.ReadInt16();//Always 1
                var headCellTrack2 = new [] {stream.ReadUInt16(), stream.ReadUInt16()};
                stream.ReadInt16();//Always 1
                for (var i = 0; i < LegacyTiles.Length; i++)
                    LegacyTiles[i] = stream.ReadInt16(); //TODO 4 tailCellTrack ?
                ColorTrack.Timestamps.Add(0);
                ColorTrack.Values.Add(new C3Vector(colorTrack[0].B, colorTrack[0].G, colorTrack[0].R));
                ColorTrack.Timestamps.Add((short) (midPoint*short.MaxValue));
                ColorTrack.Values.Add(new C3Vector(colorTrack[1].B, colorTrack[1].G, colorTrack[1].R));
                ColorTrack.Timestamps.Add(short.MaxValue);
                ColorTrack.Values.Add(new C3Vector(colorTrack[2].B, colorTrack[2].G, colorTrack[2].R));

                AlphaTrack.Timestamps.Add(0);
                AlphaTrack.Values.Add(new FixedPoint_0_15((short)(128 * colorTrack[0].A)));
                AlphaTrack.Timestamps.Add((short) (midPoint*short.MaxValue));
                AlphaTrack.Values.Add(new FixedPoint_0_15((short)(128 * colorTrack[1].A)));
                AlphaTrack.Timestamps.Add(short.MaxValue);
                AlphaTrack.Values.Add(new FixedPoint_0_15((short)(128 * colorTrack[2].A)));

                ScaleTrack.Timestamps.Add(0);
                ScaleTrack.Values.Add(new C2Vector(scaleTrack[0], 0));
                ScaleTrack.Timestamps.Add((short) (midPoint*short.MaxValue));
                ScaleTrack.Values.Add(new C2Vector(scaleTrack[1], 0));
                ScaleTrack.Timestamps.Add(short.MaxValue);
                ScaleTrack.Values.Add(new C2Vector(scaleTrack[2], 0));

                HeadCellTrack.Timestamps.Add(0);
                HeadCellTrack.Values.Add(headCellTrack1[0]);
                HeadCellTrack.Timestamps.Add((short) (midPoint*short.MaxValue));
                HeadCellTrack.Values.Add(headCellTrack1[1]);
                HeadCellTrack.Timestamps.Add((short) (midPoint*short.MaxValue));
                HeadCellTrack.Values.Add(headCellTrack2[0]);
                HeadCellTrack.Timestamps.Add(short.MaxValue);
                HeadCellTrack.Values.Add(headCellTrack2[1]);
                // TODO TailCellTrack
            }
            SomethingParticleStyle = stream.ReadSingle();
            Spread = stream.ReadC2Vector();
            TwinkleScale = stream.ReadCRange();
            Blank1 = stream.ReadSingle();
            Drag = stream.ReadSingle();
            if (version >= M2.Format.LichKing)
            {
                BaseSpin = stream.ReadSingle();
                BaseSpinVary = stream.ReadSingle();
                Spin = stream.ReadSingle();
                SpinVary = stream.ReadSingle();
                Blank2 = stream.ReadSingle();
                Model1Rotation = stream.ReadC3Vector();
                Model2Rotation = stream.ReadC3Vector();
                ModelTranslation = stream.ReadC3Vector();
            }
            else
            {
                var rotation = stream.ReadSingle();
                for (var i = 0; i < LegacyManyFloats.Length; i++) LegacyManyFloats[i] = stream.ReadSingle();//LegacyManyFloats
                BaseSpin = rotation;//TODO maybe not be right. Also, the other floats may fit BaseSpinVary, Spin and SpinVary
            }
            FollowParams = stream.ReadC4Vector();
            UnknownReference.Load(stream, version);
            EnabledIn.Load(stream, version);
            if (version <= M2.Format.LichKing) return;
            for (var i = 0; i < MultiTextureParam0.Length; i++)
                MultiTextureParam0[i] = stream.ReadFixedPoint_6_9();
            for (var i = 0; i < MultiTextureParam1.Length; i++)
                MultiTextureParam1[i] = stream.ReadFixedPoint_6_9();
        }

        public void Save(BinaryWriter stream, M2.Format version)
        {
            stream.Write(Unknown);
            if(version < M2.Format.LichKing) stream.Write(Flags & 0xFFFF);
            else stream.Write(Flags);
            stream.Write(Position);
            stream.Write(Bone);
            stream.Write(Texture);
            _modelFileName.Save(stream, version);
            _childEmitterFileName.Save(stream, version);
            if (version >= M2.Format.LateBurningCrusade)
            {
                stream.Write(BlendingType);
                stream.Write(EmitterType);
                stream.Write(ParticleColorIndex);
            }
            else
            {
                stream.Write((ushort) BlendingType);
                stream.Write((ushort) EmitterType);
            }
            if (version >= M2.Format.Cataclysm)
            {
                foreach (var item in MultiTextureParamX)
                    stream.Write(item);
            }
            else
            {
                stream.Write(ParticleType);
                stream.Write(HeadOrTail);
            }
            stream.Write(TextureTileRotation);
            stream.Write(TextureDimensionsRows);
            stream.Write(TextureDimensionsColumns);
            EmissionSpeed.Save(stream, version);
            SpeedVariation.Save(stream, version);
            VerticalRange.Save(stream, version);
            HorizontalRange.Save(stream, version);
            Gravity.Save(stream, version);
            Lifespan.Save(stream, version);
            if (version >= M2.Format.LichKing) stream.Write(LifespanVary);
            EmissionRate.Save(stream, version);
            if (version >= M2.Format.LichKing) stream.Write(EmissionRateVary);
            EmissionAreaLength.Save(stream, version);
            EmissionAreaWidth.Save(stream, version);
            ZSource.Save(stream, version);
            if (version >= M2.Format.LichKing)
            {
                ColorTrack.Save(stream, version);
                AlphaTrack.Save(stream, version);
                ScaleTrack.Save(stream, version);
                stream.Write(ScaleVary);
                HeadCellTrack.Save(stream, version);
                TailCellTrack.Save(stream, version);
            }
            else
            {
                float midPoint = 0;
                var colorTrack = new CArgb[3];
                var scaleTrack = new float[3];
                var headCellTrack1 = new ushort[2];
                var headCellTrack2 = new ushort[2];

                if (ColorTrack.Values.Count >= 3)
                {
                    midPoint = (float) ColorTrack.Timestamps[1] / short.MaxValue;
                    for (var i = 0; i < colorTrack.Length; i++)
                    {
                        var color = ColorTrack.Values[i];
                        var alpha = AlphaTrack.Values.Count >= 3 ? AlphaTrack.Values[i] : new FixedPoint_0_15(short.MaxValue);
                        colorTrack[i] = new CArgb((byte) color.Z, (byte) color.Y, (byte) color.X, (byte) (alpha.ToShort()/128));
                    }
                }
                if (HeadCellTrack.Values.Count >= 4)
                {
                    headCellTrack1[0] = HeadCellTrack.Values[0];
                    headCellTrack1[0] = HeadCellTrack.Values[1];
                    headCellTrack2[0] = HeadCellTrack.Values[2];
                    headCellTrack2[0] = HeadCellTrack.Values[3];
                }
                //TODO Convert BC particles HeadCellTrack

                stream.Write(midPoint);
                for (var i = 0; i < colorTrack.Length; i++)
                    stream.Write(colorTrack[i]);
                for (var i = 0; i < scaleTrack.Length; i++)
                    stream.Write(scaleTrack[i]);
                for (var i = 0; i < headCellTrack1.Length; i++)
                    stream.Write(headCellTrack1[i]);
                stream.Write((short) 1);
                for (var i = 0; i < headCellTrack2.Length; i++)
                    stream.Write(headCellTrack2[i]);
                stream.Write((short) 1);
                for (var i = 0; i < LegacyTiles.Length; i++)
                    stream.Write(LegacyTiles[i]);
            }
            stream.Write(SomethingParticleStyle);
            stream.Write(Spread);
            stream.Write(TwinkleScale);
            stream.Write(Blank1);
            stream.Write(Drag);
            if (version >= M2.Format.LichKing)
            {
                stream.Write(BaseSpin);
                stream.Write(BaseSpinVary);
                stream.Write(Spin);
                stream.Write(SpinVary);
                stream.Write(Blank2);
                stream.Write(Model1Rotation);
                stream.Write(Model2Rotation);
                stream.Write(ModelTranslation);
            }
            else
            {
                var rotation = BaseSpin;//TODO maybe not be right. Also, the other floats may fit BaseSpinVary, Spin and SpinVary
                stream.Write(rotation);
                for (var i = 0; i < LegacyManyFloats.Length; i++) stream.Write(LegacyManyFloats[i]);//LegacyManyFloats
            }
            stream.Write(FollowParams);
            UnknownReference.Save(stream, version);
            if (version < M2.Format.LichKing && EnabledIn.Timestamps.Count == 0)
            {
                EnabledIn.Timestamps.Add(new M2Array<uint> { 0 });
                EnabledIn.Values.Add(new M2Array<bool> { true });
            }
            EnabledIn.Save(stream, version);
            if (version <= M2.Format.LichKing) return;
            foreach (var item in MultiTextureParam0)
                stream.Write(item);
            foreach (var item in MultiTextureParam1)
                stream.Write(item);
        }

        public void LoadContent(BinaryReader stream, M2.Format version)
        {
            _modelFileName.LoadContent(stream, version);
            _childEmitterFileName.LoadContent(stream, version);
            EmissionSpeed.LoadContent(stream, version);
            SpeedVariation.LoadContent(stream, version);
            VerticalRange.LoadContent(stream, version);
            HorizontalRange.LoadContent(stream, version);
            Gravity.LoadContent(stream, version);
            Lifespan.LoadContent(stream, version);
            EmissionRate.LoadContent(stream, version);
            EmissionAreaLength.LoadContent(stream, version);
            EmissionAreaWidth.LoadContent(stream, version);
            ZSource.LoadContent(stream, version);
            if (version >= M2.Format.LichKing)
            {
                ColorTrack.LoadContent(stream, version);
                AlphaTrack.LoadContent(stream, version);
                ScaleTrack.LoadContent(stream, version);
                HeadCellTrack.LoadContent(stream, version);
                TailCellTrack.LoadContent(stream, version);
            }
            UnknownReference.LoadContent(stream, version);
            EnabledIn.LoadContent(stream, version);
        }

        public void SaveContent(BinaryWriter stream, M2.Format version)
        {
            _modelFileName.SaveContent(stream, version);
            _childEmitterFileName.SaveContent(stream, version);
            EmissionSpeed.SaveContent(stream, version);
            SpeedVariation.SaveContent(stream, version);
            VerticalRange.SaveContent(stream, version);
            HorizontalRange.SaveContent(stream, version);
            Gravity.SaveContent(stream, version);
            Lifespan.SaveContent(stream, version);
            EmissionRate.SaveContent(stream, version);
            EmissionAreaLength.SaveContent(stream, version);
            EmissionAreaWidth.SaveContent(stream, version);
            ZSource.SaveContent(stream, version);
            if (version >= M2.Format.LichKing)
            {
                ColorTrack.SaveContent(stream, version);
                AlphaTrack.SaveContent(stream, version);
                ScaleTrack.SaveContent(stream, version);
                HeadCellTrack.SaveContent(stream, version);
                TailCellTrack.SaveContent(stream, version);
            }
            UnknownReference.SaveContent(stream, version);
            EnabledIn.SaveContent(stream, version);
        }

        public void SetSequences(IReadOnlyList<M2Sequence> sequences)
        {
            EmissionSpeed.Sequences = sequences;
            SpeedVariation.Sequences = sequences;
            VerticalRange.Sequences = sequences;
            HorizontalRange.Sequences = sequences;
            Gravity.Sequences = sequences;
            Lifespan.Sequences = sequences;
            EmissionRate.Sequences = sequences;
            EmissionAreaLength.Sequences = sequences;
            EmissionAreaWidth.Sequences = sequences;
            ZSource.Sequences = sequences;
            EnabledIn.Sequences = sequences;
        }
    }

