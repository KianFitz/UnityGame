using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityGame.Scripts.Network.Shared;

namespace Assets.Scripts.Networking.Shared
{
    public enum Opcode
    {
        SMSG_AUTH = 0x001,
        CMSG_AUTH_ACK = 0x002,
        SMSG_PLAYER_JOINED = 0x003,
        Test = 0x004
    }
}
