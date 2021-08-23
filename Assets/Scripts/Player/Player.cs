using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the reference to the player. Useful for enemy AI
/// </summary>
public class Player : MonoBehaviour {
    public Camera cam;

    public static Player Instance { get; private set; }
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.LogError($"Multiple {GetType()} components detected. This is probably a bug.");
            Destroy(this);
        }
    }
}
