using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StaminaManager))]
public class StaminaManagerInspector : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        StaminaManager staminaManager = target as StaminaManager;

        EditorGUILayout.Space();
        staminaManager.currentStamina = EditorGUILayout.Slider("Stamina", staminaManager.currentStamina, 0f, staminaManager.maxStamina);
    }
}