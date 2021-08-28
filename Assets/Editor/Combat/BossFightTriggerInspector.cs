using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BossFightTrigger))]
public class BossFightTriggerInspector : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (Application.isPlaying) {
            BossFightTrigger bft = target as BossFightTrigger;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Player Enter Arena", EditorStyles.miniButtonLeft)) {
                bft.OnPlayerEnterArena();
            }
            if (GUILayout.Button("Mark Pre-Fight Dialog Complete", EditorStyles.miniButtonLeft)) {
                bft.preFightDialog.isComplete = true;
            }
            if (GUILayout.Button("Pre-Fight Dialog Next", EditorStyles.miniButtonLeft)) {
                bft.preFightDialogManager.DisplayNextSentence();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close Arena", EditorStyles.miniButtonLeft)) {
                bft.CloseArena();
            }
            if (GUILayout.Button("Open Arena", EditorStyles.miniButtonLeft)) {
                bft.OpenArena();
            }
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Start Fight", EditorStyles.miniButtonLeft)) {
                bft.StartFight();
            }
            if (GUILayout.Button("Kill Player", EditorStyles.miniButtonLeft)) {
                Player.Instance.damageable.Kill();
            }
            if (GUILayout.Button("Kill Boss", EditorStyles.miniButtonLeft)) {
                bft.bossEnemy.damageable.Kill();
            }
            if (GUILayout.Button("Kill Dialog Enemy", EditorStyles.miniButtonLeft)) {
                bft.dialogEnemy.damageable.Kill();
            }
            GUILayout.EndHorizontal();

        }
    }
}
