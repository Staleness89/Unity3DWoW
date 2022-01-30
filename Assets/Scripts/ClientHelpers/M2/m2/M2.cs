using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Foole.Mpq;

/// <summary>
///     World of Warcraft model format.
/// </summary>
public class M2
{
    /// <summary>
    ///     Versions of M2 encountered so far.
    /// </summary>
    public enum Format
    {
        Useless = 0xCAFE,
        Classic = 256,
        BurningCrusade = 260,
        LateBurningCrusade = 263,
        LichKing = 264,
        Cataclysm = 272,
        Pandaria = 272,
        Draenor = 272,
        Legion = 274
    }

    [Flags]
    public enum GlobalFlags
    {
        TiltX = 0x0001,
        TiltY = 0x0002,
        Add2Fields = 0x0008,
        LoadPhys = 0x0020,
        HasLod = 0x0080,
        CameraRelated = 0x0100
    }

    private readonly M2Array<byte> _name = new M2Array<byte>();

    public Format Version { get; set; } = Format.Draenor;

    public string Name
    {
        get { return _name.ToNameString(); }
        set { _name.SetString(value); }
    }

    public GlobalFlags GlobalModelFlags { get; set; } = 0;
    public M2Array<int> GlobalSequences { get; } = new M2Array<int>();
    public M2Array<M2Sequence> Sequences { get; } = new M2Array<M2Sequence>();
    public M2Array<M2Bone> Bones { get; } = new M2Array<M2Bone>();
    public M2Array<M2Vertex> GlobalVertexList { get; } = new M2Array<M2Vertex>();
    public M2Array<M2SkinProfile> Views { get; } = new M2Array<M2SkinProfile>();
    public M2Array<M2Color> Colors { get; } = new M2Array<M2Color>();
    public M2Array<M2Texture> Textures { get; } = new M2Array<M2Texture>();
    public M2Array<M2TextureWeight> Transparencies { get; } = new M2Array<M2TextureWeight>();
    public M2Array<M2TextureTransform> TextureTransforms { get; } = new M2Array<M2TextureTransform>();
    public M2Array<M2Material> Materials { get; } = new M2Array<M2Material>();
    public M2Array<M2Attachment> Attachments { get; } = new M2Array<M2Attachment>();
    public M2Array<M2Event> Events { get; } = new M2Array<M2Event>();
    public M2Array<M2Light> Lights { get; } = new M2Array<M2Light>();
    public M2Array<M2Camera> Cameras { get; } = new M2Array<M2Camera>();

    //Data referenced by Views. TODO See if can be generated on the fly.
    public M2Array<short> BoneLookup { get; } = new M2Array<short>();
    public M2Array<short> TexLookup { get; } = new M2Array<short>();
    public M2Array<short> TexUnitLookup { get; } = new M2Array<short>();
    public M2Array<short> TransLookup { get; } = new M2Array<short>();
    public M2Array<short> UvAnimLookup { get; } = new M2Array<short>();

    public CAaBox BoundingBox { get; set; }
    public float BoundingSphereRadius { get; set; }
    public CAaBox CollisionBox { get; set; }
    public float CollisionSphereRadius { get; set; }
    public M2Array<ushort> CollisionTriangles { get; } = new M2Array<ushort>();
    public M2Array<C3Vector> CollisionVertices { get; } = new M2Array<C3Vector>();
    public M2Array<C3Vector> CollisionNormals { get; } = new M2Array<C3Vector>();
    public M2Array<M2Ribbon> Ribbons { get; } = new M2Array<M2Ribbon>();
    public M2Array<M2Particle> Particles { get; } = new M2Array<M2Particle>();
    public M2Array<ushort> BlendingMaps { get; } = new M2Array<ushort>();

    public void Load(MpqStream f, Format version = Format.Useless)
    {
        var stream = new BinaryReader(f);
        // LOAD MAGIC
        var magic = Encoding.UTF8.GetString(stream.ReadBytes(4));
        if (magic == "MD21")
        {
            stream.ReadBytes(4); // Ignore chunked structure of Legion
            stream = new BinaryReader(new Substream(stream.BaseStream));
            magic = Encoding.UTF8.GetString(stream.ReadBytes(4));
        }
        Debug.Assert(magic == "MD20");

        // LOAD HEADER
        if (version == Format.Useless) version = (Format)stream.ReadUInt32();
        else stream.ReadUInt32();
        Version = version;
        Debug.Assert(version != Format.Useless);
        _name.Load(stream, version);
        GlobalModelFlags = (GlobalFlags)stream.ReadUInt32();
        GlobalSequences.Load(stream, version);
        Sequences.Load(stream, version);
        SkipArrayParsing(stream, version);
        if (version < Format.LichKing) SkipArrayParsing(stream, version);
        Bones.Load(stream, version);
        SkipArrayParsing(stream, version);
        GlobalVertexList.Load(stream, version);
        uint nViews = 0; //For Lich King external views system.
        if (version < Format.LichKing) Views.Load(stream, version);
        else nViews = stream.ReadUInt32();
        Colors.Load(stream, version);
        Textures.Load(stream, version);
        Transparencies.Load(stream, version);
        if (version < Format.LichKing) SkipArrayParsing(stream, version); //Unknown Ref
        TextureTransforms.Load(stream, version);
        SkipArrayParsing(stream, version);
        Materials.Load(stream, version);
        BoneLookup.Load(stream, version);
        TexLookup.Load(stream, version);
        TexUnitLookup.Load(stream, version);
        TransLookup.Load(stream, version);
        UvAnimLookup.Load(stream, version);
        BoundingBox = stream.ReadCAaBox();
        BoundingSphereRadius = stream.ReadSingle();
        CollisionBox = stream.ReadCAaBox();
        CollisionSphereRadius = stream.ReadSingle();
        CollisionTriangles.Load(stream, version);
        CollisionVertices.Load(stream, version);
        CollisionNormals.Load(stream, version);
        Attachments.Load(stream, version);
        SkipArrayParsing(stream, version);
        Events.Load(stream, version);
        Lights.Load(stream, version);
        Cameras.Load(stream, version);
        SkipArrayParsing(stream, version);
        Ribbons.Load(stream, version);
        Particles.Load(stream, version);
        if (version >= Format.LichKing && GlobalModelFlags.HasFlag(GlobalFlags.Add2Fields)) BlendingMaps.Load(stream, version);

        // LOAD REFERENCED CONTENT
        _name.LoadContent(stream);
        GlobalSequences.LoadContent(stream);
        Sequences.LoadContent(stream, version);
        if (version >= Format.LichKing)
        {
            foreach (var seq in Sequences.Where(seq => !seq.IsAlias &&
                                                       seq.IsExtern))
            {
                //var substream = stream.BaseStream as Substream;
                var path = f._entry.Filename;// substream != null ? ((FileStream) substream.GetInnerStream()).Name : ((FileStream) stream.BaseStream).Name;
                var pathe = seq.GetAnimFilePath(path);
                MpqStream f2 = AppHandler.Instance.SearchMPQ(pathe);
                seq.ReadingAnimFile = new BinaryReader(f2);
            }
        }
        SetSequences();
        Bones.LoadContent(stream, version);
        GlobalVertexList.LoadContent(stream, version);
        //VIEWS
        if (version < Format.LichKing) Views.LoadContent(stream, version);
        else
        {
            for (var i = 0; i < nViews; i++)
            {
                var view = new M2SkinProfile();
                //var substream = stream.BaseStream as Substream;
                var path = f._entry.Filename;  //substream != null ? ((FileStream)substream.GetInnerStream()).Name : ((FileStream)stream.BaseStream).Name;
                var pathe = M2SkinProfile.SkinFileName(path, i);
                MpqStream f3 = AppHandler.Instance.SearchMPQ(pathe);
                using (var skinFile = new BinaryReader(f3))
                {
                    view.Load(skinFile, version);
                    view.LoadContent(skinFile, version);
                }
                Views.Add(view);
            }
        }
        //VIEWS END
        Colors.LoadContent(stream, version);
        Textures.LoadContent(stream, version);
        Transparencies.LoadContent(stream, version);
        TextureTransforms.LoadContent(stream, version);

        /** @author PhilipTNG */
        if (version < Format.Cataclysm)
        {
            foreach (var mat in Materials)
            {
                // Flags fix
                mat.Flags = mat.Flags & (M2Material.RenderFlags)0x1F;
                // Blending mode fix
                if (mat.BlendMode > M2Material.BlendingMode.DeeprunTram) mat.BlendMode = M2Material.BlendingMode.Mod2X;
            }
        }

        Materials.LoadContent(stream, version);
        BoneLookup.LoadContent(stream, version);
        TexLookup.LoadContent(stream, version);
        TexUnitLookup.LoadContent(stream, version);
        TransLookup.LoadContent(stream, version);
        UvAnimLookup.LoadContent(stream, version);
        CollisionTriangles.LoadContent(stream, version);
        CollisionVertices.LoadContent(stream, version);
        CollisionNormals.LoadContent(stream, version);
        Attachments.LoadContent(stream, version);
        Events.LoadContent(stream, version);
        Lights.LoadContent(stream, version);
        Cameras.LoadContent(stream, version);
        Ribbons.LoadContent(stream, version);
        Particles.LoadContent(stream, version);
        if (version >= Format.LichKing && GlobalModelFlags.HasFlag(GlobalFlags.Add2Fields)) BlendingMaps.LoadContent(stream, version);
        foreach (var seq in Sequences)
            seq.ReadingAnimFile?.Close();
    }

    public void Save(BinaryWriter stream, Format version = Format.Useless)
    {
        SetSequences();

        // SAVE MAGIC
        stream.Write(Encoding.UTF8.GetBytes("MD20"));
        if (version == Format.Useless) version = Version;

        // SAVE HEADER
        stream.Write((uint)version);
        _name.Save(stream, version);
        stream.Write((uint)GlobalModelFlags);
        GlobalSequences.Save(stream, version);
        Sequences.Save(stream, version);
        var sequenceLookup = M2Sequence.GenerateLookup(Sequences);
        sequenceLookup.Save(stream, version);
        M2Array<PlayableRecord> playableLookup = null;
        if (version < Format.LichKing)
        {
            playableLookup = M2Sequence.GeneratePlayableLookup(sequenceLookup);
            playableLookup.Save(stream, version);
        }
        Bones.Save(stream, version);
        var keyBoneLookup = M2Bone.GenerateKeyBoneLookup(Bones);
        keyBoneLookup.Save(stream, version);
        GlobalVertexList.Save(stream, version);
        if (version < Format.LichKing) Views.Save(stream, version);
        else stream.Write(Views.Count);
        Colors.Save(stream, version);
        Textures.Save(stream, version);
        Transparencies.Save(stream, version);
        if (version < Format.LichKing) stream.Write((long)0); //Unknown Ref
        TextureTransforms.Save(stream, version);
        var texReplaceLookup = M2Texture.GenerateTexReplaceLookup(Textures);
        texReplaceLookup.Save(stream, version);
        Materials.Save(stream, version);
        BoneLookup.Save(stream, version);
        TexLookup.Save(stream, version);
        if (version <= Format.LichKing && TexUnitLookup.Count == 0) TexUnitLookup.Add(0);// @author Zim4ik
        TexUnitLookup.Save(stream, version);
        TransLookup.Save(stream, version);
        UvAnimLookup.Save(stream, version);
        stream.Write(BoundingBox);
        stream.Write(BoundingSphereRadius);
        stream.Write(CollisionBox);
        stream.Write(CollisionSphereRadius);
        CollisionTriangles.Save(stream, version);
        CollisionVertices.Save(stream, version);
        CollisionNormals.Save(stream, version);
        Attachments.Save(stream, version);
        var attachmentLookup = M2Attachment.GenerateLookup(Attachments);
        attachmentLookup.Save(stream, version);
        Events.Save(stream, version);
        Lights.Save(stream, version);
        Cameras.Save(stream, version);
        var cameraLookup = M2Camera.GenerateLookup(Cameras);
        cameraLookup.Save(stream, version);
        Ribbons.Save(stream, version);
        Particles.Save(stream, version);
        if (version >= Format.LichKing && GlobalModelFlags.HasFlag(GlobalFlags.Add2Fields))
            BlendingMaps.Save(stream, version);

        // SAVE REFERENCED CONTENT
        _name.SaveContent(stream);
        GlobalSequences.SaveContent(stream);
        if (version < Format.LichKing)
        {
            uint time = 0;
            //Alias system. TODO Alias should be skipped in the timing ?
            foreach (var seq in Sequences/*.Where(seq => !seq.IsAlias)*/)
            {
                time += 3333;
                seq.TimeStart = time;
                time += seq.Length;
            }
            //set the timeStart of Alias to their real counterpart
            /*
            foreach (var seq in Sequences.Where(seq => seq.IsAlias))
                seq.TimeStart = seq.GetRealSequence(Sequences).TimeStart;
                */
        }
        Sequences.SaveContent(stream, version);
        if (version >= Format.LichKing)
        {
            foreach (var seq in Sequences.Where(seq => !seq.IsAlias &&
                                                       seq.IsExtern))
            {
                var substream = stream.BaseStream as Substream;
                var path = substream != null ? ((FileStream)substream.GetInnerStream()).Name : ((FileStream)stream.BaseStream).Name;
                seq.WritingAnimFile =
                    new BinaryWriter(
                        new FileStream(seq.GetAnimFilePath(path), FileMode.Create));
            }
        }
        sequenceLookup.SaveContent(stream);
        playableLookup?.SaveContent(stream);
        Bones.SaveContent(stream, version);
        keyBoneLookup.SaveContent(stream);
        GlobalVertexList.SaveContent(stream, version);
        //VIEWS
        if (version < Format.LichKing) Views.SaveContent(stream, version);
        else
        {
            for (var i = 0; i < Views.Count; i++)
            {
                var substream = stream.BaseStream as Substream;
                var path = substream != null ? ((FileStream)substream.GetInnerStream()).Name : ((FileStream)stream.BaseStream).Name;
                using (var skinFile = new BinaryWriter(
                    new FileStream(M2SkinProfile.SkinFileName(path, i),
                        FileMode.Create)))
                {
                    Views[i].Save(skinFile, version);
                    Views[i].SaveContent(skinFile, version);
                }
            }
        }
        //VIEWS END
        Colors.SaveContent(stream, version);
        Textures.SaveContent(stream, version);
        Transparencies.SaveContent(stream, version);
        TextureTransforms.SaveContent(stream, version);
        texReplaceLookup.SaveContent(stream, version);
        Materials.SaveContent(stream, version);
        BoneLookup.SaveContent(stream, version);
        TexLookup.SaveContent(stream, version);
        TexUnitLookup.SaveContent(stream, version);
        TransLookup.SaveContent(stream, version);
        UvAnimLookup.SaveContent(stream, version);
        CollisionTriangles.SaveContent(stream, version);
        CollisionVertices.SaveContent(stream, version);
        CollisionNormals.SaveContent(stream, version);
        Attachments.SaveContent(stream, version);
        attachmentLookup.SaveContent(stream, version);
        Events.SaveContent(stream, version);
        Lights.SaveContent(stream, version);
        Cameras.SaveContent(stream, version);
        cameraLookup.SaveContent(stream, version);
        Ribbons.SaveContent(stream, version);
        Particles.SaveContent(stream, version);
        if (version >= Format.LichKing && GlobalModelFlags.HasFlag(GlobalFlags.Add2Fields)) BlendingMaps.SaveContent(stream, version);
        foreach (var seq in Sequences)
            seq.WritingAnimFile?.Close();
    }

    private void SetSequences()
    {
        Bones.PassSequences(Sequences);
        Colors.PassSequences(Sequences);
        Transparencies.PassSequences(Sequences);
        TextureTransforms.PassSequences(Sequences);
        Attachments.PassSequences(Sequences);
        Events.PassSequences(Sequences);
        Lights.PassSequences(Sequences);
        Cameras.PassSequences(Sequences);
        Ribbons.PassSequences(Sequences);
        Particles.PassSequences(Sequences);
    }

    /// <summary>
    ///     Skip the parsing of useless M2Array (like lookups, since lookups are generated at writing).
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="version"></param>
    private void SkipArrayParsing(BinaryReader stream, Format version)
    {
        new M2Array<short>().Load(stream, version);
    }
}