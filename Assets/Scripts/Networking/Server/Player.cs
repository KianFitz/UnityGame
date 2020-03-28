using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Networking.Server
{
    class Player
    {
        private int _id;
        private string _name;
        private Vector3 _position;
        private Vector3 _rotation;


        public int Id { get => _id; set => _id = value; }
        public string Name { get => _name; set => _name = value; }
        public Vector3 Rotation { get => _rotation; set => _rotation = value; }
        public Vector3 Position { get => _position; set => _position = value; }
    }
}
