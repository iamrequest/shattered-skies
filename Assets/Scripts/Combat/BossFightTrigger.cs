using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BossFightTrigger : MonoBehaviour {
    public PlayerDamageEventChannel playerDamageEventChannel;

    public DialogManager dialogManager;
    public Dialog dialog;
    public BossDoor entryBossDoor, exitBossDoor;
    public Animator dialogBossAnimator;

    private int animHashHide;

    [Header("Boss Config")]
    public Damageable bossDamageable;

    private void Awake() {
        animHashHide = Animator.StringToHash("hide");
    }

    private void OnEnable() {
        playerDamageEventChannel.onPlayerDeath += ResetArena;
        dialog.onDialogStart.AddListener(CloseArena);
        dialog.onDialogCompleted.AddListener(StartFight);

        bossDamageable.onHealthDepleted.AddListener(OnBossDeath);
    }

    private void OnDisable() {
        playerDamageEventChannel.onPlayerDeath -= ResetArena;
        dialog.onDialogStart.RemoveListener(CloseArena);
        dialog.onDialogCompleted.RemoveListener(StartFight);

        bossDamageable.onHealthDepleted.RemoveListener(OnBossDeath);
    }

    public void OnTriggerEnter(Collider other) {
        // Only listen for the player for this trigger
        if (!other.CompareTag(Player.Instance.tag)) {
            return;
        }

        if (bossDamageable.isAlive) {
            if (dialog.isComplete) {
                // Just jump straight to the fight
                CloseArena();
                StartFight();
            } else {
                // Do the dialog first. 
                dialogManager.StartDialog(dialog);
            }
        }
    }

    public void CloseArena() {
        entryBossDoor.Close();
        exitBossDoor.Close();
    }

    public void StartFight() {
        // Optional: Comment this out for easier fights
        bossDamageable.Heal(bossDamageable.healthMax);

    }

    public void ResetArena() {
        entryBossDoor.Open();
        exitBossDoor.Open();

        // TODO: Only set this if the enemy is still alive
        dialogBossAnimator.SetBool(animHashHide, false);

        // TODO: Hide the battle boss
    }

    public void OpenArenaForVictory() {
        entryBossDoor.Open();
        exitBossDoor.Open();
    }

    private void OnBossDeath(BaseDamager arg0, Damageable arg1) {
        OpenArenaForVictory();

        // TODO: Consider adding post-death dialog
    }
}
