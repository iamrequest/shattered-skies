using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BossEnemy))]
public class BossEnemyInspector : Editor {
    private Transform t;


    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (Application.isPlaying) {
            BossEnemy enemy = target as BossEnemy;

            GUILayout.BeginHorizontal();
            t = EditorGUILayout.ObjectField("Warp Transform", t, typeof(Transform), true) as Transform;
            if (GUILayout.Button("Warp To Transform", EditorStyles.miniButtonLeft)) {
                enemy.Warp(t);
            }
            GUILayout.EndHorizontal();

        }
    }
}
