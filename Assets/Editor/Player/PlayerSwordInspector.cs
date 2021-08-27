using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerSword))]
public class PlayerSwordInspector : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (Application.isPlaying) {
            GUILayout.BeginHorizontal();

            PlayerSword ps = target as PlayerSword;
            if (GUILayout.Button("Return to starting socket", EditorStyles.miniButtonLeft)) {
                ps.ReturnToStartingSocket();
            }
            if (GUILayout.Button("Return to active checkpoint", EditorStyles.miniButtonLeft)) {
                ps.ReturnToActiveCheckpoint();
            }
            if (GUILayout.Button("Return shoulder", EditorStyles.miniButtonLeft)) {
                ps.ReturnToShoulderSocket();
            }
            GUILayout.EndHorizontal();
        }
    }
}
