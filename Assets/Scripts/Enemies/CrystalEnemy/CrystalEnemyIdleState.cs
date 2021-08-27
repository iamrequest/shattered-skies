using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CrystalEnemyIdleState : BaseState {
    private BaseEnemy enemy;
    private Animator animator;
    private int animHashIsTrackingPlayer;

    public CrystalEnemyAttackState attackState;
    public MultiAimConstraint multiAimConstraint;
    public Transform lookatTarget;

    public float attackDelay;
    private float elapsedAttackDelay;

    [Header("SFX")]
    public AudioClip onPlayerSpottedSFX;
    [Range(0f, .5f)]
    public float playerSpottedPitchRange;

    protected override void Awake() {
        base.Awake();
        enemy = parentFSM.GetComponent<BaseEnemy>();
        animator = parentFSM.GetComponentInChildren<Animator>();
        animHashIsTrackingPlayer = Animator.StringToHash("isTrackingPlayer");
    }

    public override void OnStateEnter(BaseState previousState) {
        base.OnStateEnter(previousState);

        multiAimConstraint.weight = 0f;
        animator.SetBool(animHashIsTrackingPlayer, false);
    }

    public override void OnStateUpdate() {
        base.OnStateUpdate();

        // Check if we can see the enemy
        if (enemy.vision.isPlayerInSight()) {
            if (elapsedAttackDelay == 0f) { 
                float sfxPitch = 1 + Random.Range(-playerSpottedPitchRange, playerSpottedPitchRange);
                SFXPlayer.Instance.PlaySFX(onPlayerSpottedSFX, transform.position, sfxPitch, VolumeManager.Instance.crystalPlayerSpotted);
            }

            animator.SetBool(animHashIsTrackingPlayer, true);
            multiAimConstraint.weight = 1f; // TODO: Lerp this. May need to lerp anim layer down to 0 at the same time

            lookatTarget.position = Player.Instance.cam.transform.position;
            elapsedAttackDelay += Time.deltaTime;
        } else {
            elapsedAttackDelay = Mathf.Clamp(elapsedAttackDelay - Time.deltaTime / 2, 0f, attackDelay);

            if (elapsedAttackDelay == 0f) {
                animator.SetBool(animHashIsTrackingPlayer, false); 
            }
        }

        if (elapsedAttackDelay >= attackDelay) {
            animator.SetBool(animHashIsTrackingPlayer, true);
            parentFSM.TransitionTo(attackState);
        }
    }
}
