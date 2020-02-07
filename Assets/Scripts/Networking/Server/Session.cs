﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Networking.Server
{
    class Session
    {
        private int _id;
        private TcpClient _client;
        private NetworkStream _stream;
        private byte[] _buffer;

        private const int bufferSize = 4096;
        
        public Session(int id, TcpClient client)
        {
            _id = id;
            _client = client;
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

                HandlePacket(incomingData);
            }
            catch(Exception ex)
            {
                Debug.Log($"Failed to receive data due to an exception with message {ex.Message} at {ex.StackTrace}");
            }
        }

        private void HandlePacket(byte[] incomingData)
        {

        }
    }
}