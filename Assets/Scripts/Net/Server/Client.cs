using Assets.Scripts.Net.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Net.Server
{
    public class Client
    {
        public static int dataBufferSize = 4096;

        private readonly int _playerId;
        public int PlayerId => _playerId;

        public TCP tcp;
        public UDP udp;

        public Player player;

        public Client(int _clientId)
        {
            _playerId = _clientId;
            tcp = new TCP(_playerId);
            udp = new UDP(_playerId);
        }

        public class TCP
        {
            private readonly int _playerId;
            private NetworkStream _stream;
            private Packet _receiveData;
            private byte[] _receiveBuffer;


            public TcpClient socket;


            internal TCP(int playerid)
            {
                _playerId = playerid;
            }

            public void Connect(TcpClient client)
            {
                socket = client;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                _stream = socket.GetStream();
                _receiveData = new Packet();
                _receiveBuffer = new byte[dataBufferSize];

                _stream.BeginRead(_receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                //Send welcome message
                ServerPacketSender.SendWelcome(_playerId, "Welcome");
            }

            private void ReceiveCallback(IAsyncResult ar)
            {
                try
                {
                    int byteLength = _stream.EndRead(ar);
                    if (byteLength <= 0)
                    {
                        Server.clients[_playerId].Disconnect();
                        return;
                    }

                    byte[] newData = new byte[byteLength];
                    Array.Copy(_receiveBuffer, newData, byteLength);

                    _receiveData.Reset(HandleData(newData));
                    _stream.BeginRead(_receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
                }
                catch (Exception ex)
                {
                    Debug.Log($"TCP data got cocked up: {ex}");
                    Server.clients[_playerId].Disconnect();
                }
            }

            internal void SendData(Packet packet)
            {
                try
                {
                    if (socket != null)
                    {
                        _stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null); // Send data to appropriate client
                    }
                }
                catch (Exception _ex)
                {
                    Debug.Log($"Error sending data to player {_playerId} via TCP: {_ex} :'(");
                }
            }

            private bool HandleData(byte[] newData)
            {
                int _packetLength = 0;

                _receiveData.SetBytes(newData);

                if (_receiveData.UnreadLength() >= 4)
                {
                    // If client's received data contains a packet
                    _packetLength = _receiveData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        // If packet contains no data
                        return true; // Reset receivedData instance to allow it to be reused
                    }
                }

                while (_packetLength > 0 && _packetLength <= _receiveData.UnreadLength())
                {
                    // While packet contains data AND packet data length doesn't exceed the length of the packet we're reading
                    byte[] _packetBytes = _receiveData.ReadBytes(_packetLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetId = _packet.ReadInt();
                            Server.packetHandlers[_packetId](_playerId, _packet); // Call appropriate method to handle the packet
                        }
                    });

                    _packetLength = 0; // Reset packet length
                    if (_receiveData.UnreadLength() >= 4)
                    {
                        // If client's received data contains another packet
                        _packetLength = _receiveData.ReadInt();
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

            public void Disconnect()
            {
                socket.Close();
                _stream = null;
                _receiveData = null;
                _receiveBuffer = null;
                socket = null;
            }
        }


        public class UDP
        {
            public IPEndPoint endPoint;

            private int id;

            public UDP(int _id)
            {
                id = _id;
            }

            /// <summary>Initializes the newly connected client's UDP-related info.</summary>
            /// <param name="_endPoint">The IPEndPoint instance of the newly connected client.</param>
            public void Connect(IPEndPoint _endPoint)
            {
                endPoint = _endPoint;
            }

            /// <summary>Sends data to the client via UDP.</summary>
            /// <param name="_packet">The packet to send.</param>
            public void SendData(Packet _packet)
            {
                Server.SendUDPData(endPoint, _packet);
            }

            /// <summary>Prepares received data to be used by the appropriate packet handler methods.</summary>
            /// <param name="_packetData">The packet containing the recieved data.</param>
            public void HandleData(Packet _packetData)
            {
                int _packetLength = _packetData.ReadInt();
                byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.packetHandlers[_packetId](id, _packet); // Call appropriate method to handle the packet
                    }
                });
            }

            /// <summary>Cleans up the UDP connection.</summary>
            public void Disconnect()
            {
                endPoint = null;
            }
        }

        private void Disconnect()
        {
            Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has disconnected. RIP");

            ThreadManager.ExecuteOnMainThread(() =>
            {
                //UnityEngine.Object.Destroy(player.gameObject);
                //player = null;
            });

            tcp.Disconnect();
            //udp.Disconnect();
        }

        #region Packets
        internal void SendIntoGame()
        {
            player = NetworkManager.Instance().InstantiatePlayer();
            player.Initialize(_playerId);

            foreach (Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    if (_client.PlayerId != PlayerId)
                    {
                        ServerPacketSender.SpawnPlayer(_playerId, _client.player);
                    }
                }
            }

            foreach (Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    ServerPacketSender.SpawnPlayer(_client.PlayerId, player);
                }
            }
        }
        #endregion
    }
}
