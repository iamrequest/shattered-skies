﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BossFightTrigger : MonoBehaviour {
    private int animHashHideForFight, animHashBossDefeated;
    public bool isFightInProgress { get; private set; }

    public PlayerDamageEventChannel playerDamageEventChannel;
    public BossDoor entryBossDoor, exitBossDoor;
    public Animator skyFieldAnimator;


    [Header("BGM")]
    public int battleSongIndex;

    [Header("Dialog")]
    public DialogManager preFightDialogManager;
    public Dialog preFightDialog;
    public DialogManager postFightDialogManager;
    public Dialog postFightDialog;
    public DialogInteractor dialogInteractor;
    public Transform postFightDialogInteractorTransform;

    [Header("Boss Config")]
    public BaseEnemy dialogEnemy;
    public BossEnemy bossEnemy;
    public float dialogEnemyPostDialogLifetime;

    private void Awake() {
        animHashHideForFight = Animator.StringToHash("hideForFight");
        animHashBossDefeated = Animator.StringToHash("onBossDefeated");

        isFightInProgress = false;
    }

    private void OnEnable() {
        // Pre-fight
        playerDamageEventChannel.onPlayerRevive += ResetArena;
        preFightDialog.onDialogStart.AddListener(CloseArena);
        preFightDialog.onDialogCompleted.AddListener(StartFight);

        // Just after the boss is defeated
        bossEnemy.GetComponent<Damageable>().onHealthDepleted.AddListener(OnBossDefeated);

        // Post-fight dialog
        postFightDialog.onDialogCompleted.AddListener(OnFinalDialogComplete);
        dialogEnemy.damageable.onHealthDepleted.AddListener(OnDialogBossDeath);
    }

    private void OnDisable() {
        // Pre-fight
        playerDamageEventChannel.onPlayerRevive -= ResetArena;
        preFightDialog.onDialogStart.RemoveListener(CloseArena);
        preFightDialog.onDialogCompleted.RemoveListener(StartFight);

        // Just after the boss is defeated
        bossEnemy.damageable.onHealthDepleted.RemoveListener(OnBossDefeated);

        // Post-fight dialog
        postFightDialog.onDialogCompleted.RemoveListener(OnFinalDialogComplete);
        dialogEnemy.damageable.onHealthDepleted.RemoveListener(OnDialogBossDeath);
    }

    public void OnTriggerEnter(Collider other) {
        // Only listen for the player for this trigger
        if (other.CompareTag(Player.Instance.tag)) {
            OnPlayerEnterArena();
        }
    }

    public void OnPlayerEnterArena() {
        if (isFightInProgress) return;

        // TODO: If the fight is in progress, do nothing here
        if (bossEnemy.damageable.isAlive) {
            if (preFightDialog.isComplete) {
                // Just jump straight to the fight
                CloseArena();
                StartFight();
            } else {
                // Do the dialog first. 
                preFightDialogManager.StartDialog(preFightDialog);
                BGMManager.Instance.FadeToStop();
            }
        }
    }

    public void CloseArena() {
        entryBossDoor.Close();
        exitBossDoor.Close();
    }

    public void StartFight() {
        isFightInProgress = true;

        BGMManager.Instance.PlaySong(battleSongIndex);

        dialogEnemy.animator.SetBool(animHashHideForFight, true);

        // Optional: Comment this out for easier fights
        bossEnemy.damageable.Heal(bossEnemy.damageable.healthMax);

        bossEnemy.fsm.TransitionTo(bossEnemy.multiplexerAttackState);
    }

    // On player death, restore the arena to its initial state
    public void ResetArena() {
        if (isFightInProgress) {
            BGMManager.Instance.FadeToStopThenPlay(BGMManager.Instance.initialSongIndex);
        }

        isFightInProgress = false;
        entryBossDoor.Open();

        if (bossEnemy.damageable.isAlive) {
            exitBossDoor.Open();
        } else {
            if (!dialogEnemy.damageable.isAlive) {
                // This solves a bug that causes the exit door to open if the player dies before the final dialog completes
                exitBossDoor.Open();
            }
        }

        dialogEnemy.animator.SetBool(animHashHideForFight, false);
        // TODO: Hide the battle boss
    }

    public void OpenArena() {
        entryBossDoor.Open();
        exitBossDoor.Open();
    }

    private void OnBossDefeated(BaseDamager arg0, Damageable arg1) {
        isFightInProgress = false;

        BGMManager.Instance.FadeToStop();

        dialogEnemy.animator.SetBool(animHashBossDefeated, true);

        // TODO: Consider doing this via a trigger
        dialogInteractor.transform.position = postFightDialogInteractorTransform.position;
        //postFightDialogManager.StartDialog(postFightDialog);
    }


    private void OnFinalDialogComplete() {
        // Make the dialog-enemy vulnerable
        dialogEnemy.damageable.damageTargetType = DamageTargets.ENEMY;

        StartCoroutine(DoKillDialogBossAfterDelay());
    }
    private IEnumerator DoKillDialogBossAfterDelay() {
        yield return new WaitForSeconds(dialogEnemyPostDialogLifetime);
        dialogEnemy.damageable.Kill();
    }

    private void OnDialogBossDeath(BaseDamager arg0, Damageable arg1) {
        postFightDialogManager.EndDialogEarly();
        OpenArena();
        skyFieldAnimator.SetTrigger("clear");
    }

    // Called via trigger listener
    public void StartPostFightDialog() {
        if (!bossEnemy.damageable.isAlive) {
            postFightDialogManager.StartDialog(postFightDialog);
        }
    }
}
