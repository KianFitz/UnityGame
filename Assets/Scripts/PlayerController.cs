using Assets.Scripts.Networking.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityGame.Scripts.Network.Shared;

public class PlayerController : MonoBehaviour {
    private Vector3 spawn = new Vector3(127, 50, 127);

    [SerializeField] private float playerWidth = 1;
    [SerializeField] private float wallRunDistance = 0.1f;
    [SerializeField] private float wallRunGravity = 0.2f;
    [SerializeField] private float tiltAngle = 10;
    [SerializeField] private float tiltSpeed = 15;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float crouchTransitionSpeed = 15;
    [SerializeField] private float idleHeight = 0.9f;

    private CameraController cameraController;
    private GameObject camera;
    private float wallRunTime;

    private RaycastHit hit;

    public Vector3 moveDirection;
    //public bool isJumping { get; private set; }
    private bool isWallRunning;
    [SerializeField] private bool isSliding;

    bool isSprinting;
    bool isJumping;
    bool isCrouching;

    Timer _updateTimer;

    void Start() {
        isJumping = false;
        camera = transform.GetChild(0).gameObject;
        cameraController = camera.GetComponent<CameraController>();

        //_updateTimer = new Timer(new TimerCallback(SendPositionToServer), null, TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(50));
    }

    void Update() {
        RestRotation(transform.eulerAngles);

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3.ClampMagnitude(moveDirection, 1);

        isSprinting = Input.GetKey(KeyCode.LeftShift);
        isJumping = Input.GetKey(KeyCode.Space);
        isCrouching = Input.GetKey(KeyCode.LeftControl);

        Crouch();
    }

    private void FixedUpdate()
    {
        SendPositionToServer();
    }

    private void SendPositionToServer()
    {
        ByteBuffer buffer = new ByteBuffer(Assets.Scripts.Networking.Shared.Opcode.CMSG_PLAYER_MOVING);
        buffer.Write(Client.Instance().ClientID);
        buffer.Write(moveDirection); // direction of movement;
        buffer.Write(isSprinting);
        buffer.Write(isJumping);
        buffer.Write(isCrouching);
        buffer.Write(transform.rotation);

        Client.Instance().SendUDPMessageToServer(buffer);
    }

    private void Crouch() {
        if (Input.GetKey(KeyCode.LeftControl)) {
            MoveCamera(-1);
        }
        else {
            MoveCamera(1);
        }
    }

    private void MoveCamera(int sign) {
        if (cameraController.transform.localPosition.y > crouchHeight && sign == -1) {
            cameraController.transform.localPosition += new Vector3(0, sign * crouchTransitionSpeed * Time.deltaTime, 0);
            if (cameraController.transform.localPosition.y < crouchHeight) {
                cameraController.transform.localPosition = Vector3.up * crouchHeight;
            }
        }
        else if (cameraController.transform.localPosition.y < idleHeight && sign == 1) {
            cameraController.transform.localPosition += new Vector3(0, sign * crouchTransitionSpeed * Time.deltaTime, 0);
            if (cameraController.transform.localPosition.y > idleHeight) {
                cameraController.transform.localPosition = Vector3.up * idleHeight;
            }
        }
    }

    private void TiltCamera(int sign) {
        if (Mathf.Abs(cameraController.cameraZ) < tiltAngle)
            cameraController.cameraZ += sign * tiltSpeed * Time.deltaTime;
    }

    private void RestCamera() {
        if (cameraController.cameraZ != 0) {
            if (cameraController.cameraZ < 0) {
                if (cameraController.cameraZ > -0.5) cameraController.cameraZ = 0;
                else cameraController.cameraZ += tiltSpeed * Time.deltaTime;
            }
            else if (cameraController.cameraZ > 0) {
                if (cameraController.cameraZ < 0.5) cameraController.cameraZ = 0;
                else cameraController.cameraZ -= tiltSpeed * Time.deltaTime;
            }
        }
    }

    private void RestRotation(Vector3 wallRunRotation) {
        transform.rotation = Quaternion.Slerp(Quaternion.Euler(wallRunRotation), Quaternion.Euler(cameraController.rotation), Time.deltaTime * 10);
    }
}
