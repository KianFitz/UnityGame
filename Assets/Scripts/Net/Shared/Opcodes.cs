using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Net.Shared
{
    public static class Opcodes
    {
        public enum Opcode
        {
            SMSG_WELCOME            = 0x001,
            CMSG_WELCOME_ACK        = 0x002,
            SMSG_SPAWN_PLAYER       = 0x003,
            MSG_PLAYER_MOVEMENT     = 0x004,
            SMSG_PLAYER_ROTATION    = 0x005,
        }
    }
}
