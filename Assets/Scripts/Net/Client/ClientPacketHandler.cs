using System;
using System.Collections.Generic;
using System.Linq;
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
            //UDP// Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        }

        internal static void HandleSpawnPlayer(Packet _packet)
        {
            int _id = _packet.ReadInt();
            //string _username = _packet.ReadString();
            Vector3 _position = _packet.ReadVector3();
            Quaternion _rotation = _packet.ReadQuaternion();

            GameManager.instance.SpawnPlayer(_id, _position, _rotation);
        }
    }
}
