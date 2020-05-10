using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Net.Client
{
    public static class ClientPacketHandler
    {
        internal static void HandleWelcomeMessage(Packet _packet)
        {
            int _myId = _packet.ReadInt();
            string _msg = _packet.ReadString();

            Debug.Log($"Message from server: {_msg}");
            Client.Instance().myId = _myId;


            ClientPacketSender.AskForSpawn(_myId);
            // Now that we have the client's id, connect UDP
            Client.Instance().udp.Connect(((IPEndPoint)Client.Instance().tcp.socket.Client.LocalEndPoint).Port);
        }

        internal static void HandleSpawnPlayer(Packet _packet)
        {
            int _id = _packet.ReadInt();
            //string _username = _packet.ReadString();
            Vector3 _position = _packet.ReadVector3();
            Quaternion _rotation = _packet.ReadQuaternion();

            GameManager.instance.SpawnPlayer(_id, _position, _rotation);
        }

        internal static void HandlePlayerRotation(Packet _packet)
        {
            int _id = _packet.ReadInt();
            Quaternion rotation = _packet.ReadQuaternion();

            GameManager.players[_id].transform.rotation = rotation;
        }

        internal static void HandlePlayerMovement(Packet _packet)
        {
            int _id = _packet.ReadInt();
            Vector3 position = _packet.ReadVector3();

            GameManager.players[_id].transform.position = position;
        }
    }
}
