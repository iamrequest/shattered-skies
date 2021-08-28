using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Strategy:
///     - Misc attacks N times
///     - Charge attack. 
///       - For this attack, the enemy is on the ground, and therefore is vulnerable to attacks.
///         Therefore, this one comes less often, but it's guaranteed every N attacks.
///         
/// Note: The player can still damage the enemy in air by throwing their sword, but that's cool enough to leave in
/// </summary>
public class BossAttackMultiplexerState : BaseState {
    private BossEnemy enemy;
    public BossRainAttackState rainAttack;
    public BaseState chargeAttack;
    //public BaseState projectileAttack;

    public List<Transform> randomWarpTransforms;

    [Range(0f, 2f)]
    public float warpDelay;
    private float timeSinceLastWarp;

    [Range(0f, 5f)]
    public float attackDelay;
    private float timeSinceLastAttack;

    [Range(0, 5)]
    public int maxNumAttacksBeforeChargeAttack;
    private int attacksSinceChargeAttack = 0;

    protected override void Awake() {
        base.Awake();
        enemy = GetComponentInParent<BossEnemy>();
    }

    public override void OnStateEnter(BaseState previousState) {
        base.OnStateEnter(previousState);
        timeSinceLastWarp = 0f;
        timeSinceLastAttack = 0f;
    }
    public override void OnStateExit(BaseState previousState) {
        base.OnStateEnter(previousState);
    }
    public override void OnStateUpdate() {
        base.OnStateUpdate();
        timeSinceLastAttack += Time.deltaTime;
        timeSinceLastWarp += Time.deltaTime;

        PerformNextAction();
    }

    private void PerformNextAction() {
        if (enemy.isWarping) return;

        // Every frame, see if it's been long enough that we can attack again.
        if (timeSinceLastAttack > attackDelay) {
            BaseState nextAttack = GetNextAttack();

            if (nextAttack == chargeAttack) {
                attacksSinceChargeAttack = 0;
            } else {
                attacksSinceChargeAttack++;
            }

            parentFSM.TransitionTo(nextAttack);
            return;
        }

        // If not, see if it's been long enough that we can warp 
        if (timeSinceLastWarp > warpDelay) {
            if (randomWarpTransforms.Count == 0) {
                Debug.LogError("Tried to warp, but no warp transforms defined.");
                return;
            }

            int randomIndex = Random.Range(0, randomWarpTransforms.Count - 1);
            enemy.Warp(randomWarpTransforms[randomIndex]);
        }
    }

    public BaseState GetNextAttack() {
        // TODO: Uncomment once I have my charge attack defined
        //if (attacksSinceChargeAttack >= maxNumAttacksBeforeChargeAttack) {
        //    return chargeAttack;
        //}

        // TODO: This should be random chance.
        return rainAttack;
        //if (rainAttack.isSpawningProjectiles) {
        //    return projectileAttack;
        //} else {
        //    return rainAttack;
        //}
    }


    private void OnDrawGizmos() {
    //private void OnDrawGizmosSelected() {
        // Draw warp transforms
        foreach(Transform t in randomWarpTransforms) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(t.position, 1f);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(t.position, t.position + t.transform.forward );
        }
    }
}
