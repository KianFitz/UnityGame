using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private float playerWidth = 1;
    [SerializeField] private float wallRunDistance = 0.1f;
    [SerializeField] private float wallRunGravity = 0.2f;

    private CameraController cameraController;
    private CharacterController controller;

    public Vector3 moveDirection;
    public Vector3 gravityVector;
    public bool isJumping { get; private set; }
    private bool isWallRunning;

    void Start() {
        isJumping = false;
        controller = GetComponent<CharacterController>();
        cameraController = transform.GetChild(0).GetComponent<CameraController>();
    }

    void Update() {
        if (!isWallRunning) transform.eulerAngles = cameraController.rotation;

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        Vector3.ClampMagnitude(moveDirection, 1);

        if (!isJumping && Input.GetKey(KeyCode.Space)) {
            isJumping = true;
            gravityVector = Vector3.zero;
            StartCoroutine(JumpEvent());
        } else {
            gravityVector += ApplyGravity();
        }

        moveSpeed = Input.GetKey(KeyCode.LeftShift) ? Mathf.Lerp(runSpeed, walkSpeed, Time.deltaTime * buildUpSpeed) : Mathf.Lerp(walkSpeed, runSpeed, Time.deltaTime * buildUpSpeed);

        moveDirection *= moveSpeed;

        WallRun();

        moveDirection = transform.TransformDirection(moveDirection);

        controller.Move((gravityVector + moveDirection) * Time.deltaTime);
    }

    private void WallRun() {
        //Debug.DrawRay(transform.position, transform.right * (playerWidth / 2 + wallRunDistance), Color.red);
        //Debug.DrawRay(transform.position, -transform.right * (playerWidth / 2 + wallRunDistance), Color.blue);
        if (!controller.isGrounded) {
            RaycastHit hit;
            if (Input.GetKey(KeyCode.D) && Physics.Raycast(transform.position, transform.right, out hit, playerWidth / 2 + wallRunDistance)) {
                isWallRunning = true;
                transform.rotation = Quaternion.LookRotation(-Vector3.Cross(hit.normal, Vector3.up));
                moveDirection.x = 0;
                gravityVector.y *= wallRunGravity;
                moveDirection.z = Mathf.Lerp(runSpeed, walkSpeed, Time.deltaTime * buildUpSpeed);

            } else if(Input.GetKey(KeyCode.A) && Physics.Raycast(transform.position, -transform.right, out hit, playerWidth / 2 + wallRunDistance)) {
                isWallRunning = true;
                transform.rotation = Quaternion.LookRotation(Vector3.Cross(hit.normal, Vector3.up));
                moveDirection.x = 0;
                gravityVector.y *= wallRunGravity;
                moveDirection.z = Mathf.Lerp(runSpeed, walkSpeed, Time.deltaTime * buildUpSpeed);

            }
            else {
                isWallRunning = false;
            }
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
