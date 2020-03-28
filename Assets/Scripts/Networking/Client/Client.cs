using Assets.Scripts.Networking.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityGame.Scripts.Network.Shared;
using Assets.Scripts.Networking.Client.Handlers.Auth;
using Assets.Scripts.Networking.Client.Handlers.WorldHandler;
using Assets.Scripts.Networking.Client.Sockets;

namespace Assets.Scripts.Networking.Client
{
    public class Client : MonoBehaviour
    {
        private static Client _instance;
        public static Client Instance() => _instance;

        public string Ip = "127.0.0.1";
        public int Port = 27930;
        public int ClientID;

        internal delegate void OpcodeHandler(ByteBuffer data);
        internal static Dictionary<Opcode, OpcodeHandler> OpcodeTable;

        [SerializeField] internal GameObject localPlayerPrefab;
        [SerializeField] internal GameObject remotePlayerPrefab;

        TCP _tcp;
        UDP _udp;


        private void Awake()
        {
            if (_instance is null)
            {
                _instance = this;
            }
            else
            {
                Debug.LogError("Duplicate instance of Client, Destroying newest");
                Destroy(this);
            }
        }

        void Start()
        {
            InitialisePackets();

            _udp = new UDP();
            _tcp = new TCP();

            ConnectToServer();
        }

        private void InitialisePackets()
        {
            OpcodeTable = new Dictionary<Opcode, OpcodeHandler>()
        {
            { Opcode.SMSG_AUTH, AuthHandler.HandleServerAuthMSG },
            { Opcode.CMSG_AUTH_ACK, Handle_NULL },
            { Opcode.SMSG_PLAYER_JOINED, WorldHandler.NewPlayerSpawned },
            { Opcode.MSG_PLAYER_POSITION, WorldHandler.PlayerPosition },
            { Opcode.MSG_PLAYER_ROTATION, WorldHandler.PlayerRotation }
        };
        }

        private void Handle_NULL(ByteBuffer data)
        {
            throw new NotImplementedException("Received Client Side Packet on Client!");
        }

        private void ConnectToServer()
        {
            _tcp.Connect();
        }

        internal void ConnectUDP()
        {
            _udp.Connect(_tcp.GetLocalEndPoint().Port);
        }

        private void OnApplicationQuit()
        {
            _tcp.Disconnect();
            _udp.Disconnect();
        }

        internal void SendTCPMessageToServer(ByteBuffer buff)
        {
            _tcp.SendData(buff);
        }

        internal void SendUDPMessageToServer(ByteBuffer buff)
        {
            _udp.SendData(buff);
        }
    }
}