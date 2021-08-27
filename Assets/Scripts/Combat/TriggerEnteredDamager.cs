using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnteredDamager : BaseDamager {
    public float damage;

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Damageable damageable)) {
            ApplyDamage(damageable);
        } else {
            Damageable damageableInParent = other.GetComponentInParent<Damageable>();
            if (damageableInParent) {
                ApplyDamage(damageableInParent);
            }
        }
    }
    public void ApplyDamage(Damageable damageable) {
        // Do not apply damage if the target cannot be damaged by this damage type
        if (!isValidDamageTarget(damageable.damageTargetType)) return;

        if (damage == 0f) {
            //Debug.Log("No damage applied.");
            return;
        } else {
            //Debug.Log($"Applying {damage} damage.");
            damageable.ApplyDamage(damage, this);
        }
    }
}
