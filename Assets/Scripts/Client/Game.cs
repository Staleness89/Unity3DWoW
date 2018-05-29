 
using System.Threading;
using Client.Authentication;
using Client.Authentication.Network;
using Client.World.Network;
using Client.Chat;
using System.Collections.Generic;
 

namespace Client
{
    public interface IGame
    {
        BigInteger Key { get; }
        string Username { get; }
        
        GameWorld World { get; }

        void ConnectTo(WorldServerInfo server);

        void Start();

        void Reconnect();

        void NoCharactersFound();

        void InvalidCredentials();

        void Exit();

        void SendPacket(OutPacket packet);

        void HandleTriggerInput(TriggerActionType type, params object[] inputs);
    }
}
