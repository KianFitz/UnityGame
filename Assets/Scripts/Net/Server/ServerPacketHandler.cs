using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Net.Server
{
    class ServerPacketHandler
    {
        #region Auth

        internal static void HandleWelcomeAck(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();

            Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
            if (_fromClient != _clientIdCheck)
            {
                Debug.Log($"Player (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }

            Server.clients[_fromClient].SendIntoGame();
        }

        #endregion

    }
}
