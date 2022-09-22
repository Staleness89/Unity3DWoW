using System;
using System.IO;
using System.Linq;

    public class M2Texture : IReferencer
    {
        [Flags]
        public enum TextureFlags : uint
        {
            WrapX = 0x01,
            WrapY = 0x02
        }

        public enum TextureType : uint
        {
            None = 0,
            Skin = 1,
            ObjectSkin = 2,
            WeaponBlade = 3,
            WeaponHandle = 4,
            Environment = 5,
            CharacterHair = 6,
            CharacterFacialHair = 7,
            SkinExtra = 8,
            UiSkin = 9,
            TaurenMane = 10,
            MonsterSkin1 = 11,
            MonsterSkin2 = 12,
            MonsterSkin3 = 13,
            ItemIcon = 14,
            GuildBackgroundColor = 15,
            GuildEmblemColor = 16,
            GuildBorderColor = 17,
            GuildEmblem = 18
        }

        private readonly M2Array<byte> _name = new M2Array<byte>();
        public TextureType Type { get; set; } = TextureType.MonsterSkin1;
        //public TextureFlags Flags { get; set; }

    public TextureFlags Flags = new TextureFlags();

    public void Clear()
    {
        Flags = new TextureFlags();
    }

    public void SetFlag(TextureFlags flag)
    {
        Flags |= flag;
    }
    public void UnSetFlag(TextureFlags flag)
    {
        Flags &= ~flag;
    }
    public bool IsFlagSet(TextureFlags flag)
    {
        return ((Flags & flag) >= (TextureFlags)1) ? true : false;
    }
    public string Name
        {
            get { return _name.ToNameString(); }
            set { _name.SetString(value); }
        }

        public void Load(BinaryReader stream, M2.Format version)
        {
            Type = (TextureType) stream.ReadUInt32();
            SetFlag((TextureFlags)stream.ReadUInt32());
           //Flags = (TextureFlags) stream.ReadUInt32();
            _name.Load(stream, version);
        }

        public void Save(BinaryWriter stream, M2.Format version)
        {
            stream.Write((uint) Type);
            stream.Write((uint) Flags);
            _name.Save(stream, version);
        }

        public void LoadContent(BinaryReader stream, M2.Format version)
        {
            _name.LoadContent(stream, version);
        }

        public void SaveContent(BinaryWriter stream, M2.Format version)
        {
            _name.SaveContent(stream, version);
        }

        public static M2Array<short> GenerateTexReplaceLookup(M2Array<M2Texture> textures)
        {
            var lookup = new M2Array<short>();
            if (textures.Count == 0) return lookup;
            var maxId = (short) textures.Max(x => x.Type);
            for (short i = 0; i <= maxId; i++) lookup.Add(-1);
            for (short i = 0; i < textures.Count; i++)
            {
                var id = (short) textures[i].Type;
                if (lookup[id] == -1) lookup[id] = i;
            }
            return lookup;
        }

        public override string ToString()
        {
            return $"Flags: {Flags}, Name: {Name}, Type: {Type}";
        }
    }
