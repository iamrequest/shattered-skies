using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour {
    public DamageTargets damageTargetType;
    public float healthMax, healthCurrent;

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

    public virtual void ApplyDamage(float incomingDamage, BaseDamager damager) {
        if (!isAlive) return;

        healthCurrent -= incomingDamage;
        onDamageApplied.Invoke(incomingDamage, damager, this);

        if (healthCurrent <= 0) {
            onHealthDepleted.Invoke(damager, this);
        }
    }

    public void Kill() {
        ApplyDamage(healthMax, null);
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
        if (isAlive) return;
        healthCurrent = Mathf.Min(healthMax, newHealth);
    }
}
