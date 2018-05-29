using Assets.Scripts.Shared;
using Client;
using Client.World;
using Client.World.Definitions;
using Client.World.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Player : WorldObject
{   
    public uint this[PlayerField index]
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

    public uint this[UnitField index]
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

    public bool IsGhost
    {
        get
        {
            return HasFlag(PlayerFlags.PLAYER_FLAGS_GHOST);
        }
    }

    public bool IsAlive
    {
        get
        {
            return this[UnitField.UNIT_FIELD_HEALTH] > 0 && !IsGhost;
        }
    }

    WorldObject target;
    public WorldObject Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
        }
    }

    public uint Health
    {
        get
        {
            return this[UnitField.UNIT_FIELD_HEALTH];
        }
    }

    public Gender Gender
    {
        get
        {
            return (Gender)((Bytes0 & 0xFF000000) >> 24);
        }
    }

    public Class Class
    {
        get
        {
            return (Class)((Bytes0 & 0x0000FF00) >> 8);
        }
    }

    public Race Race
    {
        get
        {
            return (Race)((Bytes0 & 0x000000FF) >> 0);
        }
    }

    public uint Bytes0
    {
        get
        {
            return this[UnitField.UNIT_FIELD_BYTES_0];
        }
    }

    public uint Bytes1
    {
        get
        {
            return this[UnitField.UNIT_FIELD_BYTES_1];
        }
    }

    public uint Bytes2
    {
        get
        {
            return this[UnitField.UNIT_FIELD_BYTES_2];
        }
    }

    public uint MaxHealth
    {
        get
        {
            return this[UnitField.UNIT_FIELD_MAXHEALTH];
        }

    }

    public uint Level
    {
        get
        {
            return this[UnitField.UNIT_FIELD_LEVEL];
        }
    }

    public bool HasFlag(PlayerFlags flag)
    {
        return (this[PlayerField.PLAYER_FLAGS] & (uint)flag) != 0;
    }

    public Vector3 CorpsePosition
    {
        get;
        set;
    }

    public bool isGrounded;
    public bool IsGrounded
    {
        get { return isGrounded; }
        set
        {
            isGrounded = value;
        }
    }
}
public class UpdateFieldEventArg : EventArgs
{
    public int Index
    {
        get;
        private set;
    }

    public uint NewValue
    {
        get;
        private set;
    }

    public WorldObject Object
    {
        get;
        private set;
    }

    public UpdateFieldEventArg(int Index, uint NewValue, WorldObject Object)
    {
        this.Index = Index;
        this.NewValue = NewValue;
        this.Object = Object;
    }
}