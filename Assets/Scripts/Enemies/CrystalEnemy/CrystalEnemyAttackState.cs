using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CrystalEnemyAttackState : BaseState {
    private BossEnemy enemy;
    private Animator animator;
    private int animHashIsTrackingPlayer;

    public CrystalEnemyIdleState idleState;
    public MultiAimConstraint multiAimConstraint;
    public Transform lookatTarget;

    [Header("SFX")]
    public AudioClip projectileFiredSFX;
    [Range(0f, .5f)]
    public float projectileFiredPitchRange;

    [Header("Timers")]
    public float attackDelay;
    private float elapsedAttackDelay;

    public float returnToIdleDelay;
    private float elapsedReturnToIdleDelay;

    [Header("Attack Logic")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnTransform;
    public float projectileForce;
    public Vector3 randomProjectileOffset;

    protected override void Awake() {
        base.Awake();
        enemy = parentFSM.GetComponent<BossEnemy>();
        animator = parentFSM.GetComponentInChildren<Animator>();
        animHashIsTrackingPlayer = Animator.StringToHash("isTrackingPlayer");
    }

    public override void OnStateEnter(BaseState previousState) {
        base.OnStateEnter(previousState);

        multiAimConstraint.weight = 1f;
        animator.SetBool(animHashIsTrackingPlayer, true);
    }

    public override void OnStateExit(BaseState previousState) {
        multiAimConstraint.weight = 0f;
        animator.SetBool(animHashIsTrackingPlayer, false);
    }

    public override void OnStateUpdate() {
        base.OnStateUpdate();

        elapsedAttackDelay += Time.deltaTime;

        // Check if we can see the enemy
        if (enemy.vision.isPlayerInSight()) {
            elapsedReturnToIdleDelay = 0f;
            lookatTarget.position = Player.Instance.cam.transform.position;

            if (elapsedAttackDelay >= attackDelay) {
                ProjectileAttack();
                elapsedAttackDelay = 0f;
            }
        } else {
            // If we haven't seen the player in a while, return to the idle state
            elapsedReturnToIdleDelay += Time.deltaTime;
            if (elapsedReturnToIdleDelay >= returnToIdleDelay) {
                if (!enemy.DEBUG_DO_NOT_RETURN_TO_BASE_STATE) {
                    parentFSM.TransitionTo(idleState);
                }
            }
        }

    }
    private void ProjectileAttack() {
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnTransform.position, projectileSpawnTransform.rotation);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        float sfxPitch = 1 + Random.Range(-projectileFiredPitchRange, projectileFiredPitchRange);
        SFXPlayer.Instance.PlaySFX(projectileFiredSFX, transform.position, sfxPitch, VolumeManager.Instance.crystalProjectileFired);

        // Add a random angular offset. Pretty sure this is in world space, not local space
        // I think the rotation of the enemy also makes this weird
        Vector3 fireDir = projectileSpawnTransform.forward;
        fireDir = Quaternion.Euler(GetRandomOffset(randomProjectileOffset.x), 
            GetRandomOffset(randomProjectileOffset.y), 
            GetRandomOffset(randomProjectileOffset.z)) 
            * fireDir;

        projectileRb.AddForce(fireDir * projectileForce, ForceMode.VelocityChange);
    }

    private float GetRandomOffset(float offset) {
        return Random.Range(-offset, offset);
    }



    public void OnDrawGizmosSelected() {
        // Guessing the projectile lifetime here, since it's on the prefab
        float projectileFullDistance = projectileForce * 5f;

        // I think this is wrong, since it produces weird angles depending on which dir the enemy is facing.
        // But it's good enough for visualization
        // Just realized, this is a world space rotation offset, not local space. 
        Vector3 v = Quaternion.Euler(randomProjectileOffset.x, 0f, 0f) * projectileSpawnTransform.forward;
        Debug.DrawRay(projectileSpawnTransform.position, v * projectileFullDistance, Color.magenta);
        v = Quaternion.Euler(-randomProjectileOffset.x, 0f, 0f) * projectileSpawnTransform.forward;
        Debug.DrawRay(projectileSpawnTransform.position, v * projectileFullDistance, Color.magenta);

        v = Quaternion.Euler(0f, randomProjectileOffset.y, 0f) * projectileSpawnTransform.forward;
        Debug.DrawRay(projectileSpawnTransform.position, v * projectileFullDistance, Color.magenta);
        v = Quaternion.Euler(0f, -randomProjectileOffset.y, 0f) * projectileSpawnTransform.forward;
        Debug.DrawRay(projectileSpawnTransform.position, v * projectileFullDistance, Color.magenta);
    }
}
