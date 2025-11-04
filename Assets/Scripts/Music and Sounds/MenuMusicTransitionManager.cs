using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Sources")]
    public AudioSource menuSource;     
    public AudioSource gameplaySource; 

    [Header("Audio Clips")]
    public AudioClip menuClip;         
    public AudioClip gameplayClip;    

    public float fadeSeconds = 2f;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        if (menuSource && menuClip)
        {
            menuSource.clip = menuClip;
            menuSource.volume = 1f;
            menuSource.loop = true;
            menuSource.Play();
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        if (s.name == "Forsaken") FadeToGameplay();
        else if (s.name == "Main Menu Scene IA") FadeToMenu();
    }

    public void FadeToGameplay()
    {
        if (!gameplaySource || !gameplayClip) return;
        gameplaySource.clip = gameplayClip;
        gameplaySource.volume = 0f;
        gameplaySource.loop = true;
        gameplaySource.Play();
        StopAllCoroutines();
        StartCoroutine(Crossfade(menuSource, gameplaySource, fadeSeconds));
    }

    public void FadeToMenu()
    {
        if (!menuSource || !menuClip) return;
        if (!menuSource.isPlaying)
        {
            menuSource.clip = menuClip;
            menuSource.volume = 0f;
            menuSource.loop = true;
            menuSource.Play();
        }
        StopAllCoroutines();
        StartCoroutine(Crossfade(gameplaySource, menuSource, fadeSeconds));
    }

    private IEnumerator Crossfade(AudioSource from, AudioSource to, float seconds)
    {
        float t = 0f;
        float fromStart = from ? from.volume : 0f;
        float toStart = to ? to.volume : 0f;

        while (t < seconds)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / seconds);
            if (from) from.volume = Mathf.Lerp(fromStart, 0f, k);
            if (to) to.volume = Mathf.Lerp(toStart, 1f, k);
            yield return null;
        }

        if (from) { from.volume = 0f; from.Stop(); }
        if (to) to.volume = 1f;
    }
}
