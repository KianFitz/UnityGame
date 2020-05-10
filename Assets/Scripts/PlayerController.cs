using Assets.Scripts.Net.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 spawn = new Vector3(127, 50, 127);
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float walkSpeed = 5;
    [SerializeField] private float runSpeed = 10;
    [SerializeField] private float buildUpSpeed = 1;
    [SerializeField] private float gravity;
    [SerializeField] private float drag = 0.5f;
    [SerializeField] private AnimationCurve jumpAcceleration;
    [SerializeField] private AnimationCurve slideAcceleration;
    [SerializeField] private float playerWidth = 1;
    [SerializeField] private float wallRunDistance = 0.1f;
    [SerializeField] private float wallRunGravity = 0.2f;
    [SerializeField] private float tiltAngle = 10;
    [SerializeField] private float tiltSpeed = 15;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float crouchTransitionSpeed = 15;
    [SerializeField] private float crouchSpeed = 0.5f;
    [SerializeField] private float idleHeight = 0.9f;


    private void FixedUpdate()
    {
        SendInputToServer();
    }

    private void SendInputToServer()
    {
        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space)
        };

        ClientPacketSender.PlayerMovement(_inputs);
    }
}

//    private CameraController cameraController;
//    private GameObject camera;
//    private CharacterController controller;
//    private float wallRunTime;

//    private RaycastHit hit;

//    public Vector3 moveDirection;
//    public Vector3 gravityVector;
//    public bool isJumping { get; private set; }
//    private bool isWallRunning;
//    [SerializeField] private bool isSliding;

//    void Start() {
//        isJumping = false;
//        controller = GetComponent<CharacterController>();
//        camera = transform.GetChild(0).gameObject;
//        cameraController = camera.GetComponent<CameraController>();
//    }

//    void Update() {
//        //if (!isWallRunning) transform.eulerAngles = cameraController.rotation;
//        if (!isWallRunning) RestRotation(transform.eulerAngles);

//        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
//        Vector3.ClampMagnitude(moveDirection, 1);

//        if (!isJumping && Input.GetKey(KeyCode.Space)) {
//            isJumping = true;
//            gravityVector = Vector3.zero;
//            StartCoroutine(JumpEvent());
//        } else {
//            gravityVector += ApplyGravity();
//        }

//        moveSpeed = Input.GetKey(KeyCode.LeftShift) && !isSliding ? Mathf.Lerp(runSpeed, walkSpeed, Time.deltaTime * buildUpSpeed) : Mathf.Lerp(walkSpeed, runSpeed, Time.deltaTime * buildUpSpeed);

//        Crouch();

//        moveDirection *= moveSpeed;

//        //WallRun();

//        moveDirection = transform.TransformDirection(moveDirection);

//        controller.Move((gravityVector + moveDirection) * Time.deltaTime);
//    }

//    private void WallRun() {
//        if (!controller.isGrounded && moveDirection.z > 10) {
//            if (Input.GetKey(KeyCode.D) && Physics.Raycast(transform.position, transform.right, out hit, playerWidth / 2 + wallRunDistance)) {
//                isWallRunning = true;
//                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(-Vector3.Cross(hit.normal, Vector3.up)), 1);
//                moveDirection.x = 0;
//                gravityVector.y *= wallRunGravity;
//                moveDirection.z = Mathf.Lerp(runSpeed, walkSpeed, Time.deltaTime * buildUpSpeed);
//                TiltCamera(1);

//            } else if(Input.GetKey(KeyCode.A) && Physics.Raycast(transform.position, -transform.right, out hit, playerWidth / 2 + wallRunDistance)) {
//                isWallRunning = true;
//                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(Vector3.Cross(hit.normal, Vector3.up)), 1);
//                moveDirection.x = 0;
//                gravityVector.y *= wallRunGravity;
//                moveDirection.z = Mathf.Lerp(runSpeed, walkSpeed, Time.deltaTime * buildUpSpeed);
//                TiltCamera(-1);
//            }
//            else {
//                wallRunTime = 0;
//                RestCamera();
//                RestRotation(transform.eulerAngles);
//                isWallRunning = false;
//            }
//        }
//        else {
//            RestCamera();
//            RestRotation(transform.eulerAngles);
//            isWallRunning = false;
//        }
//    }

//    private void Crouch() {
//        if (Input.GetKey(KeyCode.LeftControl)) {
//            MoveCamera(-1);
//            moveSpeed *= crouchSpeed;
//        }
//        else {
//            MoveCamera(1);
//        }
//    }

//    private void MoveCamera(int sign) {
//        if (cameraController.transform.localPosition.y > crouchHeight && sign == -1) {
//            cameraController.transform.localPosition += new Vector3(0, sign * crouchTransitionSpeed * Time.deltaTime, 0);
//            if (cameraController.transform.localPosition.y < crouchHeight) {
//                cameraController.transform.localPosition = Vector3.up * crouchHeight;
//            }
//        }
//        else if (cameraController.transform.localPosition.y < idleHeight && sign == 1) {
//            cameraController.transform.localPosition += new Vector3(0, sign * crouchTransitionSpeed * Time.deltaTime, 0);
//            if (cameraController.transform.localPosition.y > idleHeight) {
//                cameraController.transform.localPosition = Vector3.up * idleHeight;
//            }
//        }
//    }

//    private void TiltCamera(int sign) {
//        if (Mathf.Abs(cameraController.cameraZ) < tiltAngle)
//            cameraController.cameraZ += sign * tiltSpeed * Time.deltaTime;
//    }

//    private void RestCamera() {
//        if (cameraController.cameraZ != 0) {
//            if (cameraController.cameraZ < 0) {
//                if (cameraController.cameraZ > -0.5) cameraController.cameraZ = 0;
//                else cameraController.cameraZ += tiltSpeed * Time.deltaTime;
//            }
//            else if (cameraController.cameraZ > 0) {
//                if (cameraController.cameraZ < 0.5) cameraController.cameraZ = 0;
//                else cameraController.cameraZ -= tiltSpeed * Time.deltaTime;
//            }
//        }
//    }

//    private void RestRotation(Vector3 wallRunRotation) {
//        transform.rotation = Quaternion.Slerp(Quaternion.Euler(wallRunRotation), Quaternion.Euler(cameraController.rotation), Time.deltaTime * 10);
//    }

//    private Vector3 ApplyGravity() {
//        if (!controller.isGrounded)
//            return Vector3.down * gravity * Time.deltaTime;
//        return Vector3.zero;
//    }

//    private IEnumerator JumpEvent() {
//        controller.slopeLimit = 90f;
//        float timeInAir = 0f;

//        do {
//            float acceleration = jumpAcceleration.Evaluate(timeInAir);
//            controller.Move(Vector3.up * acceleration * jumpSpeed * Time.deltaTime);
//            timeInAir += Time.deltaTime;
//            yield return null;
//        } while (!controller.isGrounded && controller.collisionFlags != CollisionFlags.Above);

//        controller.slopeLimit = 45f;
//        isJumping = false;
//    }
//}
