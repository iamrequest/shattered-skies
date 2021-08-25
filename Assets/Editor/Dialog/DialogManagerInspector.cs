using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogManager))]
public class DialogManagerInspector : Editor {
    private Dialog tmpDialog;
    private Dialog d;

    public void OnValidate() {
        Dialog tmpDialog = target as Dialog;
        if (tmpDialog) {
            d = tmpDialog;
        }
    }

    public void Awake() {
        Dialog tmpDialog = target as Dialog;
        if (tmpDialog) {
            d = tmpDialog;
        }
    }


    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (Application.isPlaying) {
            DialogManager dm = target as DialogManager;

            GUILayout.BeginHorizontal();
            d = EditorGUILayout.ObjectField("Dialog", d, typeof(Dialog), true) as Dialog;
            if (GUILayout.Button("Start Dialog", EditorStyles.miniButtonLeft)) {
                dm.StartDialog(d);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Next Sentence", EditorStyles.miniButtonLeft)) {
                dm.DisplayNextSentence();
            }
            if (GUILayout.Button("Skip Sentence", EditorStyles.miniButtonMid)) {
                dm.SkipCurrentSentence();
            }
            if (GUILayout.Button("End Dialog Early", EditorStyles.miniButtonRight)) {
                dm.EndDialogEarly();
            }
            GUILayout.EndHorizontal();
        }
    }
}
