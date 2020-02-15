using Assets.Scripts.Networking.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGame.Scripts.Network.Shared;

namespace Assets.Scripts.Networking.Server
{
    public class PlayerHandler
    {
        internal static void HandleAuthAckOpcode(ByteBuffer data, Session session)
        {
            ByteBuffer buff = new ByteBuffer(Opcode.SMSG_PLAYER_JOINED);
            buff.Write(session.GetId());
            buff.Write(Server.SessionManager.Instance()._serverSpawnPosition);

            SessionManager.Instance().SendToAll(buff);
            SpawnPlayerServerSide();
        }
    }
}
