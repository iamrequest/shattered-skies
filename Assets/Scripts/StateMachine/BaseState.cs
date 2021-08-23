using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseState : MonoBehaviour {
    [HideInInspector]
    public FiniteStateMachine parentFSM;
    public UnityEvent onStateEnter, onStateExit;

    protected virtual void Awake() {
        parentFSM = GetComponentInParent<FiniteStateMachine>();
    }

    public virtual void OnStateEnter(BaseState previousState) {
        onStateEnter.Invoke();
    }
    public virtual void OnStateExit(BaseState previousState) { 
        onStateExit.Invoke();
    }

    /// <summary>
    /// Called on every Update()
    /// </summary>
    public virtual void OnStateUpdate() { }

    /// <summary>
    /// Called on every FixedUpdate()
    /// </summary>
    public virtual void OnStateFixedUpdate() { }
}
