using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSword : MonoBehaviour {
    public PlayerDamageEventChannel playerDamageEventChannel;
    private HVRGrabbable hvrGrabbable;

    public bool hasBeenGrabbed;

    [Range(0f, 30f)]
    [Tooltip("The maximum distance the player can travel from the sword before it returns to the shoulder socket")]
    public float returnToSocketDistance;

    private void Awake() {
        hvrGrabbable = GetComponent<HVRGrabbable>();
    }
    private void OnEnable() {
        playerDamageEventChannel.onPlayerRevive += OnPlayerDeath;
        hvrGrabbable.HandGrabbed.AddListener(OnGrabbed);
    }

    private void OnDisable() {
        playerDamageEventChannel.onPlayerRevive -= OnPlayerDeath;
        hvrGrabbable.HandGrabbed.RemoveListener(OnGrabbed);
    }

    private void Update() {
        // If the player has found the sword, and isn't holding it right now...
        if (hasBeenGrabbed && !hvrGrabbable.IsHandGrabbed) {
            // ... and they're too far from the sword, then return it to the shoulder socket
            float squaredDistanceToPlayer = (Player.Instance.playerController.transform.position - transform.position).sqrMagnitude;
            if (squaredDistanceToPlayer > returnToSocketDistance * returnToSocketDistance) {
                ReturnToShoulderSocket();
            }
        }
    }

    private void OnGrabbed(HVRGrabberBase arg0, HVRGrabbable arg1) {
        hasBeenGrabbed = true;
    }

    private void OnPlayerDeath() {
        if (hasBeenGrabbed) {
            ReturnToActiveCheckpoint();
        } else {
            ReturnToStartingSocket();
        }
    }

    public void ReturnToActiveCheckpoint() {
        if (Player.Instance.activeCheckpoint.swordRespawnSocket == null) {
            Debug.LogWarning("Tried to return the sword to the active checkpoint, but the socket was null");
            ReturnToShoulderSocket();
            return;
        }

        Player.Instance.activeCheckpoint.swordRespawnSocket.TryGrab(hvrGrabbable, true);
    }
    public void ReturnToStartingSocket() {
        hvrGrabbable.StartingSocket.TryGrab(hvrGrabbable, true);
    }
    public void ReturnToShoulderSocket() {
        Player.Instance.shoulderSocket.TryGrab(hvrGrabbable, true);
    }
}
