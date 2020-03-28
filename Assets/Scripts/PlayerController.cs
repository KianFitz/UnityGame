using Assets.Scripts.Networking.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGame.Scripts.Network.Shared;

public class PlayerController : MonoBehaviour {
    private Vector3 spawn = new Vector3(127, 50, 127);
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float walkSpeed = 5;
    [SerializeField] private float runSpeed = 10;
    [SerializeField] private float buildUpSpeed = 1;
    [SerializeField] private float gravity;
    [SerializeField] private float drag = 0.5f;
    [SerializeField] private AnimationCurve jumpAcceleration;

    private CameraController cameraController;
    private CharacterController controller;

    public Vector3 moveDirection;
    public Vector3 gravityVector { get; private set; }
    public bool isJumping { get; private set; }

    void Start() {
        isJumping = false;
        controller = GetComponent<CharacterController>();
        cameraController = transform.GetChild(0).GetComponent<CameraController>();
    }

    void Update() {
        transform.eulerAngles = cameraController.rotation;

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3.ClampMagnitude(moveDirection, 1);
        moveDirection = transform.TransformDirection(moveDirection);

        if (!isJumping && Input.GetKey(KeyCode.Space)) {
            isJumping = true;
            gravityVector = Vector3.zero;
            StartCoroutine(JumpEvent());
        } else {
            gravityVector += ApplyGravity();
        }

        moveSpeed = Input.GetKey(KeyCode.LeftShift) ? Mathf.Lerp(runSpeed, walkSpeed, Time.deltaTime * buildUpSpeed) : Mathf.Lerp(walkSpeed, runSpeed, Time.deltaTime * buildUpSpeed);

        moveDirection *= moveSpeed;

        controller.Move((gravityVector + moveDirection) * Time.deltaTime);

        SendPositionToServer();
    }

    private void SendPositionToServer()
    {
        using (ByteBuffer buffer = new ByteBuffer(Assets.Scripts.Networking.Shared.Opcode.MSG_PLAYER_POSITION))
        {
            buffer.Write(Client.Instance().ClientID);
            buffer.Write(this.transform.position);
            
            Client.Instance().SendUDPMessageToServer(buffer);
        }

    }

    private Vector3 ApplyGravity() {
        if (!controller.isGrounded)
            return Vector3.down * gravity * Time.deltaTime;
        return Vector3.zero;
    }

    private IEnumerator JumpEvent() {
        controller.slopeLimit = 90f;
        float timeInAir = 0f;

        do {
            float acceleration = jumpAcceleration.Evaluate(timeInAir);
            controller.Move(Vector3.up * acceleration * jumpSpeed * Time.deltaTime);
            timeInAir += Time.deltaTime;
            yield return null;
        } while (!controller.isGrounded && controller.collisionFlags != CollisionFlags.Above);

        controller.slopeLimit = 45f;
        isJumping = false;
    }
}
