using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Assets.Scripts.Net.Shared.Opcodes;

namespace Assets.Scripts.Net.Client
{
    class ClientPacketSender
    {
        /// <summary>Sends a packet to the server via TCP.</summary>
        /// <param name="_packet">The packet to send to the sever.</param>
        private static void SendTCPData(Packet _packet)
        {
            _packet.WriteLength();
            Client.Instance().tcp.SendData(_packet);
        }

        private static void SendUDPData(Packet _packet)
        {
            _packet.WriteLength();
            Client.Instance().udp.SendData(_packet);
        }

        internal static void AskForSpawn(int myId)
        {
            using (Packet _packet = new Packet((int)Opcode.CMSG_WELCOME_ACK))
            {
                _packet.Write(myId);

                SendTCPData(_packet);
            }
        }

        internal static void PlayerMovement(bool[] inputs)
        {
            using (Packet _packet = new Packet((int)Opcode.MSG_PLAYER_MOVEMENT))
            {
                _packet.Write(inputs.Length);
                foreach (bool _input in inputs)
                {
                    _packet.Write(_input);
                }
                _packet.Write(GameManager.players[Client.Instance().myId].transform.rotation);

                SendUDPData(_packet);
            }
        }
    }
}
