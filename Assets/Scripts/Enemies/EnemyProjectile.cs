﻿using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VelocityCollisionDamager))]
public class EnemyProjectile : MonoBehaviour {
    public Rigidbody rb { get; private set;}
    public VelocityCollisionDamager damager { get; private set;}

    public float lifetime;
    public AudioClip projectileHitSFX;
    [Range(0f, .5f)]
    public float sfxPitchRange;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        damager = GetComponent<VelocityCollisionDamager>();
    }

    private void Start() {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision) {
        if (projectileHitSFX) {
            float sfxPitch = 1 + Random.Range(-sfxPitchRange, sfxPitchRange);
            SFXPlayer.Instance.PlaySFX(projectileHitSFX, transform.position, sfxPitch, VolumeManager.Instance.crystalProjectileHit);
        }

        Destroy(gameObject);
    }
}
