using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.World.Network;
using Client.World.Definitions;

namespace Client.World
{
    public class Character
    {
        public ulong GUID;
        public string Name { get; private set; }
        public Race Race { get; private set; }
        public Class Class { get; private set; }
        Gender Gender;
        byte[] Bytes;    // 5
        public byte Level { get; private set; }
        public uint ZoneId;
        public uint MapId;
        public float X, Y, Z;
        uint GuildId;
        uint Flags;
        uint PetInfoId;
        uint PetLevel;
        uint PetFamilyId;

        public Character(InPacket packet)
        {
            GUID = packet.ReadUInt64();
            Name = packet.ReadCString();
            Race = (Race)packet.ReadByte();
            Class = (Class)packet.ReadByte();
            Gender = (Gender)packet.ReadByte();
            Bytes = packet.ReadBytes(5);
            Level = packet.ReadByte();
            ZoneId = packet.ReadUInt32();
            MapId = packet.ReadUInt32();
            X = packet.ReadSingle();
            Y = packet.ReadSingle();
            Z = packet.ReadSingle();
            GuildId = packet.ReadUInt32();
            Flags = packet.ReadUInt32();
            packet.ReadUInt32();    // customize (rename, etc)
            packet.ReadByte();        // first login
            PetInfoId = packet.ReadUInt32();
            PetLevel = packet.ReadUInt32();
            PetFamilyId = packet.ReadUInt32();

            // read items
            for (int i = 0; i < 19; ++i)
            {
                packet.ReadUInt32();
                packet.ReadByte();
                packet.ReadUInt32();    // ???
            }

            // read bags
            for (int i = 0; i < 4; ++i)
            {
                packet.ReadUInt32();
                packet.ReadByte();
                packet.ReadUInt32();
            }
        }
    }
}
