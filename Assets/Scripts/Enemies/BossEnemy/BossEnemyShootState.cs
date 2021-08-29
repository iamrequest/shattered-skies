using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BossEnemyShootState : BaseState {
    private BossEnemy enemy;
    private int animHashIsShootingProjectiles;

    public List<Transform> randomWarpTransforms;

    [Header("SFX and VFX")]
    public VisualEffect handVFX;
    public AudioClip projectileFiredSFX;
    public float pitchRange;

    [Header("Timers")]
    public Vector2 initialWaitDuration;
    [Tooltip("How long are we shooting for")]
    public Vector2 returnToStateDelay;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnTransform;
    public Vector2 shotDelayRange;

    private float timeSinceLastShot;

    private Coroutine shootProjectilesCoroutine, returnToStateCoroutine;

    protected override void Awake() {
        base.Awake();
        enemy = GetComponentInParent<BossEnemy>();
        animHashIsShootingProjectiles = Animator.StringToHash("isShootingProjectiles");
    }

    public override void OnStateEnter(BaseState previousState) {
        base.OnStateEnter(previousState);
        timeSinceLastShot = 0f;

        WarpToRandomTransform();
        enemy.isLookingAtPlayer = true;
        enemy.setIsFloating(true);

        shootProjectilesCoroutine = StartCoroutine(DoShootProjectiles());
    }

    public override void OnStateExit(BaseState previousState) {
        base.OnStateExit(previousState);
        // Stop coroutines
        if (shootProjectilesCoroutine != null) {
            StopCoroutine(shootProjectilesCoroutine);
            shootProjectilesCoroutine = null;
        }

        if (returnToStateCoroutine == null) {
            StopCoroutine(returnToStateCoroutine);
            returnToStateCoroutine = null;
        }

        // Stop animations
        enemy.animator.SetBool(animHashIsShootingProjectiles, false);
        handVFX.Stop();
    }

    public override void OnStateUpdate() {
        base.OnStateUpdate();
        if (!enemy.isWarping) {
            timeSinceLastShot += Time.deltaTime;
        }
    }


    public IEnumerator DoShootProjectiles() {
        // Wait for warp to finish
        yield return new WaitForSeconds(enemy.warpDuration * 2);

        // Start shoot animation
        enemy.animator.SetBool(animHashIsShootingProjectiles, true);
        handVFX.Play();

        // Wait for initial delay before shooting
        yield return new WaitForSeconds(enemy.getDamageScaledValue(initialWaitDuration));

        // Start the timer to go back to the base state
        returnToStateCoroutine = StartCoroutine(ReturnToBaseStateAfterDelay());

        // Shoot projectiles until we return to the parent state
        float shotDelay = enemy.getDamageScaledValue(shotDelayRange);
        while(true) {
            timeSinceLastShot += Time.deltaTime;

            if (timeSinceLastShot >= shotDelay) {
                timeSinceLastShot = 0f;
                ShootProjectile();
            }

            if (returnToStateCoroutine == null) {
                // Something weird happened, bail out now
                parentFSM.TransitionTo(enemy.multiplexerAttackState);
            }

            yield return null;
        }
    }

    public IEnumerator ReturnToBaseStateAfterDelay() {
        yield return new WaitForSeconds(enemy.getDamageScaledValue(returnToStateDelay));
        parentFSM.TransitionTo(enemy.multiplexerAttackState);
    }

    public void ShootProjectile() {
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnTransform.position, projectileSpawnTransform.rotation);
        //TrackingProjectile trackingProjectile = projectile.GetComponent<TrackingProjectile>();

        projectile.transform.LookAt(Player.Instance.cam.transform.position);

        // Play SFX
        float sfxPitch = 1 + Random.Range(-pitchRange, pitchRange);
        SFXPlayer.Instance.PlaySFX(projectileFiredSFX, transform.position, sfxPitch, VolumeManager.Instance.bossProjectileShot);
    }


    // This is copied from the multiplexer state, it should be refactored into BossEnemy
    public void WarpToRandomTransform() {
        if (randomWarpTransforms.Count == 0) {
            Debug.LogError("Tried to warp, but no warp transforms defined.");
            return;
        }

        int randomIndex = Random.Range(0, randomWarpTransforms.Count - 1);
        enemy.Warp(randomWarpTransforms[randomIndex]);
    }
}
