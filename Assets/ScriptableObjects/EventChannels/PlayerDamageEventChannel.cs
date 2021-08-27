using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Event Channels/Player Damage Event Channel")]
public class PlayerDamageEventChannel : ScriptableObject {
    public UnityAction onPlayerDeath;

    public void RaiseOnPlayerDeath() {
        if (onPlayerDeath != null) onPlayerDeath.Invoke();
    }
}
