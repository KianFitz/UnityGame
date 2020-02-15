using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityGame.Scripts.Network.Shared;

namespace Assets.Scripts.Networking.Client.Handlers.WorldHandler
{
    class WorldHandler
    {
        internal static void NewPlayerSpawned(ByteBuffer data)
        {
            int clientId = data.ReadInt();
            Vector3 spawnPosition = data.ReadVector3();

            Client thisClient = Client.Instance();

            GameObject go = (clientId == thisClient.ClientID) ? thisClient.localPlayerPrefab : thisClient.remotePlayerPrefab;
            GameObject.Instantiate(go, spawnPosition, Quaternion.identity);
        }
    }
}
