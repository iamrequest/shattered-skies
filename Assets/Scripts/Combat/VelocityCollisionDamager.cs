using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Freya;

public class VelocityCollisionDamager : BaseDamager {
    public Rigidbody rb { get; private set; }
    public float minDamage, maxDamage;
    public float minDamagingSpeed, maxDamagingSpeed;

    [Header("SFX")]
    [Range(0f, 1f)]
    public float pitchRange;
    public Vector2 volumeRange;

    // Damage applied sfx
    public AudioClip onDamageSFX;
    [Range(0f, 1f)]
    public float dmgSFXCooldown;
    private float lastDmgSFXPlayed;

    // Env collision sfx
    public LayerMask envLayerMask;
    public AudioClip onEnvCollisionSFX;

    [Range(0f, 1f)]
    public float envCollisionSFXCooldown;
    private float lastEnvCollisionSFXPlayed;


    private void Awake() {
        rb = GetComponentInParent<Rigidbody>();
    }
    private void Update() {
        lastEnvCollisionSFXPlayed += Mathf.Clamp(lastEnvCollisionSFXPlayed + Time.deltaTime, 0f, envCollisionSFXCooldown);
        lastDmgSFXPlayed += Mathf.Clamp(lastDmgSFXPlayed + Time.deltaTime, 0f, dmgSFXCooldown);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.TryGetComponent(out Damageable damageable)) {
            ApplyDamage(damageable, collision.relativeVelocity.magnitude);
            return;
        } else {
            Damageable damageableInParent = collision.collider.GetComponentInParent<Damageable>();
            if (damageableInParent) {
                ApplyDamage(damageableInParent, collision.relativeVelocity.magnitude);
                return;
            }
        }

        // No damageable here. 
        // Check if we collided with environment (There's probably a cleaner way of doing this), and play SFX
        if (onEnvCollisionSFX) {
            if (lastEnvCollisionSFXPlayed >= envCollisionSFXCooldown) {
                lastEnvCollisionSFXPlayed = 0f;

                if ((envLayerMask.value & (1 << collision.collider.gameObject.layer)) > 0) {
                    float sfxPitch = 1 + Freya.Random.Range(-pitchRange, pitchRange);
                    SFXPlayer.Instance.PlaySFX(onEnvCollisionSFX, 
                        transform.position, 
                        sfxPitch, 
                    Mathfs.InverseLerpClamped(volumeRange.x, volumeRange.y, collision.relativeVelocity.magnitude));
                }
            }
        }
    }

    public void ApplyDamage(Damageable damageable, float relativeVelocityMagnitude) {
        // Do not apply damage if the target cannot be damaged by this damage type
        if (!isValidDamageTarget(damageable.damageTargetType)) return;

        float damage = CalculateDamage(relativeVelocityMagnitude);

        // Play SFX
        if (onDamageSFX) {
            if (lastDmgSFXPlayed >= dmgSFXCooldown) {
                lastDmgSFXPlayed = 0f;
                float sfxPitch = 1 + Freya.Random.Range(-pitchRange, pitchRange);
                SFXPlayer.Instance.PlaySFX(onDamageSFX,
                    transform.position,
                    sfxPitch,
                Mathfs.InverseLerpClamped(volumeRange.x, volumeRange.y, relativeVelocityMagnitude));
            }
        }

        if (damage == 0f) {
            Debug.Log("No damage applied.");
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
