using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using Client.Authentication;
using System.IO;
using Assets.Scripts.Shared;
using UnityEngine;

namespace Client.World.Network
{
    public partial class WorldSocket : GameSocket
    {
        WorldServerInfo ServerInfo;

        private long transferred;
        public long Transferred { get { return transferred; } }

        private long sent;
        public long Sent { get { return sent; } }

        private long received;
        public long Received { get { return received; } }

        public override string LastOutOpcodeName
        {
            get
            {
                return LastOutOpcode?.ToString();
            }
        }
        public WorldCommand? LastOutOpcode
        {
            get;
            protected set;
        }
        public override string LastInOpcodeName
        {
            get
            {
                return LastInOpcode?.ToString();
            }
        }
        public WorldCommand? LastInOpcode
        {
            get;
            protected set;
        }

        public WorldSocket(IGame program, WorldServerInfo serverInfo)
        {
            Game = program;
            ServerInfo = serverInfo;
        }

        #region Handler registration

        Dictionary<WorldCommand, PacketHandler> PacketHandlers;

        public override void InitHandlers()
        {
            PacketHandlers = new Dictionary<WorldCommand, PacketHandler>();

            RegisterHandlersFrom(this);
            RegisterHandlersFrom(Game);
        }

        void RegisterHandlersFrom(object obj)
        {
            // create binding flags to discover all non-static methods
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            IEnumerable<PacketHandlerAttribute> attributes;
            foreach (var method in obj.GetType().GetMethods(flags))
            {
                if (!method.TryGetAttributes(false, out attributes))
                    continue;

                PacketHandler handler = (PacketHandler)PacketHandler.CreateDelegate(typeof(PacketHandler), obj, method);

                foreach (var attribute in attributes)
                {
                    PacketHandlers[attribute.Command] = handler;
                }
            }
        }

        #endregion

        #region Asynchronous Reading

        int Index;
        int Remaining;
        
        private void ReadAsync(EventHandler<SocketAsyncEventArgs> callback, object state = null)
        {
            if (Disposing)
                return;

            SocketAsyncState = state;
            SocketArgs.SetBuffer(ReceiveData, Index, Remaining);
            SocketCallback = callback;
            connection.Client.ReceiveAsync(SocketArgs);
        }

        /// <summary>
        /// Determines how large the incoming header will be by
        /// inspecting the first byte, then initiates reading the header.
        /// </summary>
        private void ReadSizeCallback(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                int bytesRead = e.BytesTransferred;
                if (bytesRead == 0)
                {
                    // TODO: world server disconnect
                    Exchange.AuthMessage = "Server has closed the connection";
                    //Game.Reconnect();
                    return;
                }

                Interlocked.Increment(ref transferred);
                Interlocked.Increment(ref received);

                authenticationCrypto.Decrypt(ReceiveData, 0, 1);
                if ((ReceiveData[0] & 0x80) != 0)
                {
                    // need to resize the buffer
                    byte temp = ReceiveData[0];
                    ReserveData(5);
                    ReceiveData[0] = (byte)((0x7f & temp));

                    Remaining = 4;
                }
                else
                    Remaining = 3;

                Index = 1;
                ReadAsync(ReadHeaderCallback);
            }
            // these exceptions can happen as race condition on shutdown
            catch(ObjectDisposedException ex)
            {

            }
            catch(NullReferenceException ex)
            {

            }
            catch(InvalidOperationException ex)
            {

            }
            catch(SocketException ex)
            {
                //Game.Reconnect();
            }
        }

        /// <summary>
        /// Reads the rest of the incoming header.
        /// </summary>
        private void ReadHeaderCallback(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                int bytesRead = e.BytesTransferred;
                if (bytesRead == 0)
                {
                    // TODO: world server disconnect
                    Exchange.AuthMessage = "Server has closed the connection";
                    //Game.Reconnect();
                    return;
                }

                Interlocked.Add(ref transferred, bytesRead);
                Interlocked.Add(ref received, bytesRead);

                if (bytesRead == Remaining)
                {
                    authenticationCrypto.Decrypt(ReceiveData, 1, ReceiveDataLength - 1);
                    ServerHeader header = new ServerHeader(ReceiveData, ReceiveDataLength);
                    
                    if (header.Size > 0)
                    {
                        Index = 0;
                        Remaining = header.Size;
                        ReserveData(header.Size);
                        ReadAsync(ReadPayloadCallback, header);
                    }
                    else
                    {
                        HandlePacket(new InPacket(header));
                        Start();
                    }
                }
                else
                {
                    Index += bytesRead;
                    Remaining -= bytesRead;
                    ReadAsync(ReadHeaderCallback);
                }
            }
            catch (ObjectDisposedException ex)
            {

            }
            catch (NullReferenceException ex)
            {

            }
            catch (SocketException ex)
            {

            }
        }
        
        private void ReadPayloadCallback(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                int bytesRead = e.BytesTransferred;
                if (bytesRead == 0)
                {
                    Exchange.AuthMessage = "Disconncted from server.";
                    //Game.Reconnect();
                    return;
                }

                Interlocked.Add(ref transferred, bytesRead);
                Interlocked.Add(ref received, bytesRead);

                if (bytesRead == Remaining)
                {
                    // get header and packet, handle it
                    ServerHeader header = (ServerHeader)SocketAsyncState;
                    HandlePacket(new InPacket(header, ReceiveData, ReceiveDataLength));

                    // start new asynchronous read
                    Start();
                }
                else
                {
                    // more payload to read
                    Index += bytesRead;
                    Remaining -= bytesRead;
                    ReadAsync(ReadPayloadCallback, SocketAsyncState);
                }
            }
            catch(NullReferenceException ex)
            {

            }
            catch(SocketException ex)
            {
                //Game.Reconnect();
                return;
            }
        }

        #endregion

        public void HandlePackets()
        {

        }

        private void HandlePacket(InPacket packet)
        {
            try
            {
                Debug.Log(packet.Header.Command.ToString() + " Size: " + packet.Header.Size.ToString());
                LastInOpcode = packet.Header.Command;
                PacketHandler handler;
                if (PacketHandlers.TryGetValue(packet.Header.Command, out handler))
                {
                    handler(packet);
                }
                Game.HandleTriggerInput(TriggerActionType.Opcode, packet);
            }
            catch(Exception ex)
            {

            }
            finally
            {
                //packet.Dispose();
            }
        }
        

        #region GameSocket Members

        public override void Start()
        {
            ReserveData(4, true);
            Index = 0;
            Remaining = 1;
            ReadAsync(ReadSizeCallback);
        }

        public override bool Connect()
        {
            try
            {
                Exchange.AuthMessage = string.Format("Connecting to realm {0}... ", ServerInfo.Name);

                if (connection != null)
                    connection.Close();
                connection = new TcpClient(ServerInfo.Address, ServerInfo.Port);
            }
            catch (SocketException ex)
            {
                return false;
            }

            return true;
        }

        #endregion

        public void Send(OutPacket packet)
        {
            LastOutOpcode = packet.Header.Command;
            byte[] data = packet.Finalize(authenticationCrypto);

            try
            {
                connection.Client.Send(data, 0, data.Length, SocketFlags.None);
            }
            catch(ObjectDisposedException ex)
            {

            }
            catch(EndOfStreamException ex)
            {

            }

            Interlocked.Add(ref transferred, data.Length);
            Interlocked.Add(ref sent, data.Length);
        }
    }
}
