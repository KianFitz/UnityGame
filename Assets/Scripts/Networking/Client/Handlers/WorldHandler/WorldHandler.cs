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
            bool localPlayer = clientId == thisClient.ClientID;

            GameObject go = (localPlayer) ? thisClient.localPlayerPrefab : thisClient.remotePlayerPrefab;

            GameObject createdGo = GameObject.Instantiate(go, spawnPosition, Quaternion.identity);
            createdGo.transform.name = clientId.ToString();
        }

        internal static void PlayerPosition(ByteBuffer data)
        {
            int clientId = data.ReadInt();
            Vector3 newPosition = data.ReadVector3();

            GameObject go = GameObject.Find(clientId.ToString());
            go.transform.position = newPosition;
        }

        internal static void PlayerRotation(ByteBuffer data)
        {
            int clientId = data.ReadInt();
            Vector3 newRotation = data.ReadVector3();

            GameObject go = GameObject.Find(clientId.ToString());
            go.transform.rotation = Quaternion.Euler(newRotation);
        }
    }
}
