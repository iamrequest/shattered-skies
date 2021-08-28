using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Freya;
using System;

public class BossRainAttackState : BaseState {
    private BaseEnemy baseEnemy;
    public PlayerDamageEventChannel playerDamagedEventChannel;
    public BaseState postAttackState;

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

    public float initialChannelDuration;
    public Vector2 postChannelIdleDuration;

    // The rate of spawning will vary based on how much health the boss has left
    public Vector2 spawnDurationRange;
    public Vector2 spawnRateRange;

    private Coroutine spawnProjectilesCoroutine, returnToBaseStateCoroutine;
    public bool isSpawningProjectiles {
        get {
            return spawnProjectilesCoroutine != null;
        }
    }

    protected override void Awake() {
        base.Awake();
        baseEnemy = GetComponentInParent<BaseEnemy>();
    }

    private void OnEnable() {
        playerDamagedEventChannel.onPlayerDeath += OnPlayerDeath;
        baseEnemy.damageable.onHealthDepleted.AddListener(OnBossDeath);
    }
    private void OnDisable() {
        playerDamagedEventChannel.onPlayerDeath -= OnPlayerDeath;
        baseEnemy.damageable.onHealthDepleted.RemoveListener(OnBossDeath);
    }

    public override void OnStateEnter(BaseState previousState) {
        base.OnStateEnter(previousState);

        // TODO: We shouldn't start this case at all, if we're already spawning things
        if (isSpawningProjectiles) {
            Debug.LogError("We tried to jump to the rain attack, but we're already in the middle of spawning projectiles");
            parentFSM.TransitionTo(postAttackState);
        }

        CancelSpawning();

        spawnProjectilesCoroutine = StartCoroutine(DoSpawnProjectiles());
    }

    public override void OnStateExit(BaseState previousState) {
        base.OnStateExit(previousState);

        if (returnToBaseStateCoroutine != null) {
            StopCoroutine(returnToBaseStateCoroutine);
        }
    }

    public IEnumerator DoSpawnProjectiles() {
        // Evaluate spawn rate/duration based on boss health at the start of this state
        float spawnDuration = Mathfs.RemapClamped(0f, 1f, spawnDurationRange.x, spawnDurationRange.y, baseEnemy.damageable.getHealthPercentage);
        float spawnRate = Mathfs.RemapClamped(0f, 1f, spawnRateRange.x, spawnRateRange.y, baseEnemy.damageable.getHealthPercentage);

        float elapsedSpawnDuration = 0f, timeSinceLastSpawn = 0f;

        // TODO: Channel animation
        baseEnemy.animator.SetTrigger("isChanneling");

        yield return new WaitForSeconds(initialChannelDuration);
        returnToBaseStateCoroutine = StartCoroutine(ReturnToBaseStateAfterDelay());

        while (elapsedSpawnDuration < spawnDuration) {
            elapsedSpawnDuration += Time.deltaTime;
            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn >= spawnRate) {
                SpawnProjectile();
                timeSinceLastSpawn = 0f;
            }
        }
    }

    public IEnumerator ReturnToBaseStateAfterDelay() {
        float returnToStateDelay = Mathfs.RemapClamped(0f, 1f, postChannelIdleDuration.x, postChannelIdleDuration.y, baseEnemy.damageable.getHealthPercentage);
        yield return new WaitForSeconds(returnToStateDelay);
        parentFSM.TransitionTo(postAttackState);
    }

    public void CancelSpawning() {
        if (spawnProjectilesCoroutine != null) StopCoroutine(spawnProjectilesCoroutine);
    }
    public void CancelAllCoroutines() {
        CancelSpawning();
        if (returnToBaseStateCoroutine != null) StopCoroutine(returnToBaseStateCoroutine);
        // TODO: Confirm that this doesn't break channel animation
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spawnCenter.position, spawnRadius);
        Gizmos.DrawLine(spawnCenter.position, spawnCenter.position + Vector3.up * spawnYOffset);
    }
}
