using HurricaneVR.Framework.Core.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This was necessary to play SFX via the animator, but it needs to be refactored.
public enum EnemySFXType { Step }
public class EnemySFXPlayer : MonoBehaviour {
    public AudioClip stepSFX;
    [Range(0f, .5f)]
    public float pitchRange;

    public void PlaySFX(EnemySFXType sfxType) {
        switch (sfxType) {
            case EnemySFXType.Step:
                PlaySFX(sfxType, stepSFX, VolumeManager.Instance.lizardStep);
                break;
            default:
                return;
        }
    }

    private void PlaySFX(EnemySFXType sfxType, AudioClip clip, float volume) {
        if (clip == null) {
            Debug.LogError($"Failed to play audio clip (not defined: {sfxType}).");
            return;
        }

        float sfxPitch = 1 + Random.Range(-pitchRange, pitchRange);
        SFXPlayer.Instance.PlaySFX(clip, transform.position, sfxPitch, volume);
    }
}
