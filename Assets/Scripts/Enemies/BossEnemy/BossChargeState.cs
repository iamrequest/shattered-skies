﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossChargeState : BaseState {
    private BossEnemy enemy;
    private Coroutine chargeAtPlayerCoroutine;
    private int animHashRun, animHashRunAttack;

    public Transform warpTransform;

    [Range(0f, 3f)]
    public float initialWaitDuration;
    [Range(0f, 3f)]
    public float postAttackWaitDuration;

    [Range(0f, 7f)]
    public float runSpeed;
    [Range(0f, 1f)]
    public float rotateSpeed;

    [Range(0f, 3f)]
    public float stopDistance;

    protected override void Awake() {
        base.Awake();
        enemy = GetComponentInParent<BossEnemy>();

        animHashRun = Animator.StringToHash("isRunning");
        animHashRunAttack = Animator.StringToHash("runAttack");
    }

    public override void OnStateEnter(BaseState previousState) {
        base.OnStateEnter(previousState);

        // Warp to random transform
        enemy.Warp(warpTransform);
        enemy.setIsFloating(false);

        // Charge at player
        chargeAtPlayerCoroutine = StartCoroutine(ChargeAtPlayerAfterDelay());
    }

    public override void OnStateExit(BaseState previousState) {
        base.OnStateExit(previousState);
        if (chargeAtPlayerCoroutine != null) StopCoroutine(chargeAtPlayerCoroutine);
    }


    private IEnumerator ChargeAtPlayerAfterDelay() {
        yield return new WaitForSeconds(initialWaitDuration);

        enemy.animator.SetBool(animHashRun, true);

        float distanceToPlayer = GetDistanceToPlayer();
        while (distanceToPlayer > stopDistance) {
            // Rotate to face player
            Vector3 enemyToPlayer = (Player.Instance.playerController.transform.position - enemy.transform.position);
            enemyToPlayer = Vector3.ProjectOnPlane(enemyToPlayer, Vector3.up).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(enemyToPlayer, Vector3.up);

            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            // Move forward
            enemy.transform.position += enemy.transform.forward * runSpeed * Time.deltaTime;

            distanceToPlayer = GetDistanceToPlayer();
            yield return null;
        }

        DoAttack();

        yield return new WaitForSeconds(postAttackWaitDuration);
        parentFSM.TransitionTo(enemy.multiplexerAttackState);
    }

    private float GetDistanceToPlayer() {
        return (Player.Instance.playerController.transform.position - enemy.transform.position).magnitude;
    }

    public void DoAttack() {
        enemy.animator.SetBool(animHashRun, false);
        enemy.animator.SetTrigger(animHashRunAttack);
    }
}
