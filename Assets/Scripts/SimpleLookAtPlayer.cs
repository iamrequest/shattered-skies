using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLookAtPlayer : MonoBehaviour {
    public Transform lookatTarget;
    public bool isLookingAtPlayer;
    [Range(0f, 1f)]
    public float lookAtPlayerSpeed;

    private void Update() {
        if (isLookingAtPlayer) {
            lookatTarget.transform.position = Vector3.Lerp(lookatTarget.transform.position, Player.Instance.cam.transform.position, lookAtPlayerSpeed);
        }
    }
}

