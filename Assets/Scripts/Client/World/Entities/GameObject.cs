using Client.World.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameObject : WorldObject
{
    
    public uint? Entry;
    public GameObjectType GOType;
    public uint? DisplayID;
    public string IconName;
    public string CastCaption;
    public string UnkString;
    public float? Size;

    //TODO: move to gameobject_questitem
    public uint?[] QuestItems;
    public int?[] Data;
    public int? RequiredLevel;
    public uint? GameObjectEntry;

    public uint this[GameObjectField index]
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

    public uint GO_BYTES_1
    {
        get
        {
            return this[GameObjectField.GAMEOBJECT_BYTES_1];
        }
    }

    public uint GO_FLAGS
    {
        get
        {
            return this[GameObjectField.GAMEOBJECT_FLAGS];
        }
    }

    public uint GO_LEVEL
    {
        get
        {
            return this[GameObjectField.GAMEOBJECT_LEVEL];
        }
    }

    public uint GO_DISPLAYID
    {
        get
        {
            return this[GameObjectField.GAMEOBJECT_DISPLAYID];
        }
    }

    public uint GO_FACTION
    {
        get
        {
            return this[GameObjectField.GAMEOBJECT_FACTION];
        }
    }
}