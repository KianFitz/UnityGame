using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.Security;
using UnityEditor;
using Assets.Scripts.Net.Shared;

namespace Assets.Scripts.Networking
{
    public class PacketInspector : MonoBehaviour
    {
        private const string filePath = "./Assets/Scripts/Net/OpcodeDocs.json";

        public void SetPacket(Packet buff) 
        { 
            _packet = buff;
            _packet.SetReadPos(0);

            Length = buff.ReadUint();
            SetOpcode((Opcodes.Opcode)buff.ReadUshort());
        }
        public void SetOpcode(Opcodes.Opcode opcode) { Opcode = opcode; FindOpcode(); }

        private Packet      _packet;
        private Rootobject  _object;
        private Opcodesdoc  _currentOpcode;

        public bool autoUpdate = true;

        public ulong Length;
        public Opcodes.Opcode Opcode;

        public List<ListObject> Elements = new List<ListObject>();

        public void FindOpcode()
        {
            Elements.Clear();

            if (_packet is null)
                return;

            string json = File.ReadAllText(filePath);

            _object = JsonConvert.DeserializeObject<Rootobject>(json);

            _currentOpcode = _object.opcodesDoc.First(x => x.opcodeInt == (int)Opcode);

            if (_currentOpcode is null)
                throw new Exception("Reading unknown packet. No documentation");

            ReadValues();
        }

        private void ReadValues()
        {
            foreach (var type in _currentOpcode.types)
            {
                string value = GetNextValue(type.type);

                Elements.Add(new ListObject() { Name = type.name, Type = type.type, Value = value });
            }
        }

        private string GetNextValue(string type)
        {
            switch (type)
            {
                case "uint16"       : return _packet.ReadUshort().ToString();
                case "uint32"       : return _packet.ReadUint().ToString();
                case "uint64"       : return _packet.ReadUlong().ToString();
                case "byte"         : return _packet.ReadByte().ToString();
                case "int16"        : return _packet.ReadShort().ToString();
                case "int32"        : return _packet.ReadInt().ToString();
                case "int64"        : return _packet.ReadLong().ToString();
                case "float"        : return _packet.ReadFloat().ToString();
                case "bool"         : return _packet.ReadBool().ToString();
                case "vector3"      : return _packet.ReadVector3().ToString();
                case "quaternion"   : return _packet.ReadQuaternion().ToString();
                case "string"       : return _packet.ReadString();
            }

            return null;
        }
    }

    [System.Serializable]
    public struct ListObject
    {
        public string Name;
        public string Type;
        public string Value;
    }


    


    public class Rootobject
    {
        public Opcodesdoc[] opcodesDoc { get; set; }
    }

    public class Opcodesdoc
    {
        public string name { get; set; }
        public int opcodeInt { get; set; }
        public Type[] types { get; set; }
    }

    public class Type
    {
        public string name { get; set; }
        public string type { get; set; }
    }


}