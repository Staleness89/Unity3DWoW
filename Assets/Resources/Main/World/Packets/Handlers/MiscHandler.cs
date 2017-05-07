using Assets.Script.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class MiscHandler
{
    [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
    public static extern uint MM_GetTime();


    public static void HandlePong(ref PacketReader packet, ref World manager)
    {
        UInt32 Server_Seq = packet.ReadUInt32();
        if (Server_Seq == manager.Ping_Seq)
        {
            manager.Ping_Res_Time = MM_GetTime();
            manager.Latency = manager.Ping_Res_Time - manager.Ping_Req_Time;
            manager.Ping_Seq += 1;
            Debug.LogWarning("Got nice pong. We love server;)");
        }

        Debug.LogWarning("Server pong'd bad sequence! Ours: " + manager.Ping_Seq + " Theirs: " + Server_Seq);
    }

    public static void HandleNameQueryResponse(ref PacketReader packet, ref World manager)
    {
        WoWGuid guid = new WoWGuid(packet.ReadUInt64());
        string name = packet.ReadString();
        packet.ReadByte();
        Race Race = (Race)packet.ReadUInt32();
        Gender Gender = (Gender)packet.ReadUInt32();
        Classname Class = (Classname)packet.ReadUInt32();


        if (manager.objectMgr.objectExists(guid))    // Update existing Object
        {
            Assets.Scripts.World.Object obj = manager.objectMgr.getObject(guid);
            obj.Name = name;
            manager.objectMgr.updateObject(obj);
        }
        else                // Create new Object        -- FIXME: Add to new 'names only' list?
        {
            Assets.Scripts.World.Object obj = new Assets.Scripts.World.Object(guid);
            obj.Name = name;
            manager.objectMgr.addObject(obj);

            for (int i = 0; i < ChatHandler.ChatQueued.Count; i++)
            {
                ChatHandler.ChatQueue message = (ChatHandler.ChatQueue)ChatHandler.ChatQueued[i];
                if (message.GUID.GetOldGuid() == guid.GetOldGuid())
                {
                    //MainWorld.ChatHeads.Add("[" + obj.Name + "] " + MainWorld.ChatTag + message.Message + "\n");
                    ChatHandler.ChatQueued.Remove(message);
                }
            }

        }
    }

    public static void QueryName(UInt64 guid, ref World manager)
    {
        PacketWriter packet = new PacketWriter(WorldServerOpCode.CMSG_NAME_QUERY);
        packet.Write(guid);
        //login.wClient.Send(packet);
    }

    public void CreatureQuery(WoWGuid guid, UInt32 entry, ref World manager)
    {
        PacketWriter packet = new PacketWriter(WorldServerOpCode.CMSG_CREATURE_QUERY);
        packet.Write(entry);
        packet.Write(guid.GetNewGuid());
        //login.wClient.Send(packet);
    }

    public void ObjectQuery(WoWGuid guid, UInt32 entry, ref World manager)
    {
        PacketWriter packet = new PacketWriter(WorldServerOpCode.CMSG_GAMEOBJECT_QUERY);
        packet.Write(entry);
        packet.Write(guid.GetNewGuid());
        //login.wClient.Send(packet);
    }

    public void QueryName(WoWGuid guid, ref World manager)
    {
        PacketWriter packet = new PacketWriter(WorldServerOpCode.CMSG_NAME_QUERY);
        packet.Write(guid.GetNewGuid());
        //login.wClient.Send(packet);
    }

    public static void Handle_CreatureQuery(ref PacketReader packet, ref World manager)
    {
        Entry entry = new Entry();
        entry.entry = packet.ReadUInt32();
        entry.name = packet.ReadString();
        entry.blarg = packet.ReadBytes(3);
        entry.subname = packet.ReadString();
        entry.flags = packet.ReadUInt32();
        entry.subtype = packet.ReadUInt32();
        entry.family = packet.ReadUInt32();
        entry.rank = packet.ReadUInt32();

        foreach (Assets.Scripts.World.Object obj in manager.objectMgr.getObjectArray())
        {
            if (obj.Fields != null)
            {
                if (obj.Fields[(int)UpdateFields.OBJECT_FIELD_ENTRY] == entry.entry)
                {
                    obj.Name = entry.name;
                    manager.objectMgr.updateObject(obj);
                }
            }
        }
    }
}
