using Assets.Scripts.Networking.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityGame.Scripts.Network.Shared;

namespace Assets.Scripts.Networking.Client.Sockets
{
    class TCP : SocketImpl
    {
        TcpClient _client;
        NetworkStream _stream;
        byte[] _buffer;

        const int DataBufferSize = 4096;

        internal TCP()
        {
            _client = new TcpClient()
            {
                SendBufferSize = DataBufferSize,
                ReceiveBufferSize = DataBufferSize
            };

            _buffer = new byte[DataBufferSize];
        }

        internal override void Connect()
        {
            _client.BeginConnect(IP, Port, OnConnect, null);
        }

        internal override void OnConnect(IAsyncResult ar)
        {
            _client.EndConnect(ar);

            if (!_client.Connected)
            {
                return;
            }

            SetConnected(true);
            _stream = _client.GetStream();
            _stream.BeginRead(_buffer, 0, DataBufferSize, OnReceiveData, null);
        }

        internal override void OnReceiveData(IAsyncResult ar)
        {
            try
            {
                int dataLength = _stream.EndRead(ar);
                if (dataLength <= 0)
                {
                    Disconnect();
                    return;
                }

                byte[] incomingData = new byte[dataLength];
                Array.Copy(_buffer, incomingData, dataLength);

                HandlePacket(incomingData);
                _stream.BeginRead(_buffer, 0, DataBufferSize, OnReceiveData, null);
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to receive data due to an exception with message {ex.Message} at {ex.StackTrace}");
            }
        }

        internal override void SendData(ByteBuffer buff)
        {
            try
            {
                if (_stream != null)
                {
                    byte[] buffer = buff.AsByteArray();

                    //int opcode = buff.ReadInt(false);
                    //Debug.Log("Sent opcode " + (Opcode)opcode);

                    _stream.BeginWrite(buffer, 0, buffer.Length, null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to send data to server.");
            }
        }

        internal IPEndPoint GetLocalEndPoint() => (IPEndPoint)_client.Client.LocalEndPoint;

        internal override void Disconnect()
        {
            if (IsConnected())
            {
                _client.Close();
                _stream.Close();
                _client = null;
                _stream = null;

                SetConnected(false);
            }
        }

    }
}
