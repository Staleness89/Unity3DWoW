using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public static class PacketHandler
    {
    public static Dictionary<WorldOpcode, HandlePacket> OpcodeHandlers = new Dictionary<WorldOpcode, HandlePacket>();
    public delegate void HandlePacket(ref PacketReader packet, ref WorldSession manager);

    public static void DefineOpcodeHandler(WorldOpcode opcode, HandlePacket handler)
    {
        OpcodeHandlers[opcode] = handler;
    }

    public static bool InvokeHandler(PacketReader reader, WorldSession manager, WorldOpcode opcode)
    {
        if (OpcodeHandlers.ContainsKey(opcode))
        {
            OpcodeHandlers[opcode].Invoke(ref reader, ref manager);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void InitializePacketHandler()
    {
        // AuthHandler
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_AUTH_CHALLENGE, AuthHandler.HandleAuthChallenge);
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_CHAR_ENUM, AuthHandler.HandleCharEnum);
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_AUTH_RESPONSE, AuthHandler.HandleAuthResponse);
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_CHAR_DELETE, AuthHandler.HandleCharDeleteResponse);
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_CHAR_CREATE, AuthHandler.HandleCharCreateResponse);
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_LOGIN_VERIFY_WORLD, AuthHandler.HandleLoginVerifyWorld);

        // ChatHandler
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_MESSAGECHAT, ChatHandler.HandleChatMessage);

        // MiscHandler        
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_INITIALIZE_FACTIONS, MiscHandler.Handle_InitializeFactions);
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_INITIAL_SPELLS, MiscHandler.Handle_InitialSpells);
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_TRADE_STATUS, MiscHandler.TradeStatusResponse);
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_TRADE_STATUS_EXTENDED, MiscHandler.TradeStatusExtendedResponse);
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_ACTION_BUTTONS, MiscHandler.Handle_ActionButtons);        
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_PONG, MiscHandler.HandlePong);

        // QueryHandler
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_NAME_QUERY_RESPONSE, QueryHandler.HandleNameQueryResponse);
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_CREATURE_QUERY_RESPONSE, QueryHandler.HandleCreatureQueryResponse);
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_GAMEOBJECT_QUERY_RESPONSE, QueryHandler.HandleGameObjectQueryResponse);
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_ITEM_QUERY_SINGLE_RESPONSE, QueryHandler.HandleItemQueryResponse);

        // ObjectUpdateHandler
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_DESTROY_OBJECT, ObjectUpdateHandler.DestroyObject);
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_UPDATE_OBJECT, ObjectUpdateHandler.HandleObjectUpdate);
        //DefineOpcodeHandler(WorldServerOpCode.SMSG_COMPRESSED_UPDATE_OBJECT, ObjectUpdateHandler.HandleCompressedObjectUpdate);


        // Player Movement
        /*DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_HEARTBEAT, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_START_FORWARD, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_START_BACKWARD, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_STOP, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_START_STRAFE_LEFT, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_START_STRAFE_RIGHT, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_STOP_STRAFE, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_JUMP, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_START_TURN_LEFT, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_START_TURN_RIGHT, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_STOP_TURN, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_START_PITCH_UP, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_START_PITCH_DOWN, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_STOP_PITCH, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_SET_RUN_MODE, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_SET_WALK_MODE, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_FALL_LAND, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_START_SWIM, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_STOP_SWIM, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_SET_FACING, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_SET_PITCH, MoveHandler.HandleMove);
        DefineOpcodeHandler(WorldServerOpCode.SMSG_MONSTER_MOVE, MoveHandler.HandleMonsterMovementPacket);
        DefineOpcodeHandler(WorldServerOpCode.SMSG_COMPRESSED_MOVES, MoveHandler.HandleMonsterMovementPacket);
        DefineOpcodeHandler(WorldServerOpCode.MSG_MOVE_TELEPORT_ACK, MoveHandler.HandleTeleport);*/
    }
}

