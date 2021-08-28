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
    public BossChargeState chargeAttack;
    //public BaseState projectileAttack;

    public List<Transform> randomWarpTransforms;

    [Range(0f, 7f)]
    public float warpDelay;
    private float timeSinceLastWarp;

    [Range(0f, 5f)]
    public float attackDelay;
    private float timeSinceLastAttack;

    [Range(0, 5)]
    public int maxNumAttacksBeforeChargeAttack;
    public int attacksSinceChargeAttack { get; private set; }
    private Transform lastWarpTransform;

    protected override void Awake() {
        base.Awake();
        enemy = GetComponentInParent<BossEnemy>();
    }

    public override void OnStateEnter(BaseState previousState) {
        base.OnStateEnter(previousState);
        timeSinceLastAttack = 0f;
        timeSinceLastWarp = 0f;

        if (previousState != parentFSM.initialState) {
            // If we're just starting the fight, let the hidden state handle the initial warp
            //  (this helps sync the dialog and combat enemies)
            WarpToRandomTransform();
        }
        enemy.setIsFloating(true);
    }
    public override void OnStateExit(BaseState previousState) {
        base.OnStateEnter(previousState);
    }
    public override void OnStateUpdate() {
        base.OnStateUpdate();
        timeSinceLastAttack += Time.deltaTime;
        if (!enemy.isWarping) {
            timeSinceLastWarp += Time.deltaTime;
        }

        PerformNextAction();
    }

    private void PerformNextAction() {
        if (enemy.isWarping) return;

        // Every frame, see if it's been long enough that we can attack again.
        if (timeSinceLastAttack > attackDelay && !enemy.isWarping) {
            if (!enemy.DEBUG_NO_STATE_MULTIPLEX) {
                DoAttack();
            }
        }

        // If not, see if it's been long enough that we can warp 
        if (timeSinceLastWarp > warpDelay) {
            WarpToRandomTransform();
        }
    }





    public void WarpToRandomTransform() {
        if (randomWarpTransforms.Count == 0) {
            Debug.LogError("Tried to warp, but no warp transforms defined.");
            return;
        }

        timeSinceLastWarp = 0f;

        // Try a few times to find a warp transform that isn't the current one
        for (int i = 0; i < 5; i++) {
            int randomIndex = Random.Range(0, randomWarpTransforms.Count - 1);

            if (lastWarpTransform != randomWarpTransforms[randomIndex]) {
                lastWarpTransform = randomWarpTransforms[randomIndex];
                enemy.Warp(lastWarpTransform);
                return;
            }
        }

        // If that fails due to probability stuff, just pick the first one
        enemy.Warp(randomWarpTransforms[0]);
    }

    public void DoAttack() {
        BaseState nextAttack = GetNextAttack();

        if (nextAttack == (chargeAttack as BaseState)) {
            attacksSinceChargeAttack = 0;
        } else {
            attacksSinceChargeAttack++;

            // TODO: Uncomment
            if (rainAttack.isSpawningProjectiles) return;
        }

        parentFSM.TransitionTo(nextAttack);
        return;
    }

    public BaseState GetNextAttack() {
        // TODO: Uncomment once I have my charge attack defined
        if (attacksSinceChargeAttack >= maxNumAttacksBeforeChargeAttack) {
            return chargeAttack;
        }

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
