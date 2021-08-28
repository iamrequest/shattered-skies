using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BossRainAttackState))]
public class BossRainAttackStateInspector : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (Application.isPlaying) {
            BossRainAttackState attackState = target as BossRainAttackState;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Spawn Projectile", EditorStyles.miniButtonLeft)) {
                attackState.SpawnProjectile();
            }
            if (GUILayout.Button("Restart State", EditorStyles.miniButtonLeft)) {
                attackState.parentFSM.TransitionTo(attackState);
            }
            GUILayout.EndHorizontal();
        }
    }
}
