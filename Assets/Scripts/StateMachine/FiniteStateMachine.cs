using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine : MonoBehaviour {
    public BaseState initialState;
    public BaseState deathState;
    public BaseState currentState { get; private set; }

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

    public void Update() {
        currentState.OnStateUpdate();
    }
    public void FixedUpdate() {
        currentState.OnStateFixedUpdate();
    }
}
