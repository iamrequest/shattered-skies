using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Freya;
using System;

/// <summary>
/// This state can exit before the projectiles finish spawning.
/// This way, the enemy can start spawning rain projectiles, and then use some other attack in  meantime.
/// </summary>
public class BossRainAttackState : BaseState {
    private BossEnemy enemy;
    private int animHashIsChanneling, animHashPortalOpen;

    public PlayerDamageEventChannel playerDamagedEventChannel;
    public Transform channelingTransform;


    [Header("Portal VFX")]
    public Animator portalAnimator;

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform spawnCenter;

    [Range(0f, 5f)]
    public float projectileSpeed;


    [Header("Spawning")]
    [Range(0f, 50f)]
    public float spawnRadius;
    [Range(0f, 50f)]
    public float spawnYOffset;

    // The rate of spawning will vary based on how much health the boss has left
    public Vector2 spawnDurationRange;
    public Vector2 spawnRateRange;

    private Coroutine spawnProjectilesCoroutine, returnToBaseStateCoroutine;


    [Header("Animation duration")]
    [Range(0f, 5)]
    public float channelAnimationDuration;
    public Vector2 postChannelIdleDuration;


    public bool isSpawningProjectiles {
        get {
            return spawnProjectilesCoroutine != null;
        }
    }




    protected override void Awake() {
        base.Awake();
        enemy = GetComponentInParent<BossEnemy>();
        animHashIsChanneling = Animator.StringToHash("isChannelingRainAttack");
        animHashPortalOpen = Animator.StringToHash("isOpen");
    }

    private void OnEnable() {
        playerDamagedEventChannel.onPlayerDeath += OnPlayerDeath;
        enemy.GetComponent<Damageable>().onHealthDepleted.AddListener(OnBossDeath);
    }
    private void OnDisable() {
        playerDamagedEventChannel.onPlayerDeath -= OnPlayerDeath;
        enemy.GetComponent<Damageable>().onHealthDepleted.RemoveListener(OnBossDeath);
    }

    public override void OnStateEnter(BaseState previousState) {
        base.OnStateEnter(previousState);

        // TODO: We shouldn't start this case at all, if we're already spawning things
        if (isSpawningProjectiles) {
            Debug.LogError("We tried to jump to the rain attack, but we're already in the middle of spawning projectiles");
            parentFSM.TransitionTo(enemy.multiplexerAttackState);
        }

        // Warp to channeling position
        enemy.Warp(channelingTransform);
        enemy.setIsFloating(true);

        CancelSpawning();
        spawnProjectilesCoroutine = StartCoroutine(DoSpawnProjectiles());

        // Init portal VFX
        Vector3 portalPosition = channelingTransform.position;
        portalPosition.y = spawnYOffset;
    }

    public override void OnStateExit(BaseState previousState) {
        base.OnStateExit(previousState);

        if (returnToBaseStateCoroutine != null) {
            StopCoroutine(returnToBaseStateCoroutine);
        }

        enemy.animator.SetBool(animHashIsChanneling, false);
    }

    public IEnumerator DoSpawnProjectiles() {
        // Evaluate spawn rate/duration based on boss health at the start of this state
        float spawnDuration = Mathfs.RemapClamped(0f, 1f, spawnDurationRange.x, spawnDurationRange.y, enemy.damageable.getHealthPercentage);
        float spawnRate = Mathfs.RemapClamped(0f, 1f, spawnRateRange.x, spawnRateRange.y, enemy.damageable.getHealthPercentage);

        float elapsedSpawnDuration = 0f, timeSinceLastSpawn = 0f;

        // Wait for warp to complete
        yield return new WaitForSeconds(enemy.warpDuration * 2);

        // Start channeling
        enemy.animator.SetBool(animHashIsChanneling, true);
        portalAnimator.SetBool(animHashPortalOpen, true);
        yield return new WaitForSeconds(channelAnimationDuration);
        enemy.animator.SetBool(animHashIsChanneling, false);


        // Return to state after a delay
        if (!enemy.DEBUG_NO_STATE_RETURN) {
            returnToBaseStateCoroutine = StartCoroutine(ReturnToBaseStateAfterDelay());
        }


        // Start spawning projectiles
        while (elapsedSpawnDuration < spawnDuration) {
            elapsedSpawnDuration += Time.deltaTime;
            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn >= spawnRate) {
                SpawnProjectile();
                timeSinceLastSpawn = 0f;
            }
            yield return null;
        }

        CancelSpawning();
    }

    public IEnumerator ReturnToBaseStateAfterDelay() {
        float returnToStateDelay = Mathfs.RemapClamped(0f, 1f, postChannelIdleDuration.x, postChannelIdleDuration.y, enemy.damageable.getHealthPercentage);
        yield return new WaitForSeconds(returnToStateDelay);
        parentFSM.TransitionTo(enemy.multiplexerAttackState);
    }


    public void CancelSpawning() {
        if (spawnProjectilesCoroutine != null) {
            StopCoroutine(spawnProjectilesCoroutine);
            spawnProjectilesCoroutine = null;
        }
        portalAnimator.SetBool(animHashPortalOpen, false);
    }
    public void CancelAllCoroutines() {
        CancelSpawning();
        if (returnToBaseStateCoroutine != null) StopCoroutine(returnToBaseStateCoroutine);
    }

    public void SpawnProjectile() {
        Vector3 spawnPosition = spawnCenter.position;
        spawnPosition.x += UnityEngine.Random.Range(-spawnRadius, spawnRadius);
        spawnPosition.z += UnityEngine.Random.Range(-spawnRadius, spawnRadius);
        spawnPosition.y += spawnYOffset;

        GameObject projectile = Instantiate(projectilePrefab);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        projectile.transform.position = spawnPosition;

        projectileRb.AddForce(-Vector3.up * projectileSpeed, ForceMode.VelocityChange);
    }


    private void OnPlayerDeath() {
        CancelAllCoroutines();
    }

    private void OnBossDeath(BaseDamager arg0, Damageable arg1) {
        CancelAllCoroutines();
    }

    //private void OnDrawGizmos() {
    private void OnDrawGizmosSelected() {
        // Draw spawn range
        if (spawnCenter) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(spawnCenter.position, spawnRadius);
            Gizmos.DrawLine(spawnCenter.position, spawnCenter.position + Vector3.up * spawnYOffset);
        }
    }
}
