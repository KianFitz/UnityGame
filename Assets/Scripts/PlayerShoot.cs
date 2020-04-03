using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour {

    private Vector3 cameraRotation;
    private Transform cameraTransform;

    [SerializeField] MapManager mapManager;
    [SerializeField] private bool allowShooting;
    [SerializeField] private int fireRate;

    void Start() {
        cameraTransform = transform.GetChild(0).transform;
        allowShooting = true;
    }

    void Update() {
        // get player transform
        // check for inputs
        cameraRotation = cameraTransform.forward;
        if (Input.GetKey(KeyCode.Mouse0) && allowShooting) StartCoroutine(Shoot());
    }

    private IEnumerator Shoot() {
        allowShooting = false;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, cameraRotation, out hit)) {
            if (hit.transform.CompareTag("Player")) {
                PlayerStats player = hit.transform.gameObject.GetComponent<PlayerStats>();
                player.Damage(3); // temp
            }
            else if (hit.transform.CompareTag("Terrain")) {
                List<Chunk> chunks = mapManager.ClearSphere((int)hit.point.x, (int)hit.point.y, (int)hit.point.z, 8);
                foreach (Chunk c in chunks) {
                    if (c != null) {
                        c.GenerateMesh();
                        c.UpdateMesh();
                    }
                }
            }
        }
        yield return new WaitForSeconds(GetFireRateTime(fireRate));
        allowShooting = true;
    }

    /// <summary>
    /// Converts fire rate in RPM to the time between rounds
    /// </summary>
    /// <param name="fireRate">The fire rate of the current weapon in RPM</param>
    /// <returns></returns>
    private float GetFireRateTime(int fireRate) {
        return 60f / fireRate;
    }
}
