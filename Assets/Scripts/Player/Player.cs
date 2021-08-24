using HurricaneVR.Framework.Core.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the reference to the player. Useful for enemy AI
/// </summary>
public class Player : MonoBehaviour {
    private HVRPlayerController playerController;
    private Damageable damageable;

    // Used for fade in/out
    private Animator animator;
    private int animHashVisionFade;

    public Camera cam;

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

        yield return new WaitForSeconds(respawnDelay);
        playerController.transform.position = activeCheckpoint.respawnTransform.position;
        playerController.transform.rotation = activeCheckpoint.respawnTransform.rotation;
        damageable.Revive();

        animator.SetBool(animHashVisionFade, false);
    }
}
