using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SkyShardAnimation : MonoBehaviour {
    private Animator animator;
    private int animHashFall;
    private bool hasFallen;

    [Range(0f, 2f)]
    public float fallDelay;
    public AudioClip fallSFX, impactSFX;

    [Range(0f, .5f)]
    public float sfxPitchRange;

    private void Awake() {
        animator = GetComponent<Animator>();
        animHashFall = Animator.StringToHash("fall");
    }

    public void StartFall() {
        if (hasFallen) return;

        hasFallen = true;
        animator.SetTrigger(animHashFall);
        StartCoroutine(PlaySFX());
    }

    private IEnumerator PlaySFX() {
        float sfxPitch = 1 + Random.Range(-sfxPitchRange, sfxPitchRange);

        SFXPlayer.Instance.PlaySFX(fallSFX, transform.position, sfxPitch, VolumeManager.Instance.skyShardFall);
        yield return new WaitForSeconds(fallDelay);
        SFXPlayer.Instance.PlaySFX(impactSFX, transform.position, sfxPitch, VolumeManager.Instance.skyShardImpact);
    }
}
