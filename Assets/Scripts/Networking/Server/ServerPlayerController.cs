using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using UnityGame.Scripts.Network.Shared;

namespace Assets.Scripts.Networking.Server
{
    class ServerPlayerController : MonoBehaviour
    {
        private int _id;
        private string _name;
        private CharacterController _charController;


        [SerializeField] private float moveSpeed;
        [SerializeField] private float jumpSpeed;
        [SerializeField] private float walkSpeed = 5;
        [SerializeField] private float runSpeed = 10;
        [SerializeField] private float buildUpSpeed = 1;
        [SerializeField] private float gravity;
        [SerializeField] private float drag = 0.5f;
        [SerializeField] private float crouchSpeed = 0.5f;
        [SerializeField] private AnimationCurve jumpAcceleration;

        private bool _isMidJump;
        private Vector3 gravityVector;

        public int Id { get => _id; set => _id = value; }
        public string Name { get => _name; set => _name = value; }


        private void Start()
        {
            _charController = GetComponent<CharacterController>();
            _isMidJump = false;
        }

        internal void Move(UnityGame.Scripts.Network.Shared.ByteBuffer buffer)
        {
            Vector3 moveDirection = buffer.ReadVector3();
            bool isSprinting = buffer.ReadBool();
            bool isJumping = buffer.ReadBool();
            bool isCrouching = buffer.ReadBool();

            if (!_isMidJump && isJumping)
            {
                _isMidJump = true;
                gravityVector = Vector3.zero;
                StartCoroutine(JumpEvent());
            }
            else
            {
                gravityVector += ApplyGravity();
            }

            moveSpeed = isSprinting ? Mathf.Lerp(runSpeed, walkSpeed, Time.deltaTime * buildUpSpeed) : Mathf.Lerp(walkSpeed, runSpeed, Time.deltaTime * buildUpSpeed);

            if (isCrouching)
                moveSpeed *= crouchSpeed;

            moveDirection *= moveSpeed;
            moveDirection = transform.TransformDirection(moveDirection);

            _charController.Move((gravityVector + moveDirection) * Time.deltaTime);

            SendPositionToPlayers();
        }

        private void SendPositionToPlayers()
        {
            using (ByteBuffer buffer = new ByteBuffer(Shared.Opcode.SMSG_PLAYER_MOVED))
            {
                buffer.Write(_id);
                buffer.Write(transform.position);

                SessionManager.Instance().SendUDPToAll(buffer);
            }
        }

        private Vector3 ApplyGravity()
        {
            if (!_charController.isGrounded)
                return Vector3.down * gravity * Time.deltaTime;
            return Vector3.zero;
        }

        private IEnumerator JumpEvent()
        {
            _charController.slopeLimit = 90f;
            float timeInAir = 0f;

            do
            {
                float acceleration = jumpAcceleration.Evaluate(timeInAir);
                _charController.Move(Vector3.up * acceleration * jumpSpeed * Time.deltaTime);
                timeInAir += Time.deltaTime;
                yield return null;
            } while (!_charController.isGrounded && _charController.collisionFlags != CollisionFlags.Above);

            _charController.slopeLimit = 45f;
            _isMidJump = false;
        }
    }
}
