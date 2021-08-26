using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DialogInteractor : MonoBehaviour {
    private Animator animator;
    private int animHashIsOpen;
    public DialogEventChannel dialogEventChannel;

    private void Awake() {
        animator = GetComponent<Animator>();
        animHashIsOpen = Animator.StringToHash("isOpen");
    }

    private void OnEnable() {
        dialogEventChannel.onDialogStarted += Open;
        dialogEventChannel.onDialogFinished += Close;
    }

    private void OnDisable() {
        dialogEventChannel.onDialogStarted -= Open;
        dialogEventChannel.onDialogFinished -= Close;
    }

    public void Open() {
        animator.SetBool(animHashIsOpen, true);
    }
    public void Close() {
        animator.SetBool(animHashIsOpen, false);
    }
}
