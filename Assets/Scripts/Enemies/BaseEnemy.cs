using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores references to common components that an enemy would have
/// </summary>
public class BaseEnemy : MonoBehaviour {
    [HideInInspector]
    public Damageable damageable;

    [HideInInspector]
    public EnemyVision vision;

    [HideInInspector]
    public Animator animator;

    [HideInInspector]
    public FiniteStateMachine fsm;

    private void Awake() {
        damageable = GetComponent<Damageable>();
        vision = GetComponent<EnemyVision>();
        animator = GetComponentInChildren<Animator>();
        fsm = GetComponent <FiniteStateMachine>();
    }
}
