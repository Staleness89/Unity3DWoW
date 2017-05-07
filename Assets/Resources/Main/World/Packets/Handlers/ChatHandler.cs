using Assets.Script.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class ChatHandler
{
    public static World PublicWorld = null;// login.wClient;
    public static ArrayList ChatQueued = new ArrayList();

    public struct ChatQueue
    {
        public WoWGuid GUID;
        public byte Type;
        public UInt32 Language;
        public string Channel;
        public UInt32 Length;
        public string Message;
        public byte AFK;

    };

    public static void HandleChatMessage(ref PacketReader packet, ref World manager)
    {
        try
        {
            string channel = null;
            UInt64 guid = 0;
            WoWGuid fguid = null, fguid2 = null;
            string FinalMessage = "";
            string Message = "";
            string username = null;

            ChatMsg Type = (ChatMsg)packet.ReadByte();
            UInt32 Language = packet.ReadUInt32();

            switch (Type)
            {
                case ChatMsg.CHAT_MSG_MONSTER_WHISPER:
                case ChatMsg.CHAT_MSG_RAID_BOSS_WHISPER:
                case ChatMsg.CHAT_MSG_RAID_BOSS_EMOTE:
                case ChatMsg.CHAT_MSG_MONSTER_EMOTE:
                    //data << uint32(strlen(senderName) + 1);
                    //data << senderName;
                    fguid2 = new WoWGuid(packet.ReadUInt64());                         // Unit Target
                    break;

                case ChatMsg.CHAT_MSG_SAY:
                case ChatMsg.CHAT_MSG_PARTY:
                case ChatMsg.CHAT_MSG_YELL:
                    guid = packet.ReadUInt64();
                    guid = packet.ReadUInt64();
                    break;

                case ChatMsg.CHAT_MSG_MONSTER_SAY:
                case ChatMsg.CHAT_MSG_MONSTER_YELL:
                    guid = packet.ReadUInt64();
                    //data << uint32(strlen(senderName) + 1);
                    //data << senderName;
                    fguid2 = new WoWGuid(packet.ReadUInt64());                        // Unit Target
                    break;

                case ChatMsg.CHAT_MSG_CHANNEL:
                    channel = packet.ReadString();
                    //data << uint32(playerRank);
                    //data << ObjectGuid(senderGuid);
                    break;

                default:
                    guid = packet.ReadUInt64();
                    break;
            }

            UInt32 Length = packet.ReadUInt32();
            Message = packet.ReadString();// Encoding.Default.GetString(packet.ReadBytes((int)Length));
            PlayerChatTag chatTag = (PlayerChatTag)packet.ReadByte();

            fguid = new WoWGuid(guid);

            //Message = Regex.Replace(Message, @"\|[rc]{1}[a-zA-z0-9]{0,8}", ""); // Colorfull chat message also isn't the most important thing.

            if (fguid.GetOldGuid() == 0)
            {
                username = "System";
            }
            else
            {
                if (manager.objectMgr.objectExists(fguid))
                    username = manager.objectMgr.getObject(fguid).Name;
            }

            byte afk = 0;

            if (username == null)
            {

                ChatQueue que = new ChatQueue();
                que.GUID = fguid;
                que.Type = (byte)Type;
                que.Language = Language;
                if ((ChatMsg)Type == ChatMsg.CHAT_MSG_CHANNEL)
                    que.Channel = channel;
                que.Length = Length;
                que.Message = Message;
                que.AFK = afk;
                ChatQueued.Add(que);
                MiscHandler.QueryName(guid, ref manager);
                return;
            }

            if (!PublicWorld.inWorld)
            {
                FinalMessage = Message + "\n";
                // MainWorld.ChatHeads.Add(FinalMessage);
            }
            else
            {

                //FinalMessage = "[" + username + "] " + MainWorld.ChatTag + Message + "\n";
                // MainWorld.ChatHeads.Add(FinalMessage);
                // MainWorld.ChatBox = false;

                //MainWorld.ChatMessage = "";
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Exception Occured");
            Debug.LogWarning("Message: " + ex.Message);
            Debug.LogWarning("Stacktrace: " + ex.StackTrace);
        }
    }

    public static void SendChatMsg(ChatMsg Type, Languages Language, string Message)
    {
        if (Type != ChatMsg.CHAT_MSG_WHISPER || Type != ChatMsg.CHAT_MSG_CHANNEL)
            SendChatMsg(Type, Language, Message, "");
    }

    public static void SendChatMsg(ChatMsg Type, Languages Language, string Message, string To)
    {
        PacketWriter packet = new PacketWriter(WorldServerOpCode.CMSG_MESSAGECHAT);
        packet.Write((UInt32)Type);
        packet.Write((UInt32)Language);
        if ((Type == ChatMsg.CHAT_MSG_WHISPER || Type == ChatMsg.CHAT_MSG_CHANNEL) && To != "")
            packet.Write(To);
        packet.Write(Message);
        PublicWorld.Send(packet);
    }
}