using System.Collections.Generic;
using Client.Chat;
using Client.Chat.Definitions;
using Assets.Scripts.Shared;

namespace Client.World.Network
{
    public partial class WorldSocket
    {
        [PacketHandler(WorldCommand.SMSG_NAME_QUERY_RESPONSE)]
        protected void HandleNameQueryResponse(InPacket packet)
        {
            var pguid = packet.ReadPackedGuid();
            var ukn = packet.ReadByte();
            var name = packet.ReadCString();

            WorldObject[] worldObjects = new WorldObject[Exchange.gameClient.Objects.Count];
            Exchange.gameClient.Objects.Values.CopyTo(worldObjects, 0);

            foreach (WorldObject obj in worldObjects)
            {
                if (obj.GUID == pguid)
                { obj.Name = name; }
            }


            if (!Game.World.PlayerNameLookup.ContainsKey(pguid))
            {
                //! Add name definition per GUID
                Game.World.PlayerNameLookup.Add(pguid, name);
                //! See if any queued messages for this GUID are stored
                Queue<ChatMessage> messageQueue = null;
                if (Game.World.QueuedChatMessages.TryGetValue(pguid, out messageQueue))
                {
                    ChatMessage m;
                    while (messageQueue.GetEnumerator().MoveNext())
                    {
                        //! Print with proper name and remove from queue
                        m = messageQueue.Dequeue();
                        m.Sender.Sender = name;
                        //Game.UI.PresentChatMessage(m);
                    }
                }
            }

            /*
            var realmName = packet.ReadCString();
            var race = (Race)packet.ReadByte();
            var gender = (Gender)packet.ReadByte();
            var cClass = (Class)packet.ReadByte();
            var decline = packet.ReadBoolean();

            if (!decline)
                return;

            for (var i = 0; i < 5; i++)
                var declinedName = packet.ReadCString();
            */
        }

        [PacketHandler(WorldCommand.SMSG_GAMEOBJECT_QUERY_RESPONSE)]
        protected void HandleGameObjectQueryResponse(InPacket packet)
        {

            uint entry = packet.ReadUInt32();
            uint type = packet.ReadUInt32();
            uint displayId = packet.ReadUInt32();

            var name = packet.ReadCString();

            packet.ReadByte();
            packet.ReadByte();
            packet.ReadByte();

            var IconName = packet.ReadCString();
            var CastBarCaption = packet.ReadCString();
            var unk1 = packet.ReadCString();

            var size = packet.ReadSingle();

            WorldObject[] worldObjects = new WorldObject[Exchange.gameClient.Objects.Count];
            Exchange.gameClient.Objects.Values.CopyTo(worldObjects, 0);

            foreach (WorldObject obj in worldObjects)
            {
                if (obj.ObjectEntry == entry)
                { obj.Name = name; }
            }
            
        }

        [PacketHandler(WorldCommand.SMSG_CREATURE_QUERY_RESPONSE)]
        protected void HandleCreatureQueryResponse(InPacket packet)
        {
            var entry = packet.ReadUInt32();

            var name = packet.ReadCString();

            packet.ReadByte();
            packet.ReadByte();
            packet.ReadByte();

            var Subname = packet.ReadCString();
            var IconName = packet.ReadCString();

            uint type_flags = packet.ReadUInt32();
            uint type = packet.ReadUInt32();
            uint family = packet.ReadUInt32();
            uint rank = packet.ReadUInt32();
            uint KillCredit = packet.ReadUInt32();
            uint KillCredit2 = packet.ReadUInt32();
            uint Modelid1 = packet.ReadUInt32();
            uint Modelid2 = packet.ReadUInt32();
            uint Modelid3 = packet.ReadUInt32();
            uint Modelid4 = packet.ReadUInt32();

            float ModHealth = packet.ReadSingle();
            float ModMana = packet.ReadSingle();

            byte RacialLeader = packet.ReadByte();
            
            //for (uint i = 0; i < MAX_CREATURE_QUEST_ITEMS; ++i)
                //data << uint32(ci->questItems[i]);              // itemId[6], quest drop

            uint movementId = packet.ReadUInt32();

            WorldObject[] worldObjects = new WorldObject[Exchange.gameClient.Objects.Count];
            Exchange.gameClient.Objects.Values.CopyTo(worldObjects, 0);

            foreach (WorldObject obj in worldObjects)
            {
                if (obj.ObjectEntry == entry)
                {
                    obj.Name = name;
                }
            }

        }

        [PacketHandler(WorldCommand.SMSG_ITEM_QUERY_SINGLE_RESPONSE)]
        protected void HandleItemQueryResponse(InPacket packet)
        {
            var entry = packet.ReadUInt32();
            var Class = packet.ReadUInt32();
            var SubClass = packet.ReadUInt32();
            var SoundOverrideSubclass = packet.ReadUInt32();

            var name = packet.ReadCString();

            var DisplayID = packet.ReadUInt32();

            var Quality = packet.ReadUInt32();

            var Flags = packet.ReadUInt32();

            var FlagsExtra = packet.ReadUInt32();

            var BuyPrice = packet.ReadUInt32();

            var SellPrice = packet.ReadUInt32();

            var InventoryType = packet.ReadUInt32();

            var AllowedClasses = packet.ReadUInt32();

            var AllowedRaces = packet.ReadUInt32();

            var ItemLevel = packet.ReadUInt32();

            var RequiredLevel = packet.ReadUInt32();

            var RequiredSkillId = packet.ReadUInt32();

            var RequiredSkillLevel = packet.ReadUInt32();

            var RequiredSpell = packet.ReadUInt32();

            var RequiredHonorRank = packet.ReadUInt32();

            var RequiredCityRank = packet.ReadUInt32();

            var RequiredRepFaction = packet.ReadUInt32();

            WorldObject[] worldObjects = new WorldObject[Exchange.gameClient.Objects.Count];
            Exchange.gameClient.Objects.Values.CopyTo(worldObjects, 0);

            foreach (WorldObject obj in worldObjects)
            {
                if (obj.ObjectEntry == entry)
                { obj.Name = name; }
            }

            /* item.RequiredRepValue = packet.ReadUInt32("Required Rep Value");

             item.MaxCount = packet.ReadInt32("Max Count");

             item.MaxStackSize = packet.ReadInt32("Max Stack Size");

             item.ContainerSlots = packet.ReadUInt32("Container Slots");

             item.StatsCount = ClientVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056) ? packet.ReadUInt32("Stats Count") : 10;
             item.StatTypes = new ItemModType?[item.StatsCount.GetValueOrDefault()];
             item.StatValues = new int?[item.StatsCount.GetValueOrDefault()];
             for (int i = 0; i < item.StatsCount; i++)
             {
                 ItemModType type = packet.ReadInt32E<ItemModType>("Stat Type", i);
                 item.StatTypes[i] = type == ItemModType.None ? ItemModType.Mana : type; // TDB
                 item.StatValues[i] = packet.ReadInt32("Stat Value", i);
             }

             if (ClientVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
             {
                 item.ScalingStatDistribution = packet.ReadInt32("SSD ID");
                 item.ScalingStatValue = packet.ReadUInt32("SSD Value");
             }

             int dmgCount = ClientVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767) ? 2 : 5;
             item.DamageMins = new float?[dmgCount];
             item.DamageMaxs = new float?[dmgCount];
             item.DamageTypes = new DamageType?[dmgCount];
             for (int i = 0; i < dmgCount; i++)
             {
                 item.DamageMins[i] = packet.ReadSingle("Damage Min", i);
                 item.DamageMaxs[i] = packet.ReadSingle("Damage Max", i);
                 item.DamageTypes[i] = packet.ReadInt32E<DamageType>("Damage Type", i);
             }


             item.Armor = packet.ReadUInt32("Armor");
             item.HolyResistance = packet.ReadUInt32("HolyResistance");
             item.FireResistance = packet.ReadUInt32("FireResistance");
             item.NatureResistance = packet.ReadUInt32("NatureResistance");
             item.FrostResistance = packet.ReadUInt32("FrostResistance");
             item.ShadowResistance = packet.ReadUInt32("ShadowResistance");
             item.ArcaneResistance = packet.ReadUInt32("ArcaneResistance");

             item.Delay = packet.ReadUInt32("Delay");

             item.AmmoType = packet.ReadInt32E<AmmoType>("Ammo Type");

             item.RangedMod = packet.ReadSingle("Ranged Mod");

             item.TriggeredSpellIds = new int?[5];
             item.TriggeredSpellTypes = new ItemSpellTriggerType?[5];
             item.TriggeredSpellCharges = new int?[5];
             item.TriggeredSpellCooldowns = new int?[5];
             item.TriggeredSpellCategories = new uint?[5];
             item.TriggeredSpellCategoryCooldowns = new int?[5];
             for (int i = 0; i < 5; i++)
             {
                 item.TriggeredSpellIds[i] = packet.ReadInt32<SpellId>("Triggered Spell ID", i);
                 item.TriggeredSpellTypes[i] = packet.ReadInt32E<ItemSpellTriggerType>("Trigger Spell Type", i);
                 item.TriggeredSpellCharges[i] = packet.ReadInt32("Triggered Spell Charges", i);
                 item.TriggeredSpellCooldowns[i] = packet.ReadInt32("Triggered Spell Cooldown", i);
                 item.TriggeredSpellCategories[i] = packet.ReadUInt32("Triggered Spell Category", i);
                 item.TriggeredSpellCategoryCooldowns[i] = packet.ReadInt32("Triggered Spell Category Cooldown", i);
             }

             item.Bonding = packet.ReadInt32E<ItemBonding>("Bonding");

             item.Description = packet.ReadCString();

             item.PageText = packet.ReadUInt32("Page Text");

             item.Language = packet.ReadInt32E<Language>("Language");

             item.PageMaterial = packet.ReadInt32E<PageMaterial>("Page Material");

             item.StartQuestId = (uint)packet.ReadInt32<QuestId>("Start Quest");

             item.LockId = packet.ReadUInt32("Lock ID");

             item.Material = packet.ReadInt32E<Material>("Material");

             item.SheathType = packet.ReadInt32E<SheathType>("Sheath Type");

             item.RandomPropery = packet.ReadInt32("Random Property");

             item.RandomSuffix = packet.ReadUInt32("Random Suffix");

             item.Block = packet.ReadUInt32("Block");

             item.ItemSet = packet.ReadUInt32("Item Set");

             item.MaxDurability = packet.ReadUInt32("Max Durability");

             item.AreaID = packet.ReadUInt32<AreaId>("Area");

             // In this single (?) case, map 0 means no map
             item.MapID = packet.ReadInt32<MapId>("Map");

             item.BagFamily = packet.ReadInt32E<BagFamilyMask>("Bag Family");

             item.TotemCategory = packet.ReadInt32E<TotemCategory>("Totem Category");

             item.ItemSocketColors = new ItemSocketColor?[3];
             item.SocketContent = new uint?[3];
             for (int i = 0; i < 3; i++)
             {
                 item.ItemSocketColors[i] = packet.ReadInt32E<ItemSocketColor>("Socket Color", i);
                 item.SocketContent[i] = packet.ReadUInt32("Socket Item", i);
             }

             item.SocketBonus = packet.ReadInt32("Socket Bonus");

             item.GemProperties = packet.ReadInt32("Gem Properties");

             item.RequiredDisenchantSkill = packet.ReadInt32("Required Disenchant Skill");

             item.ArmorDamageModifier = packet.ReadSingle("Armor Damage Modifier");

             if (ClientVersion.AddedInVersion(ClientVersionBuild.V2_4_2_8209))
                 item.Duration = packet.ReadUInt32("Duration");

             if (ClientVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
                 item.ItemLimitCategory = packet.ReadInt32("Limit Category");

             if (ClientVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
                 item.HolidayID = packet.ReadInt32E<Holiday>("Holiday");

             packet.AddSniffData(StoreNameType.Item, entry.Key, "QUERY_RESPONSE");

             Storage.ItemTemplates.Add(item, packet.TimeSpan);*/
        }
    }
}