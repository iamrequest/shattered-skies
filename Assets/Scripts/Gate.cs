using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Gate : MonoBehaviour {
    private Animator animator;
    private int animHashOpen;

    private void Awake() {
        animator = GetComponent<Animator>();
        animHashOpen = Animator.StringToHash("open");
    }

    public void Open() {
        animator.SetTrigger(animHashOpen);
    }
}
