using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDamager : MonoBehaviour {
    [Tooltip("The list of damageable targets that can recieve damage from this Damager")]
    public DamageTargets damageableTargetTypes;

    public bool isValidDamageTarget(DamageTargets targetDamageType) {
        return (targetDamageType & damageableTargetTypes) > 0;
    }
}
