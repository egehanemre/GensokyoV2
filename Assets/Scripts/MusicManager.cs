using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public List<AudioClip> combatMusic; // Changed from AudioClip to List<AudioClip>
    public AudioClip victoryMusic;
    public AudioClip defeatMusic;
    public AudioClip prepMusic;
    // Add more AudioClips as needed

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;
    }

    public void PlayMusic(AudioClip clip, bool loop = true, float fadeDuration = 1f)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeInMusicCoroutine(clip, loop, fadeDuration));
    }

    // New helper to play a combat music by index
    public void PlayCombatMusicByIndex(int index, float fadeDuration = 1f)
    {
        if (combatMusic != null && combatMusic.Count > 0)
        {
            if (index < 0 || index >= combatMusic.Count)
                index = 0;
            PlayMusic(combatMusic[index], true, fadeDuration);
        }
    }

    public void PlayMenuMusic(float fadeDuration = 1f)
    {
        PlayMusic(menuMusic, true, fadeDuration);
    }

    public void PlayVictoryMusic(float fadeDuration = 1f)
    {
        PlayMusic(victoryMusic, false, fadeDuration);
    }

    public void PlayDefeatMusic(float fadeDuration = 1f)
    {
        PlayMusic(defeatMusic, false, fadeDuration);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void FadeOutMusic(float duration = 1f)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = musicSource.volume;
        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }
        musicSource.volume = 0f;
        musicSource.Stop();
    }

    private IEnumerator FadeInMusicCoroutine(AudioClip newClip, bool loop, float duration)
    {
        // Fade out current music if playing
        if (musicSource.isPlaying && musicSource.volume > 0f)
        {
            float startVolume = musicSource.volume;
            float time = 0f;
            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
                yield return null;
            }
            musicSource.volume = 0f;
            musicSource.Stop();
        }

        // Set new clip and fade in
        musicSource.clip = newClip;
        musicSource.loop = loop;
        musicSource.Play();
        float targetVolume = 1f;
        float fadeInTime = 0f;
        while (fadeInTime < duration)
        {
            fadeInTime += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, fadeInTime / duration);
            yield return null;
        }
        musicSource.volume = targetVolume;
    }
}