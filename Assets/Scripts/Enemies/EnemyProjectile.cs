using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {
    public float lifetime;

    private void Start() {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision) {
        Destroy(gameObject);
    }
}
