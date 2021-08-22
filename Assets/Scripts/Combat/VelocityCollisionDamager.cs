using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityCollisionDamager : BaseDamager {
    public Rigidbody rb { get; private set; }
    public float minDamage, maxDamage;
    public float minDamagingSpeed, maxDamagingSpeed;

    private void Awake() {
        rb = GetComponentInParent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.TryGetComponent(out Damageable damageable)) {
            ApplyDamage(damageable, collision.relativeVelocity.magnitude);
        } else {
            Damageable damageableInParent = collision.collider.GetComponentInParent<Damageable>();
            if (damageableInParent) {
                ApplyDamage(damageableInParent, collision.relativeVelocity.magnitude);
            }
        }
    }

    public void ApplyDamage(Damageable damageable, float relativeVelocityMagnitude) {
        float damage = CalculateDamage(relativeVelocityMagnitude);

        if (damage == 0f) {
            //Debug.Log("No damage applied.");
            return;
        } else {
            Debug.Log($"Applying {damage} damage.");
            damageable.ApplyDamage(damage, this);
        }
    }

    private float CalculateDamage(float relativeVelocityMagnitude) {
        if (relativeVelocityMagnitude < minDamagingSpeed) return 0f;

        float damage = Remap(0f, maxDamagingSpeed, minDamage, maxDamage, relativeVelocityMagnitude);
        return damage;
    }
    
    public float Remap(float iMin, float iMax, float oMin, float oMax, float v) {
        float tmp = Mathf.InverseLerp(iMin, iMax, v);
        return Mathf.Lerp(oMin, oMax, tmp);
    }
}
