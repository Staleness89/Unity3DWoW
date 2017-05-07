using Assets.Script.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.World
{
    public class Object
    {
        public string Name = null;

        public WoWGuid Guid;
        public Coordinate Position = null;
        public ObjectType Type;
        public UInt32[] Fields;
        //public MovementInfo Movement;

        public UInt32 Health
        {
            get
            {
                return Fields[(int)UpdateFields.UNIT_FIELD_HEALTH];
            }
        }

        public UInt32 Money
        {
            get
            {
                return Fields[(int)UpdateFields.PLAYER_FIELD_COINAGE];
            }
        }

        public Object(WoWGuid guid)
        {
            this.Guid = guid;
        }

        public void SetPlayer(Character character)
        {
            Name = character.Name;
            Guid = new WoWGuid(character.GUID);
        }

        public void UpdatePlayer(Object obj)
        {
        }

        public void SetField(int x, UInt32 value)
        {
            Fields[x] = value;
        }



    }
}
