using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName ="Event Channels/BGM Event Channel")]
public class BGMEventChannel : ScriptableObject {
    [Tooltip("Plays a specific song")]
    public UnityAction<int> onPlaySong;
    public UnityAction onPlay, onStop, onFadeToStop;

    public void RaiseOnPlay(int newSongIndex) {
        if (onPlaySong != null) onPlaySong.Invoke(newSongIndex);
    }
    public void RaiseOnPlay() {
        if (onPlay != null) onPlay.Invoke();
    }
    public void RaiseOnStop() {
        if (onStop != null) onStop.Invoke();
    }
    public void RaiseOnFadeToStop() {
        if (onFadeToStop != null) onFadeToStop.Invoke();
    }
}
