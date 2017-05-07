
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class HandlerDefinitions
{
    public static void InitializePacketHandler()
    {
        //Login related opcodes
        PacketManager.DefineOpcodeHandler(WorldServerOpCode.SMSG_AUTH_CHALLENGE, AuthHandler.HandleAuthChallenge);
        PacketManager.DefineOpcodeHandler(WorldServerOpCode.SMSG_CHAR_ENUM, AuthHandler.HandleCharEnum);
        PacketManager.DefineOpcodeHandler(WorldServerOpCode.SMSG_AUTH_RESPONSE, AuthHandler.HandleAuthResponse);
        //PacketManager.DefineOpcodeHandler(WorldServerOpCode.SMSG_PONG, AuthHandler.HandleAuthResponse);
        PacketManager.DefineOpcodeHandler(WorldServerOpCode.SMSG_CHAR_DELETE, AuthHandler.HandleCharDeleteResponse);
        PacketManager.DefineOpcodeHandler(WorldServerOpCode.SMSG_CHAR_CREATE, AuthHandler.HandleCharCreateResponse);
        PacketManager.DefineOpcodeHandler(WorldServerOpCode.SMSG_MESSAGECHAT, ChatHandler.HandleChatMessage);
        PacketManager.DefineOpcodeHandler(WorldServerOpCode.SMSG_NAME_QUERY_RESPONSE, MiscHandler.HandleNameQueryResponse);




        PacketManager.DefineOpcodeHandler(WorldServerOpCode.SMSG_UPDATE_OBJECT, ObjectHandler.HandleObjectUpdate);
        PacketManager.DefineOpcodeHandler(WorldServerOpCode.SMSG_COMPRESSED_UPDATE_OBJECT, ObjectHandler.HandleCompressedObjectUpdate);
        PacketManager.DefineOpcodeHandler(WorldServerOpCode.SMSG_CREATURE_QUERY_RESPONSE, MiscHandler.Handle_CreatureQuery);




    }
}