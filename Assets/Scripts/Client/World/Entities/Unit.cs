using Client.World.Definitions;
using Client.World.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Unit : WorldObject
{
    public uint Level
    {
        get
        {
            return this[UnitField.UNIT_FIELD_LEVEL];
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

    public float Size
    {
        get
        {
            return this[ObjectField.OBJECT_FIELD_SCALE_X];
        }

    }

    public uint MaxHealth
    {
        get
        {
            return this[UnitField.UNIT_FIELD_MAXHEALTH];
        }

    }

    public uint Health
    {
        get
        {
            return this[UnitField.UNIT_FIELD_HEALTH];
        }

    }

    public uint Faction
    {
        get
        {
            return this[UnitField.UNIT_FIELD_FACTIONTEMPLATE];
        }

    }

    public uint MeleeTime
    {
        get
        {
            return this[UnitField.UNIT_FIELD_BASEATTACKTIME];
        }

    }

    public uint RangedTime
    {
        get
        {
            return this[UnitField.UNIT_FIELD_RANGEDATTACKTIME];
        }

    }

    public uint DisplayID
    {
        get
        {
            return this[UnitField.UNIT_FIELD_DISPLAYID];
        }

    }

    public uint MountID
    {
        get
        {
            return this[UnitField.UNIT_FIELD_MOUNTDISPLAYID];
        }

    }

    public uint DynamicFlags
    {
        get
        {
            return this[UnitField.UNIT_DYNAMIC_FLAGS];
        }

    }

    public uint NpcFlags
    {
        get
        {
            return this[UnitField.UNIT_NPC_FLAGS];
        }

    }

    public float CombatReach
    {
        get
        {
            return this[UnitField.UNIT_FIELD_COMBATREACH];
        }

    }

    public uint UnitFlags
    {
        get
        {
            return this[UnitField.UNIT_FIELD_FLAGS];
        }

    }

    public uint UnitFlags2
    {
        get
        {
            return this[UnitField.UNIT_FIELD_FLAGS_2];
        }

    }

    public uint EmoteState
    {
        get
        {
            return this[UnitField.UNIT_NPC_EMOTESTATE];
        }

    }
    
    public float BoundingRadius
    {
        get
        {
            return this[UnitField.UNIT_FIELD_BOUNDINGRADIUS];
        }

    }

    public ulong UnitTargetGUID
    {
        get
        {
            return this[UnitField.UNIT_FIELD_TARGET];
        }
    }

    public uint UnitFactionTemplate
    {
        get
        {
            return this[UnitField.UNIT_FIELD_FACTIONTEMPLATE];
        }
    }
    
    public void LoadValuesFromUpdateFields()
    {
        //Resistances = GetResistances();
        //ManaMod = this[UnitField.UNIT_FIELD_BASE_MANA];
        //HealthMod = this[UnitField.UNIT_FIELD_BASE_HEALTH];
        //HoverHeight = this[UnitField.UNIT_FIELD_HOVERHEIGHT];

    }

    public short[] GetResistances()
    {
        short[] res = new short[7];
        res[0] = (short)this[UnitField.UNIT_FIELD_RESISTANCES_ARCANE];
        res[1] = (short)this[UnitField.UNIT_FIELD_RESISTANCES_ARMOR];
        res[2] = (short)this[UnitField.UNIT_FIELD_RESISTANCES_FIRE];
        res[3] = (short)this[UnitField.UNIT_FIELD_RESISTANCES_FROST];
        res[4] = (short)this[UnitField.UNIT_FIELD_RESISTANCES_HOLY];
        res[5] = (short)this[UnitField.UNIT_FIELD_RESISTANCES_NATURE];
        res[6] = (short)this[UnitField.UNIT_FIELD_RESISTANCES_SHADOW];

        return res;
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
}