using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemySFXType { Step }
public class EnemySFXPlayer : MonoBehaviour {
    public AudioClip stepSFX;

    public void PlaySFX(EnemySFXType sfxType) {
        switch (sfxType) {
            case EnemySFXType.Step:
                PlaySFX(sfxType, stepSFX);
                break;
            default:
                return;
        }
    }

    private void PlaySFX(EnemySFXType sfxType, AudioClip clip) {
        if (clip == null) {
            Debug.LogError($"Failed to play audio clip (not defined: {sfxType}).");
            return;
        }

        SFXPlayer.Instance.PlaySFX(clip, transform.position);
    }
}
