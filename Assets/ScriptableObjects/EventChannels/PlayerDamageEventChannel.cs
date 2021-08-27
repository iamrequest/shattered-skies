using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Event Channels/Player Damage Event Channel")]
public class PlayerDamageEventChannel : ScriptableObject {
    public UnityAction onPlayerDeath, onPlayerRevive;

    /// <summary>
    /// Invoked on the frame that the player is killed
    /// </summary>
    public void RaiseOnPlayerDeath() {
        if (onPlayerDeath != null) onPlayerDeath.Invoke();
    }

    /// <summary>
    /// Invoked on the frame that the frame that the player respawns (just before the fade-in animation plays)
    /// </summary>
    public void RaiseOnPlayerRevive() {
        if (onPlayerRevive != null) onPlayerRevive.Invoke();
    }
}
