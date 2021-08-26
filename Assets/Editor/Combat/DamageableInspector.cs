using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Damageable))]
public class DamageableInspector : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (Application.isPlaying) {
            GUILayout.BeginHorizontal();
            Damageable damageable = target as Damageable;
            if (GUILayout.Button("Kill", EditorStyles.miniButtonLeft)) {
                damageable.ApplyDamage(damageable.healthMax, null);
            }
            if (GUILayout.Button("Trigger Killplane", EditorStyles.miniButtonLeft)) {
                damageable.OnKillPlaneEntered();
            }
            if (GUILayout.Button("Revive", EditorStyles.miniButtonMid)) {
                damageable.Revive();
            }
            if (GUILayout.Button("+1 Damage", EditorStyles.miniButtonRight)) {
                damageable.ApplyDamage(1, null);
            }
            GUILayout.EndHorizontal();
        }
    }

    private void OnSceneGUI() {
        Damageable damageable = target as Damageable;
        Vector3 textOffset = new Vector3(0f, 1.2f, 0f);


        Handles.Label(damageable.transform.position + textOffset,
            $"Health: {damageable.healthCurrent} / {damageable.healthMax}");
    }
}
