using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Net.Server
{
    public class Player : MonoBehaviour
    {
        public int id;
        public CharacterController controller;
        public float gravity = -9.81f;
        public float moveSpeed = 5f;
        public float jumpSpeed = 5f;

        private bool[] inputs;
        private float yVelocity = 0;

        private void Start()
        {
            gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
            moveSpeed *= Time.fixedDeltaTime;
            jumpSpeed *= Time.fixedDeltaTime;
        }

        public void Initialize(int _id)
        {
            id = _id;

            inputs = new bool[5];
        }
    }
}
