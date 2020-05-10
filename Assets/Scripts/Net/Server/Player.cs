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

        public void FixedUpdate()
        {
            Vector2 _inputDirection = Vector2.zero;
            if (inputs[0])
            {
                _inputDirection.y += 1;
            }
            if (inputs[1])
            {
                _inputDirection.y -= 1;
            }
            if (inputs[2])
            {
                _inputDirection.x -= 1;
            }
            if (inputs[3])
            {
                _inputDirection.x += 1;
            }

            Move(_inputDirection);
        }

        private void Move(Vector2 direction)
        {
            Vector3 _moveDirection = transform.right * direction.x + transform.forward * direction.y;
            _moveDirection *= moveSpeed;

            if (controller.isGrounded)
            {
                yVelocity = 0f;
                if (inputs[4])
                {
                    yVelocity = jumpSpeed;
                }
            }
            yVelocity += gravity;

            _moveDirection.y = yVelocity;
            controller.Move(_moveDirection);

            ServerPacketSender.PlayerPosition(this);
            ServerPacketSender.PlayerRotation(this);
        }

        public void SetInput(bool[] _inputs, Quaternion rotation)
        {
            inputs = _inputs;
            transform.rotation = rotation;
        }
    }
}
