using Assets.Scripts.Shared;
using Client.World;
using Client.World.Definitions;
using Client.World.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.World.Network
{
    public partial class WorldSocket
    {
        [PacketHandler(WorldCommand.SMSG_SET_FORCED_REACTIONS)]
        protected void HandleForcedReactions(InPacket packet)
        {
            var counter = packet.ReadInt32();
            for (var i = 0; i < counter; i++)
            {
                Factions factions = new Factions();
                factions.FactionID = packet.ReadUInt32(); //Faction Id
                factions.ReputationRank = packet.ReadUInt32(); //Reputation Rank
                Exchange.authClient.FactionList.Add(factions.FactionID, factions);
            }
        }

        [PacketHandler(WorldCommand.SMSG_INITIALIZE_FACTIONS)]
        protected void HandleInitializeFactions(InPacket packet)
        {
            var count = packet.ReadInt32();// ("Count");
            for (var i = 0; i < count; i++)
            {
                FactionFlag NpcFlags = (FactionFlag)packet.ReadByte(); 
                //var flag = packet.ReadByte(); // ("Faction Flags", i);
                var fac = packet.ReadUInt32(); //("Faction Standing", i);
            }
        }
    }

    public struct Factions
    {
        public uint FactionID;
        public uint ReputationRank;
    }
}