using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yes : MonoBehaviour {

    private Material mat;

    void Start() {
        mat = GetComponent<Renderer>().material;
        mat.SetFloat("_Player", 0);
    }
}
