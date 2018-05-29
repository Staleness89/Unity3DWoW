using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.Authentication;
using Client.World;
using Client.Chat;
using Client.World.Network;
using Client.Authentication.Network;
using System.Threading;
using Client.Chat.Definitions;
using Client.World.Definitions;
using System.Collections;
using Client.AI;
using Assets.Scripts.Shared;
using UnityEngine;
using System.Diagnostics;
using Assets.Scripts.Client.World.Movement;

namespace Client
{
    public class AutomatedGame : IGame
    {
        #region Properties
        public bool Running { get; set; }
        public MovementMgr movementMgr = null;
        GameSocket socket;
        public WorldHelper ThreadHelper;
        public BigInteger Key { get; private set; }
        public string Hostname { get; private set; }
        public int Port { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public static bool LoggedIn = false;
        public WorldServerInfo[] RealmServerList { get; set; }
        public bool Connected { get; private set; }
        public string LastSentPacket
        {
            get
            {
                return socket.LastOutOpcodeName;
            }
        }
        public string LastReceivedPacket
        {
            get
            {
                return socket.LastInOpcodeName;
            }
        }
        public DateTime LastUpdate
        {
            get;
            private set;
        }

        ScheduledActions scheduledActions;
        ActionFlag disabledActions;
        int scheduledActionCounter;
        public GameWorld World
        {
            get;
            private set;
        }
        public Player Player
        {
            get;
            set;
        }

        UpdateObjectHandler updateObjectHandler;

        Stack<IStrategicAI> StrategicAIs;
        Stack<ITacticalAI> TacticalAIs;
        Stack<IOperationalAI> OperationalAIs;

        public Dictionary<ulong, WorldObject> Objects
        {
            get;
            private set;
        }

        public Dictionary<uint, Factions> FactionList
        {
            get;
            private set;
        }

        protected HashSet<uint> CompletedAchievements
        {
            get;
            private set;
        }
        protected Dictionary<uint, ulong> AchievementCriterias
        {
            get;
            private set;
        }
        protected bool HasExploreCriteria(uint criteriaId)
        {
            ulong counter;
            if (AchievementCriterias.TryGetValue(criteriaId, out counter))
                return counter > 0;
            return false;
        }

        public UInt64 GroupLeaderGuid { get; private set; }
        public List<UInt64> GroupMembersGuids = new List<UInt64>();
        #endregion

        public AutomatedGame(string hostname, int port, string username, string password)
        {
            FactionList = new Dictionary<uint, Factions>();
            movementMgr = new MovementMgr();
            ThreadHelper = new WorldHelper();
            scheduledActions = new ScheduledActions();
            updateObjectHandler = new UpdateObjectHandler(this);
            Triggers = new IteratedList<Trigger>();
            World = new GameWorld();
            Player = new Player();
            Player.OnFieldUpdated += OnFieldUpdate;
            Objects = new Dictionary<ulong, WorldObject>();
            CompletedAchievements = new HashSet<uint>();
            AchievementCriterias = new Dictionary<uint, ulong>();
            StrategicAIs = new Stack<IStrategicAI>();
            TacticalAIs = new Stack<ITacticalAI>();
            OperationalAIs = new Stack<IOperationalAI>();
            PushStrategicAI(new EmptyStrategicAI());
            PushTacticalAI(new EmptyTacticalAI());
            PushOperationalAI(new EmptyOperationalAI());

            this.Hostname = hostname;
            this.Port = port;
            this.Username = username;
            this.Password = password;

            socket = new AuthSocket(this, Hostname, Port, Username, Password);
            socket.InitHandlers();
        }

        #region Basic Methods
        public void ConnectTo(WorldServerInfo server)
        {
            if (socket is AuthSocket)
                Key = ((AuthSocket)socket).Key;

            socket.Dispose();

            socket = new WorldSocket(this, server);
            socket.InitHandlers();

            if (socket.Connect())
            {
                socket.Start();
                Connected = true;
                Exchange.AuthMessage = "Connected";
            }
            // else
            //Reconnect();
        }

        public virtual void Start()
        {
            // the initial socket is an AuthSocket - it will initiate its own asynch read
            Running = socket.Connect();

        }

        public void Update()
        {
            LastUpdate = DateTime.Now;

            (socket as WorldSocket)?.HandlePackets();

            if (World.SelectedCharacter == null)
                return;

            StrategicAIs.Peek().Update();
            TacticalAIs.Peek().Update();
            OperationalAIs.Peek().Update();

            while (scheduledActions.Count != 0)
            {
                var scheduledAction = scheduledActions.First();
                if (scheduledAction.ScheduledTime <= DateTime.Now)
                {
                    scheduledActions.RemoveAt(0, false);
                    if (scheduledAction.Interval > TimeSpan.Zero)
                        ScheduleAction(scheduledAction.Action, DateTime.Now + scheduledAction.Interval, scheduledAction.Interval, scheduledAction.Flags, scheduledAction.Cancel);
                    try
                    {
                        scheduledAction.Action();
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else
                    break;
            }
        }

        public void Reconnect()
        {
            /*Connected = false;
            LoggedIn = false;
            while (Running)
            {
                socket.Disconnect();
                scheduledActions.Clear();
                ResetTriggers();
                socket = new AuthSocket(this, Hostname, Port, Username, Password);
                socket.InitHandlers();
                // exit from loop if the socket connected successfully
                if (socket.Connect())
                    break;

                // try again later
                Thread.Sleep(10000);
            }*/
        }

        public void Exit()
        {
            ClearTriggers();
            ClearAIs();
            if (LoggedIn)
            {
                OutPacket logout = new OutPacket(WorldCommand.CMSG_LOGOUT_REQUEST);
                SendPacket(logout);
            }
            else
            {
                Connected = false;
                LoggedIn = false;
                Running = false;
            }
            Exchange.authClient = null;
        }

        public void SendPacket(OutPacket packet)
        {
            if (socket is WorldSocket)
            {
                ((WorldSocket)socket).Send(packet);
                HandleTriggerInput(TriggerActionType.Opcode, packet);
            }
        }

        public int ScheduleAction(Action action, TimeSpan interval = default(TimeSpan), ActionFlag flags = ActionFlag.None, Action cancel = null)
        {
            return ScheduleAction(action, DateTime.Now, interval, flags, cancel);
        }

        public int ScheduleAction(Action action, DateTime time, TimeSpan interval = default(TimeSpan), ActionFlag flags = ActionFlag.None, Action cancel = null)
        {
            if (Running && (flags == ActionFlag.None || !disabledActions.HasFlag(flags)))
            {
                scheduledActionCounter++;
                scheduledActions.Add(new RepeatingAction(action, cancel, time, interval, flags, scheduledActionCounter));
                return scheduledActionCounter;
            }
            else
                return 0;
        }

        public void CancelActionsByFlag(ActionFlag flag, bool cancel = true)
        {
            scheduledActions.RemoveByFlag(flag, cancel);
        }

        public bool CancelAction(int actionId)
        {
            return scheduledActions.Remove(actionId);
        }

        public void DisableActionsByFlag(ActionFlag flag)
        {
            disabledActions |= flag;
            CancelActionsByFlag(flag);
        }

        public void EnableActionsByFlag(ActionFlag flag)
        {
            disabledActions &= ~flag;
        }

        public void CreateCharacter(Race race, Class classWow)
        {
            OutPacket createCharacterPacket = new OutPacket(WorldCommand.CMSG_CHAR_CREATE);
            StringBuilder charName = new StringBuilder("Bot");
            foreach (char c in Username.Substring(3).Take(9))
            {
                charName.Append((char)(97 + int.Parse(c.ToString())));
            }

            // Ensure Name rules are applied
            char previousChar = '\0';
            for (int i = 0; i < charName.Length; i++)
            {
                if (charName[i] == previousChar)
                    charName[i]++;
                previousChar = charName[i];
            }

            createCharacterPacket.Write(charName.ToString().ToCString());
            createCharacterPacket.Write((byte)race);
            createCharacterPacket.Write((byte)classWow);
            createCharacterPacket.Write((byte)Gender.Male);
            byte skin = 6; createCharacterPacket.Write(skin);
            byte face = 5; createCharacterPacket.Write(face);
            byte hairStyle = 0; createCharacterPacket.Write(hairStyle);
            byte hairColor = 1; createCharacterPacket.Write(hairColor);
            byte facialHair = 5; createCharacterPacket.Write(facialHair);
            byte outfitId = 0; createCharacterPacket.Write(outfitId);

            SendPacket(createCharacterPacket);
        }

        public void Dispose()
        {
            scheduledActions.Clear();

            Exit();

            if (socket != null)
            {
                socket.Dispose();
            }
        }

        public virtual void NoCharactersFound()
        { }

        public virtual void InvalidCredentials()
        { }


        public string GetPlayerName(WorldObject obj)
        {
            return null;// GetPlayerName(obj.GUID);
        }

        public bool PushStrategicAI(IStrategicAI ai) => PushAI(ai, StrategicAIs);
        public bool PushTacticalAI(ITacticalAI ai) => PushAI(ai, TacticalAIs);
        public bool PushOperationalAI(IOperationalAI ai) => PushAI(ai, OperationalAIs);
        bool PushAI<T>(T ai, Stack<T> AIs) where T : IGameAI
        {
            if (AIs.Count == 0)
            {
                AIs.Push(ai);
                if (ai.Activate(this))
                    return true;
                else
                {
                    AIs.Pop();
                    return false;
                }
            }

            var currentAI = AIs.Peek();
            if (currentAI.AllowPause())
            {
                if (ai.GetType() == currentAI.GetType())
                    return false;
                else
                {
                    currentAI.Pause();
                    AIs.Push(ai);
                    if (ai.Activate(this))
                        return true;
                    else
                    {
                        AIs.Pop();
                        currentAI.Resume();
                        return false;
                    }
                }
            }
            else
                return false;
        }

        public bool PopStrategicAI(IStrategicAI ai) => PopAI(ai, StrategicAIs);
        public bool PopTacticalAI(ITacticalAI ai) => PopAI(ai, TacticalAIs);
        public bool PopOperationalAI(IOperationalAI ai) => PopAI(ai, OperationalAIs);
        public bool PopAI<T>(T ai, Stack<T> AIs) where T : class, IGameAI
        {
            if (AIs.Count <= 1)
                return false;

            var currentAI = AIs.Peek();
            if (currentAI != ai)
                return false;

            currentAI.Deactivate();
            AIs.Pop();

            AIs.Peek().Resume();
            return true;
        }

        public void ClearAIs()
        {
            while (StrategicAIs.Count > 1)
            {
                var currentAI = StrategicAIs.Pop();
                currentAI.Deactivate();
            }

            while (TacticalAIs.Count > 1)
            {
                var currentAI = TacticalAIs.Pop();
                currentAI.Deactivate();
            }

            while (OperationalAIs.Count > 1)
            {
                var currentAI = OperationalAIs.Pop();
                currentAI.Deactivate();
            }
        }
        #endregion

        #region Commands
        public void DoSayChat(string message)
        {
            var response = new OutPacket(WorldCommand.CMSG_MESSAGECHAT);

            response.Write((uint)ChatMessageType.Say);
            var race = World.SelectedCharacter.Race;
            var language = race.IsHorde() ? Language.Orcish : Language.Common;
            response.Write((uint)language);
            response.Write(message.ToCString());
            SendPacket(response);
        }

        public void DoPartyChat(string message)
        {
            var response = new OutPacket(WorldCommand.CMSG_MESSAGECHAT);

            response.Write((uint)ChatMessageType.Party);
            var race = World.SelectedCharacter.Race;
            var language = race.IsHorde() ? Language.Orcish : Language.Common;
            response.Write((uint)language);
            response.Write(message.ToCString());
            SendPacket(response);
        }

        public void DoGuildChat(string message)
        {
            var response = new OutPacket(WorldCommand.CMSG_MESSAGECHAT);

            response.Write((uint)ChatMessageType.Guild);
            var race = World.SelectedCharacter.Race;
            var language = race.IsHorde() ? Language.Orcish : Language.Common;
            response.Write((uint)language);
            response.Write(message.ToCString());
            SendPacket(response);
        }

        public void DoWhisperChat(string message, string player)
        {
            var response = new OutPacket(WorldCommand.CMSG_MESSAGECHAT);

            response.Write((uint)ChatMessageType.Whisper);
            var race = World.SelectedCharacter.Race;
            var language = race.IsHorde() ? Language.Orcish : Language.Common;
            response.Write((uint)language);
            response.Write(player.ToCString());
            response.Write(message.ToCString());
            SendPacket(response);
        }

        public void Tele(string teleport)
        {
            DoSayChat(".tele " + teleport);
        }

        public void CastSpell(int spellid, bool chatLog = true)
        {
            DoSayChat(".cast " + spellid);
            if (chatLog)
                DoSayChat("Casted spellid " + spellid);
        }

        #endregion

        #region Actions
        public void DoTextEmote(TextEmote emote)
        {
            var packet = new OutPacket(WorldCommand.CMSG_TEXT_EMOTE);
            packet.Write((uint)emote);
            packet.Write((uint)0);
            packet.Write((ulong)0);
            SendPacket(packet);
        }
        
        public void Follow(WorldObject target)
        {
           /*

            var startMoving = new MovementPacket(WorldCommand.MSG_MOVE_START_FORWARD)
            {
                GUID = Player.GUID,
                flags = MovementFlags.MOVEMENTFLAG_FORWARD,
                X = Player.X,
                Y = Player.Y,
                Z = Player.Z,
                O = path.CurrentOrientation
            };
            SendPacket(startMoving);

            previousMovingTime = DateTime.Now;
            return;
        }

        Point progressPosition = path.MoveAlongPath((float)(DateTime.Now - previousMovingTime).TotalSeconds);
        Player.SetPosition(progressPosition.X, progressPosition.Y, progressPosition.Z);
                previousMovingTime = DateTime.Now;

                var heartbeat = new MovementPacket(WorldCommand.MSG_MOVE_HEARTBEAT)
                {
                    GUID = Player.GUID,
                    flags = MovementFlags.MOVEMENTFLAG_FORWARD,
                    X = Player.X,
                    Y = Player.Y,
                    Z = Player.Z,
                    O = path.CurrentOrientation
                };
                SendPacket(heartbeat);*/
    }
        #endregion

        #region Packet Handlers
        [PacketHandler(WorldCommand.SMSG_LOGIN_VERIFY_WORLD)]
        protected void HandleLoginVerifyWorld(InPacket packet)
        {
            Player.Map = packet.ReadUInt32();
            Player.Movement.Position.x = packet.ReadSingle();
            Player.Movement.Position.y = packet.ReadSingle();
            Player.Movement.Position.z = packet.ReadSingle();
            Player.Movement.Orientation = packet.ReadSingle();
        }

        [PacketHandler(WorldCommand.SMSG_NEW_WORLD)]
        protected void HandleNewWorld(InPacket packet)
        {
            Player.Map = packet.ReadUInt32();
            Player.Movement.Position.x = packet.ReadSingle();
            Player.Movement.Position.y = packet.ReadSingle();
            Player.Movement.Position.z = packet.ReadSingle();
            Player.Movement.Orientation = packet.ReadSingle();

            OutPacket result = new OutPacket(WorldCommand.MSG_MOVE_WORLDPORT_ACK);
            SendPacket(result);
        }

        [PacketHandler(WorldCommand.SMSG_TRANSFER_PENDING)]
        protected void HandleTransferPending(InPacket packet)
        {
            // Player.ResetPosition();
            var newMap = packet.ReadUInt32();
        }

        [PacketHandler(WorldCommand.MSG_MOVE_TELEPORT_ACK)]
        protected void HandleMoveTeleportAck(InPacket packet)
        {
            var packGuid = packet.ReadPackedGuid();
            packet.ReadUInt32();
            var movementFlags = packet.ReadUInt32();
            var extraMovementFlags = packet.ReadUInt16();
            var time = packet.ReadUInt32();
            Player.Movement.Position.x = packet.ReadSingle();
            Player.Movement.Position.y = packet.ReadSingle();
            Player.Movement.Position.z = packet.ReadSingle();
            Player.Movement.Orientation = packet.ReadSingle();

            CancelActionsByFlag(ActionFlag.Movement, false);

            OutPacket result = new OutPacket(WorldCommand.MSG_MOVE_TELEPORT_ACK);
            result.WritePacketGuid(Player.GUID);
            result.Write((UInt32)0);
            result.Write(time);
            SendPacket(result);
        }

        [PacketHandler(WorldCommand.SMSG_CHAR_CREATE)]
        protected void HandleCharCreate(InPacket packet)
        {
            var response = (CommandDetail)packet.ReadByte();
            if (response == CommandDetail.CHAR_CREATE_SUCCESS)
                SendPacket(new OutPacket(WorldCommand.CMSG_CHAR_ENUM));
            else
                NoCharactersFound();
        }

        [PacketHandler(WorldCommand.SMSG_LOGOUT_RESPONSE)]
        protected void HandleLogoutResponse(InPacket packet)
        {
            bool logoutOk = packet.ReadUInt32() == 0;
            bool instant = packet.ReadByte() != 0;

            if (instant || !logoutOk)
            {
                Connected = false;
                LoggedIn = false;
                Running = false;
            }
        }

        [PacketHandler(WorldCommand.SMSG_LOGOUT_COMPLETE)]
        protected void HandleLogoutComplete(InPacket packet)
        {
            Connected = false;
            LoggedIn = false;
            Running = false;
        }

        [PacketHandler(WorldCommand.SMSG_UPDATE_OBJECT)]
        protected void HandleUpdateObject(InPacket packet)
        {
            updateObjectHandler.HandleUpdatePacket(packet);
        }

        [PacketHandler(WorldCommand.SMSG_COMPRESSED_UPDATE_OBJECT)]
        protected void HandleCompressedUpdateObject(InPacket packet)
        {
            updateObjectHandler.HandleUpdatePacket(packet.Inflate());
        }

        [PacketHandler(WorldCommand.SMSG_MONSTER_MOVE)]
        protected void HandleMonsterMove(InPacket packet)
        {
            updateObjectHandler.HandleMonsterMovementPacket(packet);
        }

        [PacketHandler(WorldCommand.MSG_MOVE_START_FORWARD)]
        [PacketHandler(WorldCommand.MSG_MOVE_START_BACKWARD)]
        [PacketHandler(WorldCommand.MSG_MOVE_STOP)]
        [PacketHandler(WorldCommand.MSG_MOVE_START_STRAFE_LEFT)]
        [PacketHandler(WorldCommand.MSG_MOVE_START_STRAFE_RIGHT)]
        [PacketHandler(WorldCommand.MSG_MOVE_STOP_STRAFE)]
        [PacketHandler(WorldCommand.MSG_MOVE_JUMP)]
        [PacketHandler(WorldCommand.MSG_MOVE_START_TURN_LEFT)]
        [PacketHandler(WorldCommand.MSG_MOVE_START_TURN_RIGHT)]
        [PacketHandler(WorldCommand.MSG_MOVE_STOP_TURN)]
        [PacketHandler(WorldCommand.MSG_MOVE_START_PITCH_UP)]
        [PacketHandler(WorldCommand.MSG_MOVE_START_PITCH_DOWN)]
        [PacketHandler(WorldCommand.MSG_MOVE_STOP_PITCH)]
        [PacketHandler(WorldCommand.MSG_MOVE_SET_RUN_MODE)]
        [PacketHandler(WorldCommand.MSG_MOVE_SET_WALK_MODE)]
        [PacketHandler(WorldCommand.MSG_MOVE_FALL_LAND)]
        [PacketHandler(WorldCommand.MSG_MOVE_START_SWIM)]
        [PacketHandler(WorldCommand.MSG_MOVE_STOP_SWIM)]
        [PacketHandler(WorldCommand.MSG_MOVE_SET_FACING)]
        [PacketHandler(WorldCommand.MSG_MOVE_SET_PITCH)]
        [PacketHandler(WorldCommand.MSG_MOVE_HEARTBEAT)]
        [PacketHandler(WorldCommand.MSG_MOVE_START_ASCEND)]
        [PacketHandler(WorldCommand.MSG_MOVE_STOP_ASCEND)]
        [PacketHandler(WorldCommand.MSG_MOVE_START_DESCEND)]
        protected void HandleMove(InPacket packet)
        {
            updateObjectHandler.HandleMovementPacket(packet);
        }

        class UpdateObjectHandler
        {
            AutomatedGame game;

            uint blockCount;
            ObjectUpdateType updateType;
            ulong guid;
            ObjectType objectType;
            ObjectUpdateFlags flags;
            MovementInfo movementInfo;
            Dictionary<UnitMoveType, float> movementSpeeds;
            SplineFlag splineFlags;
            float splineFacingAngle;
            ulong splineFacingTargetGuid;
            Vector3 splineFacingPointX;
            int splineTimePassed;
            int splineDuration;
            uint splineId;
            float splineVerticalAcceleration;
            int splineEffectStartTime;
            List<Vector3> splinePoints;
            SplineEvaluationMode splineEvaluationMode;
            Vector3 splineEndPoint;
            Vector3 transportOffset;
            float o;
            float corpseOrientation;

            uint lowGuid;
            ulong targetGuid;
            uint transportTimer;
            uint vehicledID;
            float vehicleOrientation;
            long goRotation;

            Dictionary<int, uint> updateFields;

            List<ulong> outOfRangeGuids;

            public UpdateObjectHandler(AutomatedGame game)
            {
                this.game = game;
                movementSpeeds = new Dictionary<UnitMoveType, float>();
                splinePoints = new List<Vector3>();
                updateFields = new Dictionary<int, uint>();
                outOfRangeGuids = new List<ulong>();
            }

            private void HandleUpdateData()
            {
                if (guid == game.Player.GUID)
                {
                    foreach (var pair in updateFields)
                        game.Player[pair.Key] = pair.Value;
                }
                switch (updateType)
                {
                    case ObjectUpdateType.UPDATETYPE_VALUES:
                        {
                            WorldObject worldObject = game.Objects[guid];
                            foreach (var pair in updateFields)
                                worldObject[pair.Key] = pair.Value;
                            break;
                        }
                    case ObjectUpdateType.UPDATETYPE_MOVEMENT:
                        {
                            if (movementInfo != null)
                            {
                                WorldObject worldObject = game.Objects[guid];
                                worldObject.Movement.Position = (movementInfo.Position);
                                worldObject.Movement.Orientation = movementInfo.Orientation;
                            }
                            break;
                        }
                    case ObjectUpdateType.UPDATETYPE_CREATE_OBJECT:
                    case ObjectUpdateType.UPDATETYPE_CREATE_OBJECT2:
                        {
                            WorldObject worldObject_Create;

                            switch (objectType)
                            {
                                case ObjectType.Unit: worldObject_Create = new Unit(); break;
                                case ObjectType.GameObject: worldObject_Create = new GameObject(); break;
                                case ObjectType.Item: worldObject_Create = new Item(); break;
                                case ObjectType.Player: worldObject_Create = new Player(); break;
                                //case ObjectType.AreaTrigger: worldObject_Create = new SpellAreaTrigger(); break;
                                default: worldObject_Create = new WorldObject(); break;
                            }

                            worldObject_Create.GUID = guid;
                            worldObject_Create.Type = objectType;

                            if (movementInfo != null)
                            {
                                worldObject_Create.Movement = movementInfo;
                            }
                            worldObject_Create.Map = game.Player.Map;
                            worldObject_Create.Area = game.Player.Area;
                            worldObject_Create.Zone = game.Player.Zone;

                            foreach (var pair in updateFields)
                                worldObject_Create[pair.Key] = pair.Value;

                            game.Objects.Add(guid, worldObject_Create);

                            if (objectType == ObjectType.GameObject)
                            {
                                game.ThreadHelper.AddToWorld(worldObject_Create);
                                worldObject_Create = new WorldObject();
                                OutPacket nameQuery = new OutPacket(WorldCommand.CMSG_GAMEOBJECT_QUERY);
                                nameQuery.Write(game.Objects[guid].ObjectEntry);
                                nameQuery.Write(guid);
                                game.SendPacket(nameQuery);
                            }

                            if (objectType == ObjectType.Item)
                            {
                                worldObject_Create = new WorldObject();
                                OutPacket nameQuery = new OutPacket(WorldCommand.CMSG_ITEM_QUERY_SINGLE);
                                nameQuery.Write(game.Objects[guid].ObjectEntry);
                                game.SendPacket(nameQuery);
                            }

                            if (objectType == ObjectType.Unit)
                            {
                                game.ThreadHelper.AddToWorld(worldObject_Create);
                                OutPacket CMSG_CREATURE_QUERY = new OutPacket(WorldCommand.CMSG_CREATURE_QUERY);
                                CMSG_CREATURE_QUERY.Write(game.Objects[guid].ObjectEntry);
                                CMSG_CREATURE_QUERY.Write(guid);
                                game.SendPacket(CMSG_CREATURE_QUERY);
                            }
                            if (objectType == ObjectType.Player)
                            {
                                game.ThreadHelper.AddToWorld(worldObject_Create);
                                OutPacket nameQuery = new OutPacket(WorldCommand.CMSG_NAME_QUERY);
                                nameQuery.Write(guid);
                                game.SendPacket(nameQuery);
                            }
                            break;
                        }
                    default:
                        break;
                }

                foreach (var outOfRangeGuid in outOfRangeGuids)
                {
                    WorldObject worldObject;
                    if (game.Objects.TryGetValue(outOfRangeGuid, out worldObject))
                    {
                        game.ThreadHelper.RemoveFromWorld(game.Objects[guid]);
                        game.Objects.Remove(outOfRangeGuid);
                    }
                }
            }



            public void HandleUpdatePacket(InPacket packet)
            {
                blockCount = packet.ReadUInt32();
                for (int blockIndex = 0; blockIndex < blockCount; blockIndex++)
                {
                    ResetData();

                    updateType = (ObjectUpdateType)packet.ReadByte();

                    switch (updateType)
                    {
                        case ObjectUpdateType.UPDATETYPE_VALUES:
                            guid = packet.ReadPackedGuid();
                            ReadValuesUpdateData(packet);

                            WorldObject worldObject_Update = game.Objects[guid];
                            foreach (var pair in updateFields)
                                worldObject_Update[pair.Key] = pair.Value;

                            break;
                        case ObjectUpdateType.UPDATETYPE_MOVEMENT:
                            guid = packet.ReadPackedGuid();

                            movementInfo = ReadMovementUpdateData(packet);

                            if (movementInfo != null)
                            {
                                WorldObject worldObject_Movement = game.Objects[guid];
                                worldObject_Movement.Movement.Position = movementInfo.Position;
                                worldObject_Movement.Movement.Orientation = movementInfo.Orientation;
                            }

                            break;
                        case ObjectUpdateType.UPDATETYPE_CREATE_OBJECT:
                        case ObjectUpdateType.UPDATETYPE_CREATE_OBJECT2:
                            guid = packet.ReadPackedGuid();
                            objectType = (ObjectType)packet.ReadByte();

                            movementInfo = ReadMovementUpdateData(packet);
                            ReadValuesUpdateData(packet);


                            WorldObject worldObject_Create;

                            switch (objectType)
                            {
                                case ObjectType.Unit: worldObject_Create = new Unit(); break;
                                case ObjectType.GameObject: worldObject_Create = new GameObject(); break;
                                case ObjectType.Item: worldObject_Create = new Item(); break;
                                case ObjectType.Player: worldObject_Create = new Player(); break;
                                //case ObjectType.AreaTrigger: worldObject_Create = new SpellAreaTrigger(); break;
                                default: worldObject_Create = new WorldObject(); break;
                            }

                            worldObject_Create.GUID = guid;
                            worldObject_Create.Type = objectType;

                            if (movementInfo != null)
                            {
                                worldObject_Create.Movement = movementInfo;
                            }

                            worldObject_Create.Map = game.Player.Map;
                            worldObject_Create.Area = game.Player.Area;
                            worldObject_Create.Zone = game.Player.Zone;

                            foreach (var pair in updateFields)
                                worldObject_Create[pair.Key] = pair.Value;

                            if (worldObject_Create is Unit)
                            {
                                worldObject_Create.Movement.Speed = movementInfo.WalkSpeed;
                            }
                            else
                            {
                                worldObject_Create.Movement.Speed = movementInfo.RunSpeed;
                            }

                            game.Objects.Add(guid, worldObject_Create);

                            if (guid == Exchange.SelectedCharacter.GUID)
                            {
                                Exchange.authClient.Player = worldObject_Create as Player;
                            }

                            if (objectType == ObjectType.GameObject)
                            {
                                Exchange.authClient.ThreadHelper.AddToWorld(worldObject_Create);
                                worldObject_Create = new WorldObject();
                                OutPacket nameQuery = new OutPacket(WorldCommand.CMSG_GAMEOBJECT_QUERY);
                                nameQuery.Write(game.Objects[guid].ObjectEntry);
                                nameQuery.Write(guid);
                                game.SendPacket(nameQuery);
                            }

                            if (objectType == ObjectType.Item)
                            {
                                OutPacket nameQuery = new OutPacket(WorldCommand.CMSG_ITEM_QUERY_SINGLE);
                                nameQuery.Write(game.Objects[guid].ObjectEntry);
                                game.SendPacket(nameQuery);
                            }

                            if (objectType == ObjectType.Unit)
                            {
                                Exchange.authClient.ThreadHelper.AddToWorld(worldObject_Create);
                                OutPacket CMSG_CREATURE_QUERY = new OutPacket(WorldCommand.CMSG_CREATURE_QUERY);
                                CMSG_CREATURE_QUERY.Write(game.Objects[guid].ObjectEntry);
                                CMSG_CREATURE_QUERY.Write(guid);
                                game.SendPacket(CMSG_CREATURE_QUERY);
                            }
                            if (objectType == ObjectType.Player)
                            {
                                Exchange.authClient.ThreadHelper.AddToWorld(worldObject_Create);
                                OutPacket nameQuery = new OutPacket(WorldCommand.CMSG_NAME_QUERY);
                                nameQuery.Write(guid);
                                game.SendPacket(nameQuery);
                            }
                            break;
                        case ObjectUpdateType.UPDATETYPE_OUT_OF_RANGE_OBJECTS:
                            var guidCount = packet.ReadUInt32();
                            for (var guidIndex = 0; guidIndex < guidCount; guidIndex++)
                                outOfRangeGuids.Add(packet.ReadPackedGuid());
                            break;
                        case ObjectUpdateType.UPDATETYPE_NEAR_OBJECTS:
                            break;
                    }

                    foreach (var outOfRangeGuid in outOfRangeGuids)
                    {
                        WorldObject worldObject;
                        if (game.Objects.TryGetValue(outOfRangeGuid, out worldObject))
                        {
                            game.ThreadHelper.RemoveFromWorld(game.Objects[guid]);
                            game.Objects.Remove(outOfRangeGuid);
                        }
                    }
                }
            }

            public void HandleMovementPacket(InPacket packet)
            {
                ResetData();
                updateType = ObjectUpdateType.UPDATETYPE_MOVEMENT;
                guid = packet.ReadPackedGuid();
                movementInfo = ReadMovementInfo(packet);

                HandleUpdateData();
            }

            public void HandleMonsterMovementPacket(InPacket packet)
            {
                guid = packet.ReadPackedGuid();
                packet.ReadBoolean(); // Move Ticks
                var pos = packet.ReadVector3();
                packet.ReadInt32();

                SplineType type = (SplineType)packet.ReadByte(); //Spline Type

                switch (type)
                {
                    case SplineType.FacingSpot:
                        {
                            packet.ReadVector3(); //Facing Spot
                            break;
                        }
                    case SplineType.FacingTarget:
                        {
                            var tarGUID = packet.ReadUInt64(); //Facing GUID
                            break;
                        }
                    case SplineType.FacingAngle:
                        {
                            packet.ReadSingle(); //Facing Angle
                            break;
                        }
                    case SplineType.Stop:
                        return;
                }

                SplineFlag flags = (SplineFlag)packet.ReadInt32(); //Spline Flags

                if (flags.HasAnyFlag(SplineFlag.AnimationTier))
                {
                    packet.ReadByte(); //Animation State
                    packet.ReadInt32(); //Async-time in ms
                }

                packet.ReadInt32(); //Move Time

                if (flags.HasAnyFlag(SplineFlag.Trajectory))
                {
                    packet.ReadSingle(); //Vertical Speed
                    packet.ReadInt32(); //Async-time in ms
                }

                var waypoints = packet.ReadInt32(); //Waypoints

                if (flags.HasAnyFlag(SplineFlag.Flying | SplineFlag.CatmullRom))
                {
                    for (var i = 0; i < waypoints; i++)
                        packet.ReadVector3(); //Waypoint
                }
                else
                {
                    var newpos = packet.ReadVector3(); //Waypoint Endpoint
                    game.Objects[guid].Movement.Position = newpos;
                }
            }

            void ResetData()
            {
                updateType = ObjectUpdateType.UPDATETYPE_VALUES;
                guid = 0;
                lowGuid = 0;
                movementSpeeds.Clear();
                splinePoints.Clear();
                updateFields.Clear();
                outOfRangeGuids.Clear();
                movementInfo = null;
            }

            MovementInfo ReadMovementUpdateData(InPacket packet)
            {
                var moveInfo = new MovementInfo();

                flags = (ObjectUpdateFlags)packet.ReadUInt16();
                if (flags.HasFlag(ObjectUpdateFlags.UPDATEFLAG_LIVING))
                {
                    moveInfo = ReadMovementInfo(packet);

                    for (var i = 0; i < 9; ++i)
                    {
                        var speedType = (UnitMoveType)i;
                        var speed = packet.ReadSingle();

                        switch (speedType)
                        {
                            case UnitMoveType.MOVE_WALK:
                                {
                                    moveInfo.WalkSpeed = speed;
                                    break;
                                }
                            case UnitMoveType.MOVE_RUN:
                                {
                                    moveInfo.RunSpeed = speed;
                                    break;
                                }
                            case UnitMoveType.MOVE_TURN_RATE:
                                {
                                    moveInfo.TurnSpeed = speed;
                                    break;
                                }
                            case UnitMoveType.MOVE_RUN_BACK:
                                {
                                    moveInfo.RunBackSpeed = speed;
                                    break;
                                }
                        }
                    }

                    if (packet.Header.Command == WorldCommand.MSG_MOVE_SET_RUN_MODE)
                        moveInfo.Speed = moveInfo.RunSpeed;

                    if (packet.Header.Command == WorldCommand.MSG_MOVE_SET_WALK_MODE)
                        moveInfo.Speed = moveInfo.WalkSpeed;

                    if (moveInfo.Flags.HasFlag(MovementFlags.MOVEMENTFLAG_SPLINE_ENABLED) || moveInfo.HasSplineData)
                    {
                        splineFlags = (SplineFlag)packet.ReadUInt32();
                        if (splineFlags.HasFlag(SplineFlag.FinalOrientation))
                            splineFacingAngle = packet.ReadSingle();
                        else if (splineFlags.HasFlag(SplineFlag.FinalTarget))
                            splineFacingTargetGuid = packet.ReadUInt64();
                        else if (splineFlags.HasFlag(SplineFlag.FinalPoint))
                            splineFacingPointX = packet.ReadVector3();

                        splineTimePassed = packet.ReadInt32();
                        splineDuration = packet.ReadInt32();
                        splineId = packet.ReadUInt32();
                        packet.ReadSingle();
                        packet.ReadSingle();
                        splineVerticalAcceleration = packet.ReadSingle();
                        splineEffectStartTime = packet.ReadInt32();
                        uint splineCount = packet.ReadUInt32();
                        for (uint index = 0; index < splineCount; index++)
                            splinePoints.Add(packet.ReadVector3());
                        splineEvaluationMode = (SplineEvaluationMode)packet.ReadByte();
                        splineEndPoint = packet.ReadVector3();
                    }
                }
                else
                {
                    if (flags.HasFlag(ObjectUpdateFlags.UPDATEFLAG_POSITION))
                    {
                        moveInfo.TransportGuid = packet.ReadPackedGuid();
                        moveInfo.Position = packet.ReadVector3();
                        transportOffset = packet.ReadVector3();
                        moveInfo.Orientation = packet.ReadSingle();
                        corpseOrientation = packet.ReadSingle();
                    }
                    else if (flags.HasFlag(ObjectUpdateFlags.UPDATEFLAG_STATIONARY_POSITION))
                    {
                        moveInfo.Position = packet.ReadVector3();
                        moveInfo.Orientation = packet.ReadSingle();
                    }
                }

                if (flags.HasFlag(ObjectUpdateFlags.UPDATEFLAG_UNKNOWN))
                    packet.ReadUInt32();

                if (flags.HasFlag(ObjectUpdateFlags.UPDATEFLAG_LOWGUID))
                    lowGuid = packet.ReadUInt32();

                if (flags.HasFlag(ObjectUpdateFlags.UPDATEFLAG_HAS_TARGET))
                    targetGuid = packet.ReadPackedGuid();

                if (flags.HasFlag(ObjectUpdateFlags.UPDATEFLAG_TRANSPORT))
                    transportTimer = packet.ReadUInt32();

                if (flags.HasFlag(ObjectUpdateFlags.UPDATEFLAG_VEHICLE))
                {
                    vehicledID = packet.ReadUInt32();
                    vehicleOrientation = packet.ReadSingle();
                }

                if (flags.HasFlag(ObjectUpdateFlags.UPDATEFLAG_ROTATION))
                    goRotation = packet.ReadInt64();

                return moveInfo;
            }

            MovementInfo ReadMovementInfo(InPacket packet)
            {
                var info = new MovementInfo();

                info.Flags = (MovementFlags)packet.ReadInt32();

                info.FlagsExtra = (MovementFlags2)packet.ReadInt16();

                packet.ReadUInt32();

                info.Position = packet.ReadVector3();
                info.Orientation = packet.ReadSingle();

                if (info.Flags.HasAnyFlag(ObjectUpdateFlags.UPDATEFLAG_TRANSPORT))
                {
                    info.TransportGuid = packet.ReadPackedGuid();
                    info.Position = packet.ReadVector3();
                    info.TransportOffset = packet.ReadVector3();
                    info.TransportO = packet.ReadSingle();
                    packet.ReadInt32();

                    if (info.FlagsExtra.HasAnyFlag(MovementFlags2.MOVEMENTFLAG2_INTERPOLATED_MOVEMENT))
                        packet.ReadInt32();
                }

                if (info.Flags.HasAnyFlag(MovementFlags.MOVEMENTFLAG_SWIMMING | MovementFlags.MOVEMENTFLAG_FLYING) ||
                    info.FlagsExtra.HasAnyFlag(MovementFlags2.MOVEMENTFLAG2_ALWAYS_ALLOW_PITCHING))
                    packet.ReadSingle();


                packet.ReadInt32();
                if (info.Flags.HasAnyFlag(MovementFlags.MOVEMENTFLAG_FALLING))
                {
                    packet.ReadSingle();
                    packet.ReadSingle();
                    packet.ReadSingle();
                    packet.ReadSingle();
                }
                return info;
            }

            private void ReadValuesUpdateData(InPacket packet)
            {
                byte blockCount = packet.ReadByte();
                int[] updateMask = new int[blockCount];
                for (var i = 0; i < blockCount; i++)
                    updateMask[i] = packet.ReadInt32();
                var mask = new BitArray(updateMask);

                for (var i = 0; i < mask.Count; ++i)
                {
                    if (!mask[i])
                        continue;

                    updateFields[i] = packet.ReadUInt32();
                }
            }
        }

        [PacketHandler(WorldCommand.SMSG_DESTROY_OBJECT)]
        protected void HandleDestroyObject(InPacket packet)
        {
            ulong guid = packet.ReadUInt64();
            WorldObject worldObject;
            if (Objects.TryGetValue(guid, out worldObject))
            {
                ThreadHelper.RemoveFromWorld(Objects[guid]);
                Objects.Remove(guid);
            }
        }

        [PacketHandler(WorldCommand.SMSG_ALL_ACHIEVEMENT_DATA)]
        protected void HandleAllAchievementData(InPacket packet)
        {
            CompletedAchievements.Clear();
            AchievementCriterias.Clear();

            for (; ; )
            {
                uint achievementId = packet.ReadUInt32();
                if (achievementId == 0xFFFFFFFF)
                    break;

                packet.ReadPackedTime();

                CompletedAchievements.Add(achievementId);
            }

            for (; ; )
            {
                uint criteriaId = packet.ReadUInt32();
                if (criteriaId == 0xFFFFFFFF)
                    break;
                ulong criteriaCounter = packet.ReadPackedGuid();
                packet.ReadPackedGuid();
                packet.ReadInt32();
                packet.ReadPackedTime();
                packet.ReadInt32();
                packet.ReadInt32();

                AchievementCriterias[criteriaId] = criteriaCounter;
            }
        }

        [PacketHandler(WorldCommand.SMSG_CRITERIA_UPDATE)]
        protected void HandleCriteriaUpdate(InPacket packet)
        {
            uint criteriaId = packet.ReadUInt32();
            ulong criteriaCounter = packet.ReadPackedGuid();

            AchievementCriterias[criteriaId] = criteriaCounter;
        }

        [PacketHandler(WorldCommand.SMSG_GROUP_LIST)]
        protected void HandlePartyList(InPacket packet)
        {
            GroupType groupType = (GroupType)packet.ReadByte();
            packet.ReadByte();
            packet.ReadByte();
            packet.ReadByte();
            if (groupType.HasFlag(GroupType.GROUPTYPE_LFG))
            {
                packet.ReadByte();
                packet.ReadUInt32();
            }
            packet.ReadUInt64();
            packet.ReadUInt32();
            uint membersCount = packet.ReadUInt32();
            GroupMembersGuids.Clear();
            for (uint index = 0; index < membersCount; index++)
            {
                packet.ReadCString();
                UInt64 memberGuid = packet.ReadUInt64();
                GroupMembersGuids.Add(memberGuid);
                packet.ReadByte();
                packet.ReadByte();
                packet.ReadByte();
                packet.ReadByte();
            }
            GroupLeaderGuid = packet.ReadUInt64();
        }

        [PacketHandler(WorldCommand.SMSG_GROUP_DESTROYED)]
        protected void HandlePartyDisband(InPacket packet)
        {
            GroupLeaderGuid = 0;
            GroupMembersGuids.Clear();
        }
        #endregion

        #region Triggers Handling
        IteratedList<Trigger> Triggers;
        int triggerCounter;

        public int AddTrigger(Trigger trigger)
        {
            triggerCounter++;
            trigger.Id = triggerCounter;
            Triggers.Add(trigger);
            return triggerCounter;
        }

        public IEnumerable<int> AddTriggers(IEnumerable<Trigger> triggers)
        {
            var triggerIds = new List<int>();
            foreach (var trigger in triggers)
                triggerIds.Add(AddTrigger(trigger));
            return triggerIds;
        }

        public bool RemoveTrigger(int triggerId)
        {
            return Triggers.RemoveAll(trigger => trigger.Id == triggerId) > 0;
        }

        public void ClearTriggers()
        {
            Triggers.Clear();
        }

        public void ResetTriggers()
        {
            Triggers.ForEach(trigger => trigger.Reset());
        }

        public void HandleTriggerInput(TriggerActionType type, params object[] inputs)
        {
            Triggers.ForEach(trigger => trigger.HandleInput(type, inputs));
        }

        void OnFieldUpdate(object s, UpdateFieldEventArg e)
        {
            HandleTriggerInput(TriggerActionType.UpdateField, e);
        }
        #endregion
    }
}
