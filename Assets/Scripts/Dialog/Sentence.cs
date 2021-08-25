using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sentence  {
    public DialogSpeakerSettings speaker;
    public string text;
    public bool clearExistingSentence = true;

    [Range(0f, 1f)]
    [Tooltip("If non-zero, this dialog speed will be used when typing each character of the sentence.")]
    public float dialogSpeedOverride = 0f;

    public AudioClip charTypedSFXOverride;
}
