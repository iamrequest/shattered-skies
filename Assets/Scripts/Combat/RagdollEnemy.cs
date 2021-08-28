using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollEnemy : MonoBehaviour {
    [HideInInspector]
    public Damageable damageable;
    [HideInInspector]
    public CharacterController characterController;

    public Animator animator;
    public List<Rigidbody> rigidbodies;

    public bool isRagdollActiveOnInit;
    public bool isRagdollActive { get; private set; }

    [SerializeField]
    private bool ragdollOnDeath = true;


    private void Awake() {
        damageable = GetComponent<Damageable>();
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable() {
        if(damageable && ragdollOnDeath) damageable.onHealthDepleted.AddListener(OnDeath);
    }

    private void OnDisable() {
        if(damageable && ragdollOnDeath) damageable.onHealthDepleted.RemoveListener(OnDeath);
    }

    private void Start() {
        // Ignore collisions between the ragdoll and the character controller. This was needed in previous whitebox tests, but I'm using NavMeshAgents now
        /*
        if(characterController) {
            Collider[] colliders = GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++) {
                Physics.IgnoreCollision(characterController, colliders[i]);
            }
        }
        */

        SetRagdollActive(isRagdollActiveOnInit);
    }


    private void OnDeath(BaseDamager baseDamager, Damageable damageable) {
        SetRagdollActive(true);
    }

    public void SetRagdollActive(bool isRagdollActive) {
        // Do nothing if we're trying to disable ragdoll, but the enemy is already dead
        if (!isRagdollActive && !damageable.isAlive) return;

        this.isRagdollActive = isRagdollActive;
        animator.enabled = !isRagdollActive;

        foreach(Rigidbody rb in rigidbodies) {
            rb.isKinematic = !isRagdollActive;
        }
    }
}
