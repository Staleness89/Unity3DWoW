using Assets.Scripts.Shared;
using Client.World;
using Client.World.Definitions;
using Client.World.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Client.World.Movement
{
    public class MovementMgr
    {
        private System.Timers.Timer aTimer = new System.Timers.Timer();
        public MovementFlag Flags = new MovementFlag();
        UInt32 lastUpdateTime;


        public MovementMgr()
        {

        }

        public float ConvertToRadians(float angle)
        {
            return (float)(Math.PI / 180) * angle;
        }

        public void SendHeartBeat(Vector3 o, Quaternion i)
        {
            var Orientation = -(ConvertToRadians(i.eulerAngles.y)) - 4.6f;

            var startMoving = new MovementPacket(WorldCommand.MSG_MOVE_HEARTBEAT)
            {
                GUID = Exchange.authClient.Player.GUID,
                flags = (MovementFlags)Flags.MoveFlags,
                flags2 = (MovementFlags2)Flags.MoveFlags2,
                X = o.x,
                Y = o.z,
                Z = o.y,
                O = Orientation
            };
            Exchange.authClient.SendPacket(startMoving);

            Flags.Clear();
            Flags.Clear2();
        }

        public void SendFallLand(Vector3 o, Quaternion i, uint time)
        {
            var Orientation = -(ConvertToRadians(i.eulerAngles.y)) - 4.6f;

            var startMoving = new MovementPacket(WorldCommand.MSG_MOVE_FALL_LAND)
            {
                GUID = Exchange.authClient.Player.GUID,
                flags = (MovementFlags)Flags.MoveFlags,
                flags2 = (MovementFlags2)Flags.MoveFlags2,
                fallTime = time,
                X = o.x,
                Y = o.z,
                Z = o.y,
                O = Orientation
            };
            Exchange.authClient.SendPacket(startMoving);

            Flags.Clear();
            Flags.Clear2();
        }

        public void SendStopTurn(Vector3 o, Quaternion i)
        {
            var Orientation = -(ConvertToRadians(i.eulerAngles.y)) - 4.6f;

            var startMoving = new MovementPacket(WorldCommand.MSG_MOVE_STOP_TURN)
            {
                GUID = Exchange.authClient.Player.GUID,
                flags = (MovementFlags)Flags.MoveFlags,
                flags2 = (MovementFlags2)Flags.MoveFlags2,
                X = o.x,
                Y = o.z,
                Z = o.y,
                O = Orientation
            };
            Exchange.authClient.SendPacket(startMoving);

            Flags.Clear();
            Flags.Clear2();
        }

        public void SendMoveLeft(Vector3 o, Quaternion i)
        {
            var Orientation = -(ConvertToRadians(i.eulerAngles.y)) - 4.6f;

            var startMoving = new MovementPacket(WorldCommand.MSG_MOVE_START_TURN_LEFT)
            {
                GUID = Exchange.authClient.Player.GUID,
                flags = (MovementFlags)Flags.MoveFlags,
                flags2 = (MovementFlags2)Flags.MoveFlags2,
                X = o.x,
                Y = o.z,
                Z = o.y,
                O = Orientation
            };
            Exchange.authClient.SendPacket(startMoving);

            Flags.Clear();
            Flags.Clear2();
        }

        public void SendMoveRight(Vector3 o, Quaternion i)
        {
            var Orientation = -(ConvertToRadians(i.eulerAngles.y)) - 4.6f;

            var startMoving = new MovementPacket(WorldCommand.MSG_MOVE_START_TURN_RIGHT)
            {
                GUID = Exchange.authClient.Player.GUID,
                flags = (MovementFlags)Flags.MoveFlags,
                flags2 = (MovementFlags2)Flags.MoveFlags2,
                X = o.x,
                Y = o.z,
                Z = o.y,
                O = Orientation
            };

            Exchange.authClient.SendPacket(startMoving);

            Flags.Clear();
            Flags.Clear2();
        }

        public void SendMoveStop(Vector3 o, Quaternion i)
        {
            var Orientation = -(ConvertToRadians(i.eulerAngles.y)) - 4.6f;

            var startMoving = new MovementPacket(WorldCommand.MSG_MOVE_STOP)
            {
                GUID = Exchange.authClient.Player.GUID,
                flags = (MovementFlags)Flags.MoveFlags,
                flags2 = (MovementFlags2)Flags.MoveFlags2,
                X = o.x,
                Y = o.z,
                Z = o.y,
                O = Orientation
            };
            Exchange.authClient.SendPacket(startMoving);

            Flags.Clear();
            Flags.Clear2();
        }

        public void SendMoveJump(Vector3 o, Quaternion i)
        {
            var Orientation = -(ConvertToRadians(i.eulerAngles.y)) - 4.6f;

            var startMoving = new MovementPacket(WorldCommand.MSG_MOVE_JUMP)
            {
                GUID = Exchange.authClient.Player.GUID,
                flags = (MovementFlags)Flags.MoveFlags,
                flags2 = (MovementFlags2)Flags.MoveFlags2,
                X = o.x,
                Y = o.z,
                Z = o.y,
                O = Orientation
            };
            Exchange.authClient.SendPacket(startMoving);

            Flags.Clear();
            Flags.Clear2();
        }

        public void MoveForward(Vector3 o, UnityEngine.Quaternion h)
        {
            var Orientation = -(ConvertToRadians(h.eulerAngles.y)) - 4.6f;

            var startMoving = new MovementPacket(WorldCommand.MSG_MOVE_START_FORWARD)
            {
                GUID = Exchange.authClient.Player.GUID,
                flags = (MovementFlags)Flags.MoveFlags,
                flags2 = (MovementFlags2)Flags.MoveFlags2,
                X = o.x,
                Y = o.z,
                Z = o.y,
                O = Orientation
            };
            Exchange.authClient.SendPacket(startMoving);

            Flags.Clear();
            Flags.Clear2();
        }
        public void SetFacing(Vector3 o, Quaternion i)
        {
            var Orientation = -(ConvertToRadians(i.eulerAngles.y)) - 4.6f;

            var startMoving = new MovementPacket(WorldCommand.MSG_MOVE_SET_FACING)
            {
                GUID = Exchange.authClient.Player.GUID,
                flags = (MovementFlags)Flags.MoveFlags,
                flags2 = (MovementFlags2)Flags.MoveFlags2,
                X = o.x,
                Y = o.z,
                Z = o.y,
                O = Orientation
            };
            Exchange.authClient.SendPacket(startMoving);

            Flags.Clear();
            Flags.Clear2();
        }

        public uint MM_GetTime()
        {
            return (uint)(DateTime.Now - Process.GetCurrentProcess().StartTime).TotalMilliseconds;
        }

        public void Start()
        {
            try
            {
                Flags.SetMoveFlag(MovementFlags.MOVEMENTFLAG_NONE);
                lastUpdateTime = MM_GetTime();
            }
            catch (Exception ex)
            {

            }
        }

        public class MovementFlag
        {
            public MovementFlags MoveFlags;

            public void Clear()
            {
                MoveFlags = new MovementFlags();
            }

            public void SetMoveFlag(MovementFlags flag)
            {
                MoveFlags |= flag;
            }
            public void UnSetMoveFlag(MovementFlags flag)
            {
                MoveFlags &= ~flag;
            }
            public bool IsMoveFlagSet(MovementFlags flag)
            {
                return ((MoveFlags & flag) >= (MovementFlags)1) ? true : false;
            }

            public MovementFlags2 MoveFlags2;

            public void Clear2()
            {
                MoveFlags2 = new MovementFlags2();
            }

            public void SetMoveFlag2(MovementFlags2 flag2)
            {
                MoveFlags2 |= flag2;
            }
            public void UnSetMoveFlag2(MovementFlags2 flag2)
            {
                MoveFlags2 &= ~flag2;
            }
            public bool IsMoveFlagSet2(MovementFlags2 flag2)
            {
                return ((MoveFlags2 & flag2) >= (MovementFlags2)1) ? true : false;
            }
        }
    }
}