using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    [SerializeField] private int health = 100;
    private Material playerMat;
    
    private void Start() {
        playerMat = GetComponent<Renderer>().material;
        playerMat.SetFloat("_PlayerHealth", 100);
    }

    public void Damage(int damage) {
        health -= damage;

        if (health <= 0) {
            // die
        }
        playerMat.SetFloat("_PlayerHealth", health);
    }
}
