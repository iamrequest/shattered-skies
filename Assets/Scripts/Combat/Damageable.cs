using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KillPlaneEnteredActions { DEPLETE_HEALTH }
public class Damageable : MonoBehaviour {
    public DamageTargets damageTargetType;
    public float healthMax, healthCurrent;

    [Range(0f, 1f)]
    public float invincibilityFrames = 0.1f;
    private float timeSinceLastDamaged;

    public AudioClip damagedSFX, deathSFX;
    public Transform audioSourceTransform;
    public KillPlaneEnteredActions killPlaneEnteredAction = KillPlaneEnteredActions.DEPLETE_HEALTH;

    public bool isAlive {
        get {
            return healthCurrent > 0;
        }
    }

    public OnHealthChangedEvent onDamageApplied, onHealed;
    public OnDeathEvent onHealthDepleted;

    // Start is called before the first frame update
    void Awake() {
        healthCurrent = healthMax;
    }

    private void Update() {
        if (timeSinceLastDamaged < invincibilityFrames) timeSinceLastDamaged += Time.deltaTime;
    }

    public virtual void ApplyDamage(float incomingDamage, BaseDamager damager, bool ignoreInvincibility = false) {
        if (!isAlive) return;
        if (!ignoreInvincibility) {
            if (timeSinceLastDamaged < invincibilityFrames) return;
        }

        healthCurrent = Mathf.Clamp(healthCurrent - incomingDamage, 0f, healthMax);
        onDamageApplied.Invoke(incomingDamage, damager, this);

        if (healthCurrent <= 0) {
            if(deathSFX) SFXPlayer.Instance.PlaySFX(deathSFX, audioSourceTransform.position);

            FiniteStateMachine fsm = GetComponent<FiniteStateMachine>();
            if (fsm != null && fsm.deathState != null) {
                fsm.TransitionToDeathState();
            }

            onHealthDepleted.Invoke(damager, this);
        } else {
            if(damagedSFX) SFXPlayer.Instance.PlaySFX(damagedSFX, audioSourceTransform.position);
        }
    }

    public void Kill() {
        if (isAlive) {
            // Just to be safe with floats
            ApplyDamage(healthMax + 1, null, true);
        }
    }

    public void Heal(float newHealth) {
        if (!isAlive) return;

        healthCurrent = Mathf.Clamp(healthCurrent + newHealth, healthCurrent, healthMax);
        onHealed.Invoke(newHealth, null, this);
    }

    public void Revive() {
        Revive(healthMax);
    }
    public void Revive(float newHealth) {
        //if (isAlive) return;
        healthCurrent = Mathf.Min(healthMax, newHealth);
    }

    
    public virtual void OnKillPlaneEntered() {
        switch (killPlaneEnteredAction) {
            case KillPlaneEnteredActions.DEPLETE_HEALTH:
                Kill();
                break;
            default:
                break;
        }
    }
}
