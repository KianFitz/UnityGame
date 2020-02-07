using Assets.Scripts.Networking.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityGame.Scripts.Network.Shared;
using Assets.Scripts.Networking.Server;

namespace Assets.Scripts.Networking.Server
{
    class ServerHandler
    {
        public delegate void OpcodeHandler(ByteBuffer data, Session session);
        public static Dictionary<Opcode, OpcodeHandler> OpcodeTable = new Dictionary<Opcode, OpcodeHandler>()
        {
            { Opcode.SMSG_AUTH, Handle_NULL},
            { Opcode.CMSG_AUTH_ACK, Handle_NULL },
            { Opcode.SMSG_PLAYER_JOINED, Handle_NULL},
        };

        internal static void Handle_NULL(ByteBuffer data, Session sess)
        {
            Debug.LogError("Recieved incorrect packet");
            SessionManager.Instance().KickSession(sess);
        }
    }
}
