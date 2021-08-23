using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CrystalEnemyAttackState : BaseState {
    private BaseEnemy enemy;
    private Animator animator;
    private int animHashIsTrackingPlayer;

    public CrystalEnemyIdleState idleState;
    public MultiAimConstraint multiAimConstraint;
    public Transform lookatTarget;

    [Header("Timers")]
    public float attackDelay;
    private float elapsedAttackDelay;

    public float returnToIdleDelay;
    private float elapsedReturnToIdleDelay;

    [Header("Attack Logic")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnTransform;
    public float projectileForce;

    protected override void Awake() {
        base.Awake();
        enemy = parentFSM.GetComponent<BaseEnemy>();
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
                parentFSM.TransitionTo(idleState);
            }
        }

    }
    private void ProjectileAttack() {
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnTransform.position, projectileSpawnTransform.rotation);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        projectileRb.AddForce(projectileSpawnTransform.forward * projectileForce, ForceMode.VelocityChange);
    }
}
