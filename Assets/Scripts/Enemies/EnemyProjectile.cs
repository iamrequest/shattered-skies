using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {
    public float lifetime;
    public AudioClip projectileHitSFX;
    [Range(0f, .5f)]
    public float sfxPitchRange;

    private void Start() {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision) {
        float sfxPitch = 1 + Random.Range(-sfxPitchRange, sfxPitchRange);
        SFXPlayer.Instance.PlaySFX(projectileHitSFX, transform.position, sfxPitch, VolumeManager.Instance.crystalProjectileHit);

        Destroy(gameObject);
    }
}
