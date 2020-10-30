using Assets.Scripts.Net.Client;
using Assets.Scripts.Net.Shared;
using Assets.Scripts.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Assets.Scripts.Net.Client
{
    public class Client : MonoBehaviour
    {
        private static Client _instance;
        public static Client Instance() => _instance;

        private bool isConnected = false;
        private delegate void PacketHandler(Packet _packet);
        private static Dictionary<int, PacketHandler> packetHandlers;

        public static int dataBufferSize = 4096;

        public string ip = "127.0.0.1";
        public int port = 27930;
        public uint myId = 0;
        public TCP tcp;
        public UDP udp;

        PacketInspector _inspector;

        public class TCP
        {
            PacketInspector _inspector;

            public TcpClient socket;

            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public TCP(PacketInspector insp)
            {
                _inspector = insp;
            }

            /// <summary>Attempts to connect to the server via TCP.</summary>
            public void Connect()
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };

                receiveBuffer = new byte[dataBufferSize];
                socket.BeginConnect(Instance().ip, Instance().port, ConnectCallback, socket);
            }

            /// <summary>Initializes the newly connected client's TCP-related info.</summary>
            private void ConnectCallback(IAsyncResult _result)
            {
                socket.EndConnect(_result);

                if (!socket.Connected)
                {
                    return;
                }

                stream = socket.GetStream();

                receivedData = new Packet();

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }

            /// <summary>Sends data to the client via TCP.</summary>
            /// <param name="_packet">The packet to send.</param>
            public void SendData(Packet _packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null); // Send data to server
                    }
                }
                catch (Exception _ex)
                {
                    Debug.Log($"Error sending data to server via TCP: {_ex}");
                }
            }

            /// <summary>Reads incoming data from the stream.</summary>
            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);
                    if (_byteLength <= 0)
                    {
                        Instance().Disconnect();
                        return;
                    }

                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    receivedData.Reset(HandleData(_data)); // Reset receivedData if all data was handled
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch(Exception ex)
                {
                    Debug.Log(ex);
                    Disconnect();
                }
            }

            /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
            /// <param name="_data">The recieved data.</param>
            private bool HandleData(byte[] _data)
            {
                ulong _packetLength = 0;

                receivedData.SetBytes(_data);

                if (receivedData.UnreadLength() >= 4)
                {
                    // If client's received data contains a packet
                    _packetLength = receivedData.ReadUlong();
                    if (_packetLength <= 0)
                    {
                        // If packet contains no data
                        return true; // Reset receivedData instance to allow it to be reused
                    }
                }

                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    // While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
                    byte[] _packetBytes = receivedData.ReadBytes((int)_packetLength);

                    _inspector.SetPacket(receivedData);

                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            UInt16 _packetId = _packet.ReadUshort();
                            packetHandlers[_packetId](_packet); // Call appropriate method to handle the packet
                        }
                    });

                    _packetLength = 0; // Reset packet length
                    if (receivedData.UnreadLength() >= 4)
                    {
                        // If client's received data contains another packet
                        _packetLength = receivedData.ReadUlong();
                        if (_packetLength <= 0)
                        {
                            // If packet contains no data
                            return true; // Reset receivedData instance to allow it to be reused
                        }
                    }
                }

                if (_packetLength <= 1)
                {
                    return true; // Reset receivedData instance to allow it to be reused
                }

                return false;
            }

            /// <summary>Disconnects from the server and cleans up the TCP connection.</summary>
            private void Disconnect()
            {
                Instance().Disconnect();

                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }
        }
        public class UDP
        {
            public UdpClient socket;
            public IPEndPoint endPoint;

            PacketInspector _inspector;

            public UDP(PacketInspector insp)
            {
                _inspector = insp;
            }

            public UDP()
            {
                endPoint = new IPEndPoint(IPAddress.Parse(Instance().ip), Instance().port);
            }

            /// <summary>Attempts to connect to the server via UDP.</summary>
            /// <param name="_localPort">The port number to bind the UDP socket to.</param>
            public void Connect(int _localPort)
            {
                socket = new UdpClient(_localPort);

                socket.Connect(endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                using (Packet _packet = new Packet())
                {
                    SendData(_packet);
                }
            }

            /// <summary>Sends data to the client via UDP.</summary>
            /// <param name="_packet">The packet to send.</param>
            public void SendData(Packet _packet)
            {
                try
                {
                    _packet.InsertUint(Instance().myId); // Insert the client's ID at the start of the packet
                    if (socket != null)
                    {
                        socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                    }
                }
                catch (Exception _ex)
                {
                    Debug.Log($"Error sending data to server via UDP: {_ex}");
                }
            }

            /// <summary>Receives incoming UDP data.</summary>
            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    byte[] _data = socket.EndReceive(_result, ref endPoint);
                    socket.BeginReceive(ReceiveCallback, null);

                    if (_data.Length < 4)
                    {
                        Instance().Disconnect();
                        return;
                    }

                    HandleData(_data);
                }
                catch
                {
                    Disconnect();
                }
            }

            /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
            /// <param name="_data">The recieved data.</param>
            private void HandleData(byte[] _data)
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetLength = _packet.ReadInt();
                    _data = _packet.ReadBytes(_packetLength);
                    
                    
                }

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_data))
                    {
                        int _packetId = _packet.ReadInt();
                        packetHandlers[_packetId](_packet); // Call appropriate method to handle the packet
                    }
                });
            }

            /// <summary>Disconnects from the server and cleans up the UDP connection.</summary>
            private void Disconnect()
            {
                Instance().Disconnect();

                endPoint = null;
                socket = null;
            }
        }
        private void Awake()
        {
            if (_instance is null)
            {
                _instance = this;
            }
            else
            {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }
        }

        private void Start()
        {
            InitializeClientData();

            _inspector = GameObject.Find("Network Manager").GetComponent<PacketInspector>();

            tcp = new TCP(_inspector);
            udp = new UDP(_inspector);
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        private void InitializeClientData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)Opcodes.Opcode.SMSG_WELCOME, ClientPacketHandler.HandleWelcomeMessage },
            { (int)Opcodes.Opcode.SMSG_SPAWN_PLAYER, ClientPacketHandler.HandleSpawnPlayer },
            { (int)Opcodes.Opcode.MSG_PLAYER_MOVEMENT, ClientPacketHandler.HandlePlayerMovement },
            { (int)Opcodes.Opcode.SMSG_PLAYER_ROTATION, ClientPacketHandler.HandlePlayerRotation },

        };
            Debug.Log("Initialized packets.");
        }

        public void Connect()
        {
            tcp.Connect();
            isConnected = true;
        }

        private void Disconnect()
        {
            if (isConnected)
            {
                isConnected = false;
                tcp.socket.Close();
                udp.socket.Close();

                Debug.Log("Disconnected");
            }
        }
    }
}
