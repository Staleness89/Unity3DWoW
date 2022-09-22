using System;
using System.IO;

    public class M2SkinSection : IMarshalable
    {
        public ushort SubmeshId { get; set; }
        public ushort Level { get; set; }
        public ushort StartVertex { get; set; }
        public ushort NVertices { get; set; }
        public ushort StartTriangle { get; set; }
        public ushort NTriangles { get; set; }
        public ushort NBones { get; set; }
        public ushort StartBones { get; set; }
        public ushort BoneInfluences { get; set; }
        public ushort RootBone { get; set; }
        public C3Vector CenterMass { get; set; }
        public C3Vector CenterBoundingBox { get; set; }
        public float Radius { get; set; }

        public void Load(BinaryReader stream, M2.Format version)
        {
            if (version >= M2.Format.Cataclysm)
            {
                SubmeshId = stream.ReadUInt16();
                Level = stream.ReadUInt16();
            }
            else
            {
                SubmeshId = (ushort) stream.ReadUInt32();
            }
            StartVertex = stream.ReadUInt16();
            NVertices = stream.ReadUInt16();
            StartTriangle = stream.ReadUInt16();
            NTriangles = stream.ReadUInt16();
            NBones = stream.ReadUInt16();
            StartBones = stream.ReadUInt16();
            BoneInfluences = stream.ReadUInt16();
            RootBone = stream.ReadUInt16();
            CenterMass = stream.ReadC3Vector();
            if (version <= M2.Format.Classic) return;
            CenterBoundingBox = stream.ReadC3Vector();
            Radius = stream.ReadSingle();
        }

        public void Save(BinaryWriter stream, M2.Format version)
        {
            if (version < M2.Format.Cataclysm && BoneInfluences == 0) BoneInfluences = 1; //@author Mjollna

            if (version >= M2.Format.Cataclysm)
            {
                stream.Write(SubmeshId);
                stream.Write(Level);
            }
            else
            {
                if (Level > 0) throw new Exception("This model has too many polygons to be saved in this version.");
                stream.Write((uint) SubmeshId);
            }
            stream.Write(StartVertex);
            stream.Write(NVertices);
            stream.Write(StartTriangle);
            stream.Write(NTriangles);
            stream.Write(NBones);
            stream.Write(StartBones);
            stream.Write(BoneInfluences);
            stream.Write(RootBone);
            stream.Write(CenterMass);
            if (version <= M2.Format.Classic) return;
            stream.Write(CenterBoundingBox);
            stream.Write(Radius);
        }
    }
