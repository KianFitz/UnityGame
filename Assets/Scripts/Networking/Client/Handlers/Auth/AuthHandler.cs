using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityGame.Scripts.Network.Shared;

namespace Assets.Scripts.Networking.Client.Handlers.Auth
{
    class AuthHandler
    {
        public static void HandleServerAuthMSG(ByteBuffer data)
        {
            int playerId = data.ReadInt();

            Client thisClient = Client.Instance();
            thisClient.ClientID = playerId;

            AskServerForSpawn(playerId);

            Client.Instance().ConnectUDP();
        }

        private static void AskServerForSpawn(int playerId)
        {
            ByteBuffer buffer = new ByteBuffer(Shared.Opcode.CMSG_AUTH_ACK);
            buffer.Write(playerId);

            Client.Instance().SendTCPMessageToServer(buffer);
        }
    }
}
