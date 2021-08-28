using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BossDoor : MonoBehaviour {
    private Animator animator;
    private int animHashIsOpen;
    public PlayerDamageEventChannel playerDamageEventChannel;

    private void Awake() {
        animator = GetComponent<Animator>();
        animHashIsOpen = Animator.StringToHash("isOpen");
    }

    private void OnEnable() {
        playerDamageEventChannel.onPlayerDeath += Open;
    }
    private void OnDisable() {
        playerDamageEventChannel.onPlayerDeath -= Open;
    }

    public void Open() {
        animator.SetBool(animHashIsOpen, true);
    }
    public void Close() {
        animator.SetBool(animHashIsOpen, false);
    }
}
