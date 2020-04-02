using Assets.Scripts.Networking.Client;
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

namespace Assets.Scripts.Networking.Server
{
    class UdpManager
    {
        static UdpClient _udpServer;

        static UdpManager _instance;
        static Assets.Scripts.Logs.Logger _logger;

        public static UdpManager Instance()
        {
            if (_instance is null)
                _instance = new UdpManager();

            return _instance;
        }

        private UdpManager()
        {
            _udpServer = new UdpClient(27930 /*TODO: Fix*/);
            _udpServer.BeginReceive(new AsyncCallback(OnUdpDataReceived), null);
        }

        private void OnUdpDataReceived(IAsyncResult ar)
        {
            try
            {
                IPEndPoint _clientEndpoint = new IPEndPoint(IPAddress.Any, 0);

                byte[] incomingData = _udpServer.EndReceive(ar, ref _clientEndpoint);
                _udpServer.BeginReceive(OnUdpDataReceived, null);

                if (incomingData.Length < 4)
                {
                    return;
                }

                ByteBuffer buffer = new ByteBuffer(incomingData);
                int opcode = buffer.ReadInt();
                int clientId = buffer.ReadInt();

                if (clientId == 0)
                    return;

                Session clientSession = SessionManager.Instance().GetSessionById(clientId);
                if (clientSession is null)
                    return;


                if (!clientSession.IsUDPConnected())
                {
                    clientSession.ConnectUDP(_clientEndpoint);
                    return;
                }

                buffer.Reset();
                if (clientSession.UDPEndpointMatches(_clientEndpoint))
                {
                    clientSession.AddPacketToQueue(buffer);
                }

            }

            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                Debug.Log(ex.StackTrace);
            }
        }

        internal void SendUDPData(IPEndPoint endpoint, ByteBuffer packet)
        {
            try
            {
                if (endpoint is null)
                    return;

                byte[] sendData = packet.AsByteArray();

                _udpServer.BeginSend(sendData, sendData.Length, endpoint, null, null);
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError("Error in UdpManager.SendUDPData", ex);   
            }
        }
    }
}
