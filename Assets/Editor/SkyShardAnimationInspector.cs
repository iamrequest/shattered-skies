using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HurricaneVR.Framework.Core.Utils;

[CustomEditor(typeof(SkyShardAnimation))]
public class SkyShardAnimationInspector : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (Application.isPlaying) {
            SkyShardAnimation ssa = target as SkyShardAnimation; 

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Play", EditorStyles.miniButtonLeft)) {
                ssa.StartFall();
            }
            if (GUILayout.Button("Test SFX", EditorStyles.miniButtonLeft)) {
                SFXPlayer.Instance.PlaySFX(ssa.impactSFX, ssa.transform.position);
            }
            GUILayout.EndHorizontal();
        }
    }
}
