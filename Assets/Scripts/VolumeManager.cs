using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is just a quick and dirty class. I SHOULD create do a static enum and maybe a scriptable object, with a global SFX offset in some settings class
public class VolumeManager : MonoBehaviour {
    [Header("Crystal Enemy")]
    [Range(0f, 1f)]
    public float crystalPlayerSpotted;
    [Range(0f, 1f)]
    public float crystalProjectileFired, crystalProjectileHit;

    [Header("Lizard Enemy")]
    [Range(0f, 1f)]
    public float lizardPlayerSpotted;
    [Range(0f, 1f)]
    public float lizardStep, lizardAttack;

    [Header("Sky Shard")]
    [Range(0f, 1f)]
    public float skyShardFall;
    [Range(0f, 1f)]
    public float skyShardImpact;

    [Header("Boss")]
    [Range(0f, 1f)]
    public float bossWarp;
    [Range(0f, 1f)]
    public float bossProjectileShot;

    [Header("Environment")]
    [Range(0f, 1f)]
    public float bonfireLit;


    public static VolumeManager Instance { get; private set; }
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.LogError($"Multiple {GetType()} components detected. This is probably a bug.");
            Destroy(this);
        }
    }
}
