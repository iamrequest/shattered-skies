using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Dialog : MonoBehaviour {
    public UnityEvent onDialogStart, onDialogCanceled, onDialogCompleted;
    public bool isComplete;
    public DialogContents dialogContents;
}
