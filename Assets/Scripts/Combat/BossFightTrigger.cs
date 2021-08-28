using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BossFightTrigger : MonoBehaviour {
    public PlayerDamageEventChannel playerDamageEventChannel;

    public BossDoor entryBossDoor, exitBossDoor;
    public Animator dialogBossAnimator;

    private int animHashHideForFight, animHashBossDefeated;

    [Header("Dialog")]
    public DialogManager preFightDialogManager;
    public Dialog preFightDialog;
    public DialogManager postFightDialogManager;
    public Dialog postFightDialog;

    [Header("Boss Config")]
    public BaseEnemy dialogEnemy;
    public BossEnemy bossEnemy;

    private void Awake() {
        animHashHideForFight = Animator.StringToHash("hideForFight");
        animHashBossDefeated = Animator.StringToHash("onBossDefeated");
    }

    private void OnEnable() {
        playerDamageEventChannel.onPlayerDeath += ResetArena;
        preFightDialog.onDialogStart.AddListener(CloseArena);
        preFightDialog.onDialogCompleted.AddListener(StartFight);

        bossEnemy.damageable.onHealthDepleted.AddListener(OnBossDeath);
    }

    private void OnDisable() {
        playerDamageEventChannel.onPlayerDeath -= ResetArena;
        preFightDialog.onDialogStart.RemoveListener(CloseArena);
        preFightDialog.onDialogCompleted.RemoveListener(StartFight);

        bossEnemy.damageable.onHealthDepleted.RemoveListener(OnBossDeath);
    }

    public void OnTriggerEnter(Collider other) {
        // Only listen for the player for this trigger
        if (!other.CompareTag(Player.Instance.tag)) {
            return;
        }

        // TODO: If the fight is in progress, do nothing here

        if (bossEnemy.damageable.isAlive) {
            if (preFightDialog.isComplete) {
                // Just jump straight to the fight
                CloseArena();
                StartFight();
            } else {
                // Do the dialog first. 
                preFightDialogManager.StartDialog(preFightDialog);
            }
        }
    }

    public void CloseArena() {
        entryBossDoor.Close();
        exitBossDoor.Close();
    }

    public void StartFight() {
        dialogBossAnimator.SetBool(animHashHideForFight, true);

        // Optional: Comment this out for easier fights
        bossEnemy.damageable.Heal(bossEnemy.damageable.healthMax);

        // TODO: Consider adding an initial state, for a short initial delay 
        bossEnemy.fsm.TransitionTo(bossEnemy.multiplexerAttackState);
    }

    // On player death, restore the arena to its initial state
    public void ResetArena() {
        entryBossDoor.Open();
        exitBossDoor.Open();

        // TODO: Only set this if the enemy is still alive

        dialogBossAnimator.SetBool(animHashHideForFight, false);
        // TODO: Hide the battle boss
    }

    public void OpenArenaForVictory() {
        entryBossDoor.Open();
        exitBossDoor.Open();
    }

    private void OnBossDeath(BaseDamager arg0, Damageable arg1) {
        OpenArenaForVictory();

        // TODO: Consider adding post-death dialog
        dialogBossAnimator.SetBool(animHashBossDefeated, false);
    }
}
