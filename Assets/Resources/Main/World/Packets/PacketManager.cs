using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class PacketManager
{
    public static Dictionary<WorldServerOpCode, HandlePacket> OpcodeHandlers = new Dictionary<WorldServerOpCode, HandlePacket>();
    public delegate void HandlePacket(ref PacketReader packet, ref World manager);

    public static void DefineOpcodeHandler(WorldServerOpCode opcode, HandlePacket handler)
    {
        OpcodeHandlers[opcode] = handler;
    }

    public static bool InvokeHandler(PacketReader reader, World manager, WorldServerOpCode opcode)
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
}