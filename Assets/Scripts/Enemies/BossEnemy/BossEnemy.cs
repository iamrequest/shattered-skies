using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : BaseEnemy {
    [Tooltip("Useful for testing states individually")]
    public bool DEBUG_NO_STATE_MULTIPLEX, DEBUG_NO_STATE_RETURN;

    public PlayerDamageEventChannel playerDamageEventChannel;

    public BaseState multiplexerAttackState;
    public Transform lookatTarget;
    public bool isLookingAtPlayer;
    [Range(0f, 1f)]
    public float lookAtPlayerSpeed;

    [Header("Warp")]
    public AudioClip warpSFX;
    [Range(0f, .5f)]
    public float sfxPitchRange;
    [Range(0f, 2f)]
    public float warpDuration;

    private int animHashIsWarping;
    private int animHashIsFloating;

    public bool isWarping {
        get {
            return warpCoroutine != null;
        }
    }
    private Coroutine warpCoroutine;

    protected override void Awake() {
        base.Awake();
        animHashIsFloating = Animator.StringToHash("isFloating");
        animHashIsWarping = Animator.StringToHash("isWarping");
    }

    private void OnEnable() {
        playerDamageEventChannel.onPlayerRevive += OnPlayerRevive;
    }
    private void OnDisable() {
        playerDamageEventChannel.onPlayerRevive -= OnPlayerRevive;
    }
    private void Update() {
        if (isLookingAtPlayer) {
            LookAtPlayer();
        }
    }


    public void Warp(Transform warpTransform) {
        if (isWarping) {
            Debug.LogWarning("Attempted to warp, but we're already in the middle of a warp.");
            return;
        }

        warpCoroutine = StartCoroutine(DoWarp(warpTransform));
    }

    private IEnumerator DoWarp(Transform warpTransform) {
        // Play SFX
        float sfxPitch = 1 + Random.Range(-sfxPitchRange, sfxPitchRange);
        SFXPlayer.Instance.PlaySFX(warpSFX, transform.position, sfxPitch, VolumeManager.Instance.bossWarp);

        // Fade in/out
        animator.SetBool(animHashIsWarping, true);
        yield return new WaitForSeconds(warpDuration);

        // Move to transform
        transform.position = warpTransform.position;
        transform.forward = warpTransform.forward;

        // Fade back in
        animator.SetBool(animHashIsWarping, false);

        // Play SFX
        sfxPitch = 1 + Random.Range(-sfxPitchRange, sfxPitchRange);
        SFXPlayer.Instance.PlaySFX(warpSFX, transform.position, sfxPitch, VolumeManager.Instance.bossWarp);

        warpCoroutine = null;
    }

    // This should probably be in the finite state machine, but I don't have time to refactor properly
    public void OnPlayerRevive() {
        fsm.TransitionTo(fsm.initialState);
    }

    public void setIsFloating(bool isFloating) {
        animator.SetBool(animHashIsFloating, isFloating);
    }

    public void LookAtPlayer() {
        lookatTarget.transform.position = Vector3.Lerp(lookatTarget.transform.position, Player.Instance.cam.transform.position, lookAtPlayerSpeed);
    }
}
