using Assets.Scripts.Networking.Server;
using Assets.Scripts.Networking.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityGame.Scripts.Network.Shared;
using static Assets.Scripts.Networking.Server.ServerHandler;

namespace Assets.Scripts.Networking.Client.Sockets
{
    class SocketImpl
    {
        bool _isConnected = false;

        //TODO: CONFIG!
        protected string IP = "94.174.143.221";
        protected int Port = 27930;

        internal virtual void Connect()
        {
            throw new NotSupportedException("");
        }

        internal virtual void OnConnect(IAsyncResult ar)
        {
            throw new NotSupportedException("");
        }

        internal virtual void Disconnect()
        {
            throw new NotSupportedException("");
        }

        internal virtual void SendData(ByteBuffer buff)
        {
            throw new NotSupportedException("");
        }

        internal virtual void OnReceiveData(IAsyncResult ar)
        {
            throw new NotSupportedException("");
        }

        internal virtual void HandlePacket(byte[] data)
        {
            ThreadManager.AddToQueue(() =>
            {
                ByteBuffer buff = new ByteBuffer(data);
                int opcode = buff.ReadInt();
                Debug.Log("Received opcode " + (Opcode)opcode);

                if (Client.OpcodeTable.TryGetValue((Opcode)opcode, out Client.OpcodeHandler handler))
                {
                    handler.Invoke(buff);
                }
                else
                {
                    Debug.LogError("Recieved unknown packet, disconnecting client");
                    Disconnect();
                }
            });
        }

        internal bool IsConnected() => _isConnected;

        protected void SetConnected(bool value) => _isConnected = value;
    }
}
