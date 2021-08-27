using HurricaneVR.Framework.Core.Grabbers;
using HurricaneVR.Framework.Core.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the reference to the player. Useful for enemy AI
/// </summary>
public class Player : MonoBehaviour {
    public HVRPlayerController playerController { get; private set; }
    private Damageable damageable;

    // Used for fade in/out
    private Animator animator;
    private int animHashVisionFade;

    public PlayerDamageEventChannel playerDamageEventChannel;
    public Camera cam;

    public HVRSocket shoulderSocket;
    public Checkpoint activeCheckpoint;
    public float respawnDelay;

    public static Player Instance { get; private set; }
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.LogError($"Multiple {GetType()} components detected. This is probably a bug.");
            Destroy(this);
        }

        playerController = GetComponentInChildren<HVRPlayerController>();
        damageable = GetComponent<Damageable>();
        animator = GetComponentInChildren<Animator>();
        animHashVisionFade = Animator.StringToHash("visionFade");
    }

    public void RespawnAtCheckpoint() {
        if (!activeCheckpoint) {
            Debug.LogError("Tried to respawn the player, but no checkpoint was set.");
            return;
        }

        StartCoroutine(RespawnAfterDelay());
    }

    /// <summary>
    /// Fade the player's view out, move them to the respawn checkpoint, and fade the view back in.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RespawnAfterDelay() {
        animator.SetBool(animHashVisionFade, true);

        playerDamageEventChannel.RaiseOnPlayerDeath();
        playerController.enabled = false;
        playerController.CharacterController.enabled = false;

        // Drop all interactables
        playerController.LeftHand.ForceRelease();
        playerController.RightHand.ForceRelease();

        yield return new WaitForSeconds(respawnDelay);

        // Move the player into position
        playerController.transform.position = activeCheckpoint.respawnTransform.position;
        playerController.transform.rotation = activeCheckpoint.respawnTransform.rotation;

        // Move the player hands into position
        playerController.LeftHand.transform.position = playerController.transform.position + Vector3.up + playerController.transform.forward;
        playerController.RightHand.transform.position = playerController.transform.position + Vector3.up + playerController.transform.forward;

        // Wait a frame before re-enabling char controllers
        yield return null;
        playerController.CharacterController.enabled = true;
        playerController.enabled = true;

        damageable.Revive();
        playerDamageEventChannel.RaiseOnPlayerRevive();

        animator.SetBool(animHashVisionFade, false);
    }
}
