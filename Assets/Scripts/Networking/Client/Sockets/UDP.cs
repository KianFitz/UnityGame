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
    class UDP : SocketImpl
    {
        UdpClient _socket;
        IPEndPoint _endPoint;

        internal UDP()
        {
            _endPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
        }

        internal void Connect(int localPort)
        {
            Debug.LogError("Connect hit with local port: " + localPort);

            _socket = new UdpClient(localPort);

            Connect();
        }

        internal override void Connect()
        {
            _socket.Connect(_endPoint);
            Debug.Log("Socket connected expected. Socket state: " + _socket.Client.Connected);

            _socket.BeginReceive(OnReceiveData, null);

            using (ByteBuffer buffer = new ByteBuffer(0))
            {
                buffer.Write(Client.Instance().ClientID);
                SendData(buffer);
            }
        }

        internal override void OnReceiveData(IAsyncResult ar)
        {
            try
            {
                Debug.Log("a");

                byte[] _data = _socket.EndReceive(ar, ref _endPoint);
                _socket.BeginReceive(OnReceiveData, null);

                if (_data.Length < 4)
                {
                    Disconnect();
                    return;
                }

                HandlePacket(_data);
            }
            catch(Exception ex)
            {
                Debug.LogException(ex);
                Disconnect();
            }

        }

        internal override void SendData(ByteBuffer buff)
        {
            try
            {
                if (_socket is null || !_socket.Client.Connected)
                    return;

                //int opcode = buff.ReadInt(false);
                //Debug.Log("Sent opcode " + (Opcode)opcode);

                byte[] data = buff.AsByteArray();

                _socket.BeginSend(data, data.Length, null, null);
            }
            catch (Exception ex)
            {
                Debug.Log($"Failed to send using UDP: {ex}");
            }
        }
    }
}
