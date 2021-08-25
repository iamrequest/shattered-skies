using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DialogSpeakerSettings")]
public class DialogSpeakerSettings : ScriptableObject {
    public string speakerName;
    public AudioClip letterTypedSFX;
}
