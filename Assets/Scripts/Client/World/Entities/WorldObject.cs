using Client;
using Client.World.Definitions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject
{
    public ObjectType Type;

    public MovementInfo Movement;

    public uint Map;
    public string Name;
    public int Area;
    public int Zone;

    public uint ObjectEntry
    {
        get
        {
            return this[ObjectField.OBJECT_FIELD_ENTRY];
        }

    }

    public ulong GUID
    {
        get
        {
            return _guid;
        }
        set
        {
            _guid = value;
        }
    }
    ulong _guid;

    public uint this[int index]
    {
        get
        {
            uint value;
            objectFields.TryGetValue(index, out value);
            return value;
        }
        set
        {
            objectFields[index] = value;
            if (OnFieldUpdated != null)
                OnFieldUpdated(this, new UpdateFieldEventArg(index, value, this));
        }
    }
    Dictionary<int, uint> objectFields = new Dictionary<int, uint>();

    public event EventHandler<UpdateFieldEventArg> OnFieldUpdated;

    protected virtual void Reset()
    {
        objectFields.Clear();
    }

    public bool IsType(HighGuid highGuidType)
    {
        return ((GUID & 0xF0F0000000000000) >> 52) == (ulong)highGuidType;
    }

    public uint this[ObjectField index]
    {
        get
        {
            return this[(int)index];
        }
        set
        {
            this[(int)index] = value;
        }
    }

    public enum HighGuid
    {
        Player = 0x000,
        BattleGround1 = 0x101,
        InstanceSave = 0x104,
        Group = 0x105,
        BattleGround2 = 0x109,
        MOTransport = 0x10C,
        Guild = 0x10F,
        Item = 0x400,
        DynObject = 0xF00,
        GameObject = 0xF01,
        Transport = 0xF02,
        Unit = 0xF03,
        Pet = 0xF04,
        Vehicle = 0xF05
    }
    //public Dictionary<int, UpdateField> UpdateFields; // SMSG_UPDATE_OBJECT - CreateObject

    //public ICollection<Dictionary<int, UpdateField>> ChangedUpdateFieldsList; // SMSG_UPDATE_OBJECT - Values

    public uint PhaseMask;

    public HashSet<ushort> Phases; // Possible phases

    public uint DifficultyID;

    public bool ForceTemporarySpawn;

    public virtual bool IsTemporarySpawn()
    {
        return ForceTemporarySpawn;
    }
    
    public float MovementSpeed()
    {
        if (Movement.Speed > 4) //Running
        {
            Movement.Speed = Movement.RunSpeed;
        }
        else
        {
            Movement.Speed = Movement.WalkSpeed;
        }
        return Movement.Speed;
    }

    private static bool MapIsContinent(uint mapId)
    {
        // TODO: remove hardcoded checks and read map dbc instead
        switch (mapId)
        {
            case 0:     // Eastern Kingdoms
            case 1:     // Kalimdor
            case 530:   // Outland
            case 571:   // Northrend
            case 609:   // Ebon Hold
            case 638:   // Gilneas 1
            case 655:   // Gilneas 2
            case 656:   // Gilneas 3
            case 646:   // Deepholm
            case 648:   // Kezan 1
            case 659:   // Kezan 2
            case 661:   // Kezan 3
            case 732:   // Tol Barad
            case 860:   // The Wandering Isle
            case 861:   // Firelands Dailies
            case 870:   // Pandaria
            case 974:   // Darkmoon Faire
            case 1064:  // Mogu Island Daily Area
                return true;
            default:
                return false;
        }
    }
}

public sealed class MovementInfo
{
    public MovementFlags Flags;

    public MovementFlags2 FlagsExtra;

    public bool HasSplineData;

    public Vector3 Position;

    public float Orientation;

    public ulong TransportGuid;

    public Vector3 TransportOffset;

    public float TransportO;

    public Quaternion Rotation;

    public float WalkSpeed;

    public float RunSpeed;

    public float TurnSpeed;

    public float RunBackSpeed;

    public uint VehicleId;

    public bool HasWpsOrRandMov;

    public float Speed;
}