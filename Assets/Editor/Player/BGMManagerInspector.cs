using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BGMManager))]
public class BGMManagerInspector : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (Application.isPlaying) {
            BGMManager bgmM = target as BGMManager;
            BGMEventChannel bgmEC = bgmM.bgmEventChannel;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Play", EditorStyles.miniButtonLeft)) {
                bgmEC.RaiseOnPlay();
            }
            if (GUILayout.Button("Stop", EditorStyles.miniButtonMid)) {
                bgmEC.RaiseOnStop();
            }
            if (GUILayout.Button("Fade Stop", EditorStyles.miniButtonMid)) {
                bgmEC.RaiseOnFadeToStop();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            for (int i = 0; i < bgmM.songs.Count; i++) {
                if (GUILayout.Button($"Play song {i}", EditorStyles.miniButtonRight)) {
                    bgmEC.RaiseOnPlay(i);
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
