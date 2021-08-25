using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Event Channels/Dialog Event Channel")]
public class DialogEventChannel : ScriptableObject {
    public UnityAction onDialogStarted, onDialogFinished;
    public UnityAction onDialogNext, onDialogSkip;

    public void RaiseOnDialogStarted() {
        if (onDialogStarted != null) onDialogStarted.Invoke();
    }
    public void RaiseOnDialogFinished() {
        if (onDialogFinished != null) onDialogFinished.Invoke();
    }
    public void RaiseOnDialogNext() {
        if (onDialogNext != null) onDialogNext.Invoke();
    }
    public void RaiseOnDialogSkip() {
        if (onDialogSkip != null) onDialogSkip.Invoke();
    }
}
