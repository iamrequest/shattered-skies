using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossDoor : MonoBehaviour {
    private Animator animator;
    private int animHashIsOpen;

    private void Awake() {
        animator = GetComponent<Animator>();
        animHashIsOpen = Animator.StringToHash("isOpen");
    }

    public void Open() {
        animator.SetBool(animHashIsOpen, true);
    }
    public void Close() {
        animator.SetBool(animHashIsOpen, false);
    }
}
