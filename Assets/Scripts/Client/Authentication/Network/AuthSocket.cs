using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

using System.Text;
using Client.Crypto;
using System.Threading;
using Assets.Scripts.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Client.Authentication.Network
{
    public class AuthSocket : GameSocket
    {
        public BigInteger Key { get; private set; }
        byte[] m2;

        NetworkStream stream;

        private string Username;
        private readonly byte[] PasswordHash;

        private readonly string Hostname;
        private readonly int Port;
        int failedAuthentications;
        const int MAX_FAILED_AUTENTICATIONS = 10;

        Dictionary<AuthCommand, CommandHandler> Handlers;

        public override string LastOutOpcodeName
        {
            get
            {
                return LastOutOpcode.ToString();
            }
        }
        public AuthCommand? LastOutOpcode
        {
            get;
            protected set;
        }
        public override string LastInOpcodeName
        {
            get
            {
                return LastInOpcode.ToString();
            }
        }
        public AuthCommand? LastInOpcode
        {
            get;
            protected set;
        }

        public AuthSocket(IGame program, string hostname, int port, string username, string password)
        {
            this.Game = program;

            this.Username = username.ToUpper();
            this.Hostname = hostname;
            this.Port = port;

            string authstring = string.Format("{0}:{1}", this.Username, password);

            PasswordHash = HashAlgorithm.SHA1.Hash(Encoding.ASCII.GetBytes(authstring.ToUpper()));

            ReserveData(1);
        }

        ~AuthSocket()
        {
            Dispose();
        }

        void SendLogonChallenge()
        {
            ClientAuthChallenge challenge = new ClientAuthChallenge()
            {
                username = Username,
                IP = BitConverter.ToUInt32((connection.Client.LocalEndPoint as IPEndPoint).Address.GetAddressBytes(), 0)
            };

            Send(challenge);
            ReadCommand();
        }

        void Send(ISendable sendable)
        {
            LastOutOpcode = sendable.Command;
            sendable.Send(stream);
        }

        void Send(byte[] buffer)
        {
            LastOutOpcode = (AuthCommand)buffer[0];
            stream.Write(buffer, 0, buffer.Length);
        }

        #region Handlers

        public override void InitHandlers()
        {
            Handlers = new Dictionary<AuthCommand, CommandHandler>();

            Handlers[AuthCommand.LOGON_CHALLENGE] = HandleRealmLogonChallenge;
            Handlers[AuthCommand.LOGON_PROOF] = HandleRealmLogonProof;
            Handlers[AuthCommand.REALM_LIST] = HandleRealmList;
        }

        void HandleRealmLogonChallenge()
        {
            ServerAuthChallenge challenge = new ServerAuthChallenge(new BinaryReader(connection.GetStream()));

            switch (challenge.error)
            {
                case AuthResult.SUCCESS:
                    {

                        Exchange.AuthMessage = "Received logon challenge";

                        BigInteger N, A, B, a, u, x, S, salt, unk1, g, k;
                        k = new BigInteger(3);

                        #region Receive and initialize

                        B = challenge.B.ToBigInteger();            // server public key
                        g = challenge.g.ToBigInteger();
                        N = challenge.N.ToBigInteger();            // modulus
                        salt = challenge.salt.ToBigInteger();
                        unk1 = challenge.unk3.ToBigInteger();

                        #endregion

                        x = HashAlgorithm.SHA1.Hash(challenge.salt, PasswordHash).ToBigInteger();

                        #region Create random key pair

                        var rand = System.Security.Cryptography.RandomNumberGenerator.Create();

                        do
                        {
                            byte[] randBytes = new byte[19];
                            rand.GetBytes(randBytes);
                            a = randBytes.ToBigInteger();

                            A = g.modPow(a, N);
                        } while (A.modPow(1, N) == 0);

                        #endregion

                        #region Compute session key

                        u = HashAlgorithm.SHA1.Hash(A.ToCleanByteArray(), B.ToCleanByteArray()).ToBigInteger();

                        // compute session key
                        S = ((B + k * (N - g.modPow(x, N))) % N).modPow(a + (u * x), N);
                        byte[] keyHash;
                        byte[] sData = S.ToCleanByteArray();
                        if (sData.Length < 32)
                        {
                            var tmpBuffer = new byte[32];
                            Buffer.BlockCopy(sData, 0, tmpBuffer, 32 - sData.Length, sData.Length);
                            sData = tmpBuffer;
                        }
                        byte[] keyData = new byte[40];
                        byte[] temp = new byte[16];

                        // take every even indices byte, hash, store in even indices
                        for (int i = 0; i < 16; ++i)
                            temp[i] = sData[i * 2];
                        keyHash = HashAlgorithm.SHA1.Hash(temp);
                        for (int i = 0; i < 20; ++i)
                            keyData[i * 2] = keyHash[i];

                        // do the same for odd indices
                        for (int i = 0; i < 16; ++i)
                            temp[i] = sData[i * 2 + 1];
                        keyHash = HashAlgorithm.SHA1.Hash(temp);
                        for (int i = 0; i < 20; ++i)
                            keyData[i * 2 + 1] = keyHash[i];

                        Key = keyData.ToBigInteger();

                        #endregion

                        #region Generate crypto proof

                        // XOR the hashes of N and g together
                        byte[] gNHash = new byte[20];

                        byte[] nHash = HashAlgorithm.SHA1.Hash(N.ToCleanByteArray());
                        for (int i = 0; i < 20; ++i)
                            gNHash[i] = nHash[i];

                        byte[] gHash = HashAlgorithm.SHA1.Hash(g.ToCleanByteArray());
                        for (int i = 0; i < 20; ++i)
                            gNHash[i] ^= gHash[i];

                        // hash username
                        byte[] userHash = HashAlgorithm.SHA1.Hash(Encoding.ASCII.GetBytes(Username));

                        // our proof
                        byte[] m1Hash = HashAlgorithm.SHA1.Hash
                        (
                            gNHash,
                            userHash,
                            challenge.salt,
                            A.ToCleanByteArray(),
                            B.ToCleanByteArray(),
                            Key.ToCleanByteArray()
                        );

                        // expected proof for server
                        m2 = HashAlgorithm.SHA1.Hash(A.ToCleanByteArray(), m1Hash, keyData);

                        #endregion

                        #region Send proof

                        ClientAuthProof proof = new ClientAuthProof()
                        {
                            A = A.ToCleanByteArray(),
                            M1 = m1Hash,
                            crc = new byte[20],
                        };

                        Exchange.AuthMessage = "Sending logon proof";
                        Send(proof);

                        #endregion

                        break;
                    }
                case AuthResult.NO_MATCH:
                    Exchange.AuthMessage = "Unknown account name";
                    break;
                case AuthResult.ACCOUNT_IN_USE:
                    Exchange.AuthMessage = "Account already logged in";
                    break;
                case AuthResult.WRONG_BUILD_NUMBER:
                    Exchange.AuthMessage = "Wrong build number";
                    break;
            }

            // get next command
            ReadCommand();
        }

        void HandleRealmLogonProof()
        {
            ServerAuthProof proof = new ServerAuthProof(new BinaryReader(connection.GetStream()));

            switch (proof.error)
            {
                case AuthResult.UPDATE_CLIENT:
                    Exchange.AuthMessage = "Client update requested";
                    break;
                case AuthResult.NO_MATCH:
                case AuthResult.UNKNOWN2:
                    Exchange.AuthMessage = "Wrong password or invalid account or authentication error";
                    failedAuthentications++;
                    if (failedAuthentications >= MAX_FAILED_AUTENTICATIONS)
                    {
                        Game.InvalidCredentials();
                        return;
                    }
                    Thread.Sleep(1000);
                    break;
                case AuthResult.WRONG_BUILD_NUMBER:
                    Exchange.AuthMessage = "Wrong build number";
                    break;
                default:
                    if (proof.error != AuthResult.SUCCESS)
                        Exchange.AuthMessage = string.Format("Unkown error {0}", proof.error);
                    break;
            }

            if (proof.error != AuthResult.SUCCESS)
            {
                //Game.Reconnect();
                return;
            }

            Exchange.AuthMessage = "Received logon proof";

            bool equal = true;
            equal = m2 != null && m2.Length == 20;
            for (int i = 0; i < m2.Length && equal; ++i)
                if (!(equal = m2[i] == proof.M2[i]))
                    break;

            if (!equal)
            {
                Exchange.AuthMessage = "Server auth failed!";
                SendLogonChallenge();
                return;
            }
            else
            {
                Exchange.AuthMessage = "Authentication succeeded!";
                failedAuthentications = 0;

                Exchange.AuthMessage = "Requesting realm list";
                Send(new byte[] { (byte)AuthCommand.REALM_LIST, 0x0, 0x0, 0x0, 0x0 });
            }

            // get next command
            ReadCommand();
        }

        void HandleRealmList()
        {
            BinaryReader reader = new BinaryReader(connection.GetStream());

            uint size = reader.ReadUInt16();
            WorldServerList realmList = new WorldServerList(reader);
            Exchange.AuthMessage = "Connected";

            if (LoginHelpers.LAST_KNOWN_REALM_LIST.Length > 2)
            {
                foreach (WorldServerInfo rl in Exchange.gameClient.RealmServerList)
                {
                    if (rl.Name == LoginHelpers.LAST_KNOWN_REALM_LIST)
                    {
                        LoginHelpers.ConnectToWorld = true;
                        Exchange.CurrentRealm = rl;
                        Exchange.gameClient.ConnectTo(Exchange.CurrentRealm);
                    }
                }
            }
            else
            {
                AuthFrame.ShowRealms = true;
            }
        }

        #endregion

        #region GameSocket Members

        public override void Start()
        {
            ReadCommand();
        }

        private void ReadCommand()
        {
            try
            {
                this.connection.Client.BeginReceive
                (
                    ReceiveData, 0, 1,    // buffer and buffer bounds
                    SocketFlags.None,    // flags for the read
                    this.ReadCallback,    // callback to handle completion
                    null                // state object
                );
            }
            catch (Exception ex)
            {

            }
        }

        protected void ReadCallback(IAsyncResult result)
        {
            try
            {
                int size = this.connection.Client.EndReceive(result);

                if (size == 0)
                {
                    Exchange.AuthMessage = "Server has disconnected.";
                    Game.Exit();
                }

                AuthCommand command = (AuthCommand)ReceiveData[0];
                LastInOpcode = command;

                CommandHandler handler;
                if (Handlers.TryGetValue(command, out handler))
                    handler();
                else
                    Exchange.AuthMessage = (string.Format("Unkown or unhandled command '{0}'", command));
            }
            // these exceptions can happen as race condition on shutdown
            catch (ObjectDisposedException ex)
            {

            }
            catch (NullReferenceException ex)
            {

            }
            catch (SocketException ex)
            {

                // Game.Reconnect();
            }
            catch (EndOfStreamException)
            {
                // Game.Reconnect();
            }
        }

        public override bool Connect()
        {
            try
            {
                connection = new TcpClient(this.Hostname, this.Port);
                stream = connection.GetStream();

                Exchange.AuthMessage = "Connecting...";

                SendLogonChallenge();
            }
            catch (SocketException ex)
            {
                Exchange.AuthMessage = string.Format("Auth socket failed. ({0})", (SocketError)ex.ErrorCode);
                return false;
            }

            return true;
        }
        #endregion
    }
}
