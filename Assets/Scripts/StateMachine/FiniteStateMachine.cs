using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine : MonoBehaviour {
    private Damageable damageable;

    public BaseState initialState;
    public BaseState deathState;

    [Tooltip("If non-null, we will transition to this state upon receiving damage, if we're not already in that state.")]
    public BaseState damagedState;
    public BaseState currentState { get; private set; }


    private void Awake() {
        damageable = GetComponent<Damageable>();
    }

    private void OnEnable() {
        damageable.onDamageApplied.AddListener(TransitionToDamagedState);
    }

    private void OnDisable() {
        damageable.onDamageApplied.AddListener(TransitionToDamagedState);
    }


    private void Start() {
        currentState = initialState;

        initialState.enabled = true;
        initialState.OnStateEnter(null);
    }

    public void TransitionTo(BaseState newState) {
        currentState.OnStateExit(newState);

        newState.OnStateEnter(currentState);
        currentState = newState;
    }

    /// <summary>
    /// Useful for calling via UnityEvent
    /// </summary>
    public void TransitionToDeathState() {
        TransitionTo(deathState);
    }

    private void TransitionToDamagedState(float damage, BaseDamager damager, Damageable damageable) {
        if (damageable.isAlive && damagedState) {
            if (currentState != damageable) {
                TransitionTo(damagedState);
            }
        }
    }

    public void Update() {
        currentState.OnStateUpdate();
    }
    public void FixedUpdate() {
        currentState.OnStateFixedUpdate();
    }
}
