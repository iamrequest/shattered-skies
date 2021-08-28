using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BossAttackMultiplexerState))]
public class BossAttackMultiplexerStateInspector : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (Application.isPlaying) {
            BossAttackMultiplexerState attackState = target as BossAttackMultiplexerState;
            EditorGUILayout.LabelField("Attacks since charge: " + attackState.attacksSinceChargeAttack);
            EditorGUILayout.LabelField("Current State: " + attackState.parentFSM.currentState);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Charge State", EditorStyles.miniButtonLeft)) {
                attackState.parentFSM.TransitionTo(attackState.chargeAttack);
            }
            if (GUILayout.Button("Rain State", EditorStyles.miniButtonLeft)) {
                attackState.parentFSM.TransitionTo(attackState.rainAttack);
            }
            GUILayout.EndHorizontal();
        }
    }
}
