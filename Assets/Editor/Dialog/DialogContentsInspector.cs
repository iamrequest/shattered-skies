using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(DialogContents))]
public class DialogContentsInspector : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        serializedObject.Update();

        DialogContents dialogContents = target as DialogContents;
        SerializedProperty sentences = serializedObject.FindProperty("sentences");

        EditorGUILayout.Space();

        if (dialogContents.sentences == null) dialogContents.sentences = new List<Sentence>();
        for (int i = 0; i < dialogContents.sentences.Count; i++) {
            SerializedProperty currentSentence = sentences.GetArrayElementAtIndex(i);

            GUILayout.BeginHorizontal();
            // TODO: I should add a label to this one
            currentSentence.FindPropertyRelative("clearExistingSentence").boolValue = 
                EditorGUILayout.Toggle(currentSentence.FindPropertyRelative("clearExistingSentence").boolValue);

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
            dialogContents.sentences.Add(new Sentence());
        }
        GUILayout.EndHorizontal();
        
        serializedObject.ApplyModifiedProperties();
    }
}
