using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BossBarrier : MonoBehaviour {
    private SphereCollider m_collider;
    private Animator animator;
    private int animHashBarrierFlash;

    public LayerMask collisionMask;

    private void Awake() {
        m_collider = GetComponent<SphereCollider>();
        animator = GetComponentInParent<Animator>();
        animHashBarrierFlash = Animator.StringToHash("barrierFlash");
    }


    private void OnEnable() {
        m_collider.enabled = true;
    }

    private void OnDisable() {
        m_collider.enabled = false;
    }

    private void OnCollisionEnter(Collision collision) {
        if ((collisionMask.value & (1 << collision.collider.gameObject.layer)) > 0) {
            animator.SetTrigger("barrierFlash");
        }
    }
}
