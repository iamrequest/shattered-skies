using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(DialogContents))]
public class DialogContentsInspector : Editor {
    private float labelWidth = 100f;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        serializedObject.Update();

        DialogContents dialogContents = target as DialogContents;
        SerializedProperty sentences = serializedObject.FindProperty("sentences");

        EditorGUILayout.Space();

        //labelWidth = EditorGUILayout.Slider(labelWidth, 0f, 100f);
        EditorGUIUtility.labelWidth = labelWidth;

        if (dialogContents.sentences == null) dialogContents.sentences = new List<Sentence>();
        for (int i = 0; i < dialogContents.sentences.Count; i++) {
            SerializedProperty currentSentence = sentences.GetArrayElementAtIndex(i);

            GUILayout.BeginHorizontal();
            currentSentence.FindPropertyRelative("dialogSpeedOverride").floatValue =
                EditorGUILayout.Slider(new GUIContent("Dialog Speed"), currentSentence.FindPropertyRelative("dialogSpeedOverride").floatValue, 0f, 0.5f);

            currentSentence.FindPropertyRelative("clearExistingSentence").boolValue =
                EditorGUILayout.Toggle("Clear Text", currentSentence.FindPropertyRelative("clearExistingSentence").boolValue);

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(currentSentence.FindPropertyRelative("speaker"));
            EditorGUILayout.PropertyField(currentSentence.FindPropertyRelative("charTypedSFXOverride"), new GUIContent("Char Typed SFX"));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            currentSentence.FindPropertyRelative("text").stringValue = EditorGUILayout.TextArea(currentSentence.FindPropertyRelative("text").stringValue);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("-")) {
                sentences.DeleteArrayElementAtIndex(i);
                break;
            }
        }

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Sentences: {dialogContents.sentences.Count}");
        if (GUILayout.Button("+")) {
            AddSentence();
        }
        GUILayout.EndHorizontal();
        
        serializedObject.ApplyModifiedProperties();
    }

    private void AddSentence() {
        DialogContents dialogContents = target as DialogContents;
        // Copy the previous key values
        Sentence s = new Sentence();
        if (dialogContents.sentences.Count > 0) {
            s.speaker = dialogContents.sentences[dialogContents.sentences.Count - 1].speaker;
        }

        dialogContents.sentences.Add(s);
    }
}
