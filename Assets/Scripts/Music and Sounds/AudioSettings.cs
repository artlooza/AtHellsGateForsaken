using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Audio Sources")]
    public AudioSource[] sfxSources; // Assign SFX sources or leave empty to find automatically

    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    public static float MusicVolume { get; private set; } = 1f;
    public static float SFXVolume { get; private set; } = 1f;

    void Start()
    {
        // Load saved settings
        MusicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        SFXVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        // Set slider values
        if (musicSlider != null)
        {
            musicSlider.value = MusicVolume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = SFXVolume;
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // Apply volumes
        ApplyMusicVolume();
        ApplySFXVolume();
    }

    public void SetMusicVolume(float volume)
    {
        MusicVolume = volume;
        PlayerPrefs.SetFloat(MUSIC_KEY, volume);
        ApplyMusicVolume();
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = volume;
        PlayerPrefs.SetFloat(SFX_KEY, volume);
        ApplySFXVolume();
    }

    private void ApplyMusicVolume()
    {
        // Apply to MusicManager if it exists
        if (MusicManager.Instance != null)
        {
            if (MusicManager.Instance.menuSource != null)
                MusicManager.Instance.menuSource.volume = MusicVolume;
            if (MusicManager.Instance.gameplaySource != null)
                MusicManager.Instance.gameplaySource.volume = MusicVolume;
        }
    }

    private void ApplySFXVolume()
    {
        // Apply to assigned SFX sources
        foreach (var source in sfxSources)
        {
            if (source != null)
                source.volume = SFXVolume;
        }
    }

    // Call this from other scripts when playing SFX to get the correct volume
    public static float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFX_KEY, 1f);
    }

    public static float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
    }
}
