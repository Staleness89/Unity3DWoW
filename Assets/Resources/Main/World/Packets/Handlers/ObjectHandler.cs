using Assets.Script.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ObjectHandler
{
    public static void HandleCompressedObjectUpdate(ref PacketReader packet, ref World manager)
    {
        try
        {
            Int32 size = packet.ReadInt32();
            byte[] decomped = packet.ReadRemaining();
            packet = new PacketReader(decomped, 1);
            HandleObjectUpdate(ref packet, ref manager);
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex.StackTrace + " " + ex.Message);
        }
    }

    public static void HandleObjectUpdate(ref PacketReader packet, ref World manager)
    {
        UInt32 UpdateBlocks = packet.ReadUInt32();
        for (int allBlocks = 0; allBlocks < UpdateBlocks; allBlocks++)
        {
            UpdateType type = (UpdateType)packet.ReadByte();

            WoWGuid updateGuid;
            uint updateId;
            uint fCount;
            switch (type)
            {
                case UpdateType.Values:
                    Assets.Scripts.World.Object getObject;
                    updateGuid = new WoWGuid(packet.ReadUInt64());
                    if (manager.objectMgr.objectExists(updateGuid))
                    {
                        getObject = manager.objectMgr.getObject(updateGuid);
                    }
                    else
                    {
                        getObject = new Assets.Scripts.World.Object(updateGuid);
                        manager.objectMgr.addObject(getObject);
                    }
                    //Log.WriteLine(LogType.Normal, "Handling Fields Update for object: {0}", getObject.Guid.ToString());
                    HandleUpdateObjectFieldBlock(packet, getObject, ref manager);
                    manager.objectMgr.updateObject(getObject);
                    break;
                case UpdateType.Create:
                case UpdateType.CreateSelf:
                    updateGuid = new WoWGuid(packet.ReadUInt64());
                    updateId = packet.ReadByte();
                    fCount = GeUpdateFieldsCount(updateId);
                    if (manager.objectMgr.objectExists(updateGuid))
                        manager.objectMgr.delObject(updateGuid);
                    Assets.Scripts.World.Object newObject = new Assets.Scripts.World.Object(updateGuid);
                    newObject.Fields = new UInt32[2000];
                    manager.objectMgr.addObject(newObject);
                    HandleUpdateMovementBlock(ref packet, newObject, ref manager);
                    HandleUpdateObjectFieldBlock(packet, newObject, ref manager);
                    manager.objectMgr.updateObject(newObject);
                    // Log.WriteLine(LogType.Normal, "Handling Creation of object: {0}", newObject.Guid.ToString());
                    break;

                case UpdateType.OutOfRange:
                    fCount = packet.ReadByte();
                    for (int j = 0; j < fCount; j++)
                    {
                        WoWGuid guid = new WoWGuid(packet.ReadUInt64());
                        //Log.WriteLine(LogType.Normal, "Handling delete for object: {0}", guid.ToString());
                        if (manager.objectMgr.objectExists(guid))
                            manager.objectMgr.delObject(guid);
                    }
                    break;
            }
        }

    }

    public static void HandleUpdateMovementBlock(ref PacketReader packet, Assets.Scripts.World.Object newObject, ref World manager)
    {
        UInt16 flags = packet.ReadUInt16();


        if ((flags & 0x20) >= 1)
        {
            UInt32 flags2 = packet.ReadUInt32();
            UInt16 unk1 = packet.ReadUInt16();
            UInt32 unk2 = packet.ReadUInt32();
            newObject.Position = new Coordinate(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());

            if ((flags2 & 0x200) >= 1)
            {
                packet.ReadBytes(21); //transporter
            }

            if (((flags2 & 0x2200000) >= 1) || ((unk1 & 0x20) >= 1))
            {
                packet.ReadBytes(4); // pitch
            }

            packet.ReadBytes(4); //lastfalltime

            if ((flags2 & 0x1000) >= 1)
            {
                packet.ReadBytes(16); // skip 4 floats
            }

            if ((flags2 & 0x4000000) >= 1)
            {
                packet.ReadBytes(4); // skip 1 float
            }

            packet.ReadBytes(32); // all of speeds

            if ((flags2 & 0x8000000) >= 1) //spline ;/
            {
                UInt32 splineFlags = packet.ReadUInt32();

                if ((splineFlags & 0x00020000) >= 1)
                {
                    packet.ReadBytes(4); // skip 1 float
                }
                else
                {
                    if ((splineFlags & 0x00010000) >= 1)
                    {
                        packet.ReadBytes(4); // skip 1 float
                    }
                    else if ((splineFlags & 0x00008000) >= 1)
                    {
                        packet.ReadBytes(12); // skip 3 float
                    }
                }

                packet.ReadBytes(28); // skip 8 float

                UInt32 splineCount = packet.ReadUInt32();

                for (UInt32 j = 0; j < splineCount; j++)
                {
                    packet.ReadBytes(12); // skip 3 float
                }

                packet.ReadBytes(13);

            }
        }

        else if ((flags & 0x100) >= 1)
        {
            packet.ReadBytes(40);
        }
        else if ((flags & 0x40) >= 1)
        {
            newObject.Position = new Coordinate(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());
        }

        if ((flags & 0x8) >= 1)
        {
            packet.ReadBytes(4);
        }

        if ((flags & 0x10) >= 1)
        {
            packet.ReadBytes(4);
        }

        if ((flags & 0x04) >= 1)
        {
            packet.ReadBytes(8);
        }

        if ((flags & 0x2) >= 1)
        {
            packet.ReadBytes(4);
        }

        if ((flags & 0x80) >= 1)
        {
            packet.ReadBytes(8);
        }

        if ((flags & 0x200) >= 1)
        {
            packet.ReadBytes(8);
        }
    }

    public static void HandleUpdateObjectFieldBlock(PacketReader packet, Assets.Scripts.World.Object newObject, ref World manager)
    {
        uint lenght = packet.ReadByte();

        UpdateMask UpdateMask = new UpdateMask();
        UpdateMask.SetCount((ushort)(lenght));
        UpdateMask.SetMask(packet.ReadBytes((int)lenght * 4), (ushort)lenght);
        UInt32[] Fields = new UInt32[UpdateMask.GetCount()];

        for (int i = 0; i < UpdateMask.GetCount(); i++)
        {
            if (!UpdateMask.GetBit((ushort)i))
            {
                UInt32 val = packet.ReadUInt32();
                newObject.SetField(i, val);
                Debug.LogWarning("Update Field: " + (UpdateFields)i + " " + val);
            }
        }
    }
    public void DestroyObject(PacketReader packet, ref World manager)
    {
        WoWGuid guid = new WoWGuid(packet.ReadUInt64());
        manager.objectMgr.delObject(guid);
    }

    public static uint GeUpdateFieldsCount(uint updateId)
    {
        switch ((ObjectType)updateId)
        {
            case ObjectType.Object:
                return (uint)UpdateFields.GAMEOBJECT_END;

            case ObjectType.Unit:
                return (uint)UpdateFields.UNIT_END;

            case ObjectType.Player:
                return (uint)UpdateFields.PLAYER_END;

            case ObjectType.Item:
                return (uint)UpdateFields.ITEM_END;

            case ObjectType.Container:
                return (uint)UpdateFields.CONTAINER_END;

            case ObjectType.DynamicObject:
                return (uint)UpdateFields.DYNAMICOBJECT_END;

            case ObjectType.Corpse:
                return (uint)UpdateFields.CORPSE_END;
            default:
                return 0;
        }
    }
}