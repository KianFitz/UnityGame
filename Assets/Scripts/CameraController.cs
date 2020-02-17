using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private float mouseSens;
    private float cameraX;
    private float cameraY;
    public float cameraZ { get; set; }


    public Vector3 rotation { get; private set; }

    void Start() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        mouseSens = 100;
    }

    void Update() {
        cameraX += mouseSens * Input.GetAxis("Mouse X") * Time.deltaTime;
        cameraY -= mouseSens * Input.GetAxis("Mouse Y") * Time.deltaTime;
        cameraY = cameraY < -90 ? -90 : cameraY;
        cameraY = cameraY > 90 ? 90 : cameraY;

        rotation = new Vector3(0, cameraX, 0);

        transform.eulerAngles = new Vector3(cameraY, cameraX, cameraZ);
    }
}
