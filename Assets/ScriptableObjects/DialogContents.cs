using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DialogContents")]
public class DialogContents : ScriptableObject {
    public List<Sentence> sentences;
}
