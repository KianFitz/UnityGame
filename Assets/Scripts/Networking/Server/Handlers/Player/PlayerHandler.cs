﻿using Assets.Scripts.Networking.Shared;
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

            Vector3 spawnPosition = SessionManager.Instance()._serverSpawnPosition;
            buff.Write(spawnPosition);

            SessionManager.Instance().SendToAll(buff);
            session.InstantiatePlayer(spawnPosition);

            SendAllOldPlayersToNewJoiner(session);
        }

        internal static void SendAllOldPlayersToNewJoiner(Session session)
        {
            foreach (ServerPlayerController plr in SessionManager.Instance().GetAllPlayers())
            {
                if (plr.Id == session.GetId())
                    continue;

                ByteBuffer buff = new ByteBuffer(Opcode.SMSG_PLAYER_JOINED);
                buff.Write(plr.Id);
                buff.Write(plr.transform.position);

                session.SendDirectMessage(buff);
            }
        }

        internal static void HandlePlayerMoved(ByteBuffer buffer, Session session)
        {
            int clientId = buffer.ReadInt();

            if (clientId != session.GetId())
            {
                //Packet forging... cheating
                return;
            }

            ServerPlayerController plrStats = session.GetPlayer();
            plrStats.Move(buffer);

            session.UpdatePlayerLocation(plrStats.transform.position);
            SessionManager.Instance().SendUDPToAll(buffer, session);
        }

        internal static void HandlePlayerRotated(ByteBuffer buffer, Session session)
        {
            int clientId = buffer.ReadInt();

            if (clientId != session.GetId())
            {
                //Packet forging... cheating
                return;
            }

            Vector3 rotation = buffer.ReadVector3();

            session.UpdatePlayerRotation(rotation);
            SessionManager.Instance().SendUDPToAll(buffer, session);
        }
    }
}