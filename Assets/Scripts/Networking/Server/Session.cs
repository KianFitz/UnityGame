using Assets.Scripts.Networking.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityGame.Scripts.Network.Shared;

namespace Assets.Scripts.Networking.Server
{
    public partial class Session
    {
        private int _id;
        private TcpClient _client;
        private NetworkStream _stream;
        private byte[] _buffer;

        private const int bufferSize = 4096;

        private Queue<ByteBuffer> _packetQueue;
        
        public Session(int id, TcpClient client)
        {
            _id = id;
            _client = client;

            _packetQueue = new Queue<ByteBuffer>();
        }

        public void SendDirectMessage(ByteBuffer buff)
        {
            try
            {
                if (_client != null)
                {
                    byte[] buffer = buff.AsByteArray();

                    _stream.BeginWrite(buffer, 0, buffer.Length, null, null);
                }
            }
            catch(Exception ex)
            {
                Debug.LogError($"Failed to send data to player {_id}. Error {ex}");
            }
        }

        public void Connect()
        {
            _client.ReceiveBufferSize = bufferSize;
            _client.SendBufferSize = bufferSize;

            _stream = _client.GetStream();

            _buffer = new byte[bufferSize];
            _stream.BeginRead(_buffer, 0, bufferSize, OnReceivedData, null);
        }

        private void OnReceivedData(IAsyncResult result)
        {
            try
            {
                int dataLength = _stream.EndRead(result);
                if (dataLength <= 0)
                {
                    //DisconnectUser();
                    return;
                }

                byte[] incomingData = new byte[dataLength];
                Array.Copy(_buffer, incomingData, dataLength);

                ByteBuffer buff = new ByteBuffer(incomingData);
                _packetQueue.Enqueue(buff);

                _stream.BeginRead(_buffer, 0, bufferSize, OnReceivedData, null);
            }
            catch(Exception ex)
            {
                Debug.Log($"Failed to receive data due to an exception with message {ex.Message} at {ex.StackTrace}");
            }
        }

        internal void Update(double diff)
        { 
            lock (_packetQueue)
            {
                while (_packetQueue.Count > 0)
                {
                    ByteBuffer buff = _packetQueue.Dequeue();

                    HandlePacket(buff);
                }
            }
        }

        private void HandlePacket(ByteBuffer packet)
        {
            int opcode = packet.ReadInt();

            if (ServerHandler.OpcodeTable.TryGetValue((Opcode)opcode, out ServerHandler.OpcodeHandler handler))
            {
                handler.Invoke(packet);
            }
            else
            {
                Debug.LogError("Recieved unknown packet, disconnecting client");
                SessionManager.Instance().KickSession(this);
            }
        }

        internal void Kick()
        {
            _client.Close();
            _stream.Close();
        }

        internal int GetId()
        {
            return _id;
        }

        internal static void Handle_NULL(ByteBuffer data)
        {
            Debug.LogError("Recieved incorrect packet");
            SessionManager.Instance().KickSession(this);
        }
    }
}
