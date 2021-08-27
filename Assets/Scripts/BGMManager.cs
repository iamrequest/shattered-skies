using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Freya;


[RequireComponent(typeof(AudioSource))]
public class BGMManager : MonoBehaviour {
    private AudioSource audioSource;

    public BGMEventChannel bgmEventChannel;
    public List<AudioClip> songs;

    // This should be a scriptable object (track list and volumes)
    [Range(0f, 1f)]
    public List<float> songVolumes;
    public int initialSongIndex;
    public int currentSongIndex;
    private float baseVolume;

    [Range(0f, 3f)]
    public float fadeDuration;
    public Coroutine fadeCoroutine;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        baseVolume = audioSource.volume;
    }

    private void Start() {
        if (audioSource.playOnAwake) {
            PlaySong(initialSongIndex);
        }
    }

    private void OnEnable() {
        bgmEventChannel.onPlay += Play;
        bgmEventChannel.onPlaySong += PlaySong;
        bgmEventChannel.onStop += Stop;
        bgmEventChannel.onFadeToStop += FadeToStop;
    }
    private void OnDisable() {
        bgmEventChannel.onPlay -= Play;
        bgmEventChannel.onStop -= Stop;
        bgmEventChannel.onPlaySong -= PlaySong;
        bgmEventChannel.onFadeToStop -= FadeToStop;
    }



    // TODO: Fade in/out
    public void Play() {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        if (!audioSource.isPlaying) {
            audioSource.clip = songs[currentSongIndex];
            audioSource.volume = songVolumes[currentSongIndex] * baseVolume;
            audioSource.Play();
        }
    }
    public void Stop() {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        audioSource.Stop();
    }
    public void FadeToStop() {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        StartCoroutine(DoFadeToStop());
    }

    private IEnumerator DoFadeToStop() {
        float t = 0f;
        float initialVolume = audioSource.volume;

        while (t < fadeDuration) {
            t += Time.deltaTime;
            audioSource.volume = Mathfs.LerpClamped(initialVolume, 0f, t / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
    }

    public void NextSong() {
        PlaySong((currentSongIndex + 1) % songs.Count);
    }
    public void PreviousSong() {
        if (currentSongIndex - 1 < 0) {
            PlaySong(songs.Count - 1);
        } else {
            PlaySong(currentSongIndex - 1);
        }
    }

    public void PlaySong(int index) {
        // Cheap way to avoid stack overflow (not necessary here since I'm not using UI to set BGM, like in Tall Wall)
        // if (index == currentSongIndex) return;

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        if (songs.Count < 0) {
            Debug.LogError("Attempted to play BGM, but no songs are available");
            return;
        }

        currentSongIndex = index;
        audioSource.clip = songs[currentSongIndex];

        // Set song volume
        if (songVolumes.Count - 1 < currentSongIndex) {
            Debug.LogError("No available volume for this song, defaulting to 1");
            audioSource.volume = baseVolume;
        } else {
            audioSource.volume = songVolumes[currentSongIndex] * baseVolume;
        }

        audioSource.Play();
    }
}
