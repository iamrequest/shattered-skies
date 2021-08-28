using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : BaseEnemy {
    [Tooltip("Useful for testing states individually")]
    public bool DEBUG_DO_NOT_RETURN_TO_BASE_STATE;

    public BaseState multiplexerAttackState;


    [Header("Warp")]
    public AudioClip warpSFX;
    [Range(0f, .5f)]
    public float sfxPitchRange;
    [Range(0f, 2f)]
    public float warpDuration;

    public bool isWarping {
        get {
            return warpCoroutine != null;
        }
    }
    private Coroutine warpCoroutine;

    protected override void Awake() {
        base.Awake();
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

        // Fade out anim

        yield return new WaitForSeconds(warpDuration);

        // Fade in anim

        warpCoroutine = null;
    }
}
