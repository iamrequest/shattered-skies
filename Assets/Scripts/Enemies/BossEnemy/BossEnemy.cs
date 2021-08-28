using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : BaseEnemy {
    [Tooltip("Useful for testing states individually")]
    public bool DEBUG_DO_NOT_RETURN_TO_BASE_STATE;

    [Header("Warp")]
    public List<Transform> randomWarpTransforms;
    public AudioClip warpSFX;
    [Range(0f, .5f)]
    public float sfxPitchRange;



    public void WarpToRandomPosition() {
    }
    public void Warp(Transform warpTransform) {
        // Play SFX
        float sfxPitch = 1 + Random.Range(-sfxPitchRange, sfxPitchRange);
        SFXPlayer.Instance.PlaySFX(warpSFX, transform.position, sfxPitch, VolumeManager.Instance.bossWarp);
    }
}
