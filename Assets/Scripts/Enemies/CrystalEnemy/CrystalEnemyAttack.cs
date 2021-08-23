using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Freya;

public class CrystalEnemyAttack : StateMachineBehaviour {
    private CrystalEnemy enemy;
    public MultiAimConstraint multiAimConstraint;
    private Transform multiAimConstraintTarget;


    public float elapsedAttackDelay;

    public float returnToIdleDelay;
    public float elapsedReturnToIdleDelay;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        enemy = animator.GetComponentInParent<CrystalEnemy>();
        multiAimConstraint = animator.GetComponentInChildren<MultiAimConstraint>();
        multiAimConstraintTarget = multiAimConstraint.data.sourceObjects.GetTransform(0);

        elapsedAttackDelay = 0f;
        elapsedReturnToIdleDelay = 0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        // TODO: This isn't working for some reason
        multiAimConstraint.weight = 1f;

        if (enemy.vision.isPlayerInSight()) {
            multiAimConstraintTarget.position = Player.Instance.cam.transform.position;

            elapsedAttackDelay += Time.deltaTime;
            elapsedReturnToIdleDelay = Mathf.Clamp01(elapsedReturnToIdleDelay - Time.deltaTime / 2);
        } else {
            // Haven't seen the player in a while, return to idle
            elapsedReturnToIdleDelay += Time.deltaTime;

            if (elapsedReturnToIdleDelay >= returnToIdleDelay) {
                animator.SetBool("isTrackingPlayer", false);
            }
        }

        // Attack after some delay
        if (elapsedAttackDelay >= enemy.attackDelay) {
            ProjectileAttack();
            elapsedAttackDelay = 0f;
        }
    }

    private void ProjectileAttack() {
        GameObject projectile = Instantiate(enemy.projectilePrefab, enemy.projectileSpawnTransform.position, enemy.projectileSpawnTransform.rotation);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        projectileRb.AddForce(enemy.projectileSpawnTransform.forward * enemy.projectileForce, ForceMode.VelocityChange);
    }
}
