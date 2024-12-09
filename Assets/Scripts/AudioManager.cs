using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // **싱글톤 인스턴스**
    private AudioSource audioSource; // **AudioSource 컴포넌트**
    private Coroutine fadeCoroutine; // **페이드 인/아웃 코루틴 제어**

    [Header("Scene-Specific BGM Clips")]
    public AudioClip StartSceneBGM;
    public AudioClip mainSceneBGM;
    public AudioClip scene1BGM;
    public AudioClip scene2BGM;
    public AudioClip scene3_1BGM;
    public AudioClip scene3BGM;
    public AudioClip scene4BGM;
    public AudioClip scene5BGM;

    private int mirrorCount = 0; // **mirrorCount 변수**
    private int previousMirrorCount = 0; // **이전 mirrorCount 값**

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f;
        audioSource.mute = false; // 음소거 해제

        // **현재 씬의 BGM을 재생**
        Debug.Log($"현재 씬: {SceneManager.GetActiveScene().name}");
        PlayBGMForScene(SceneManager.GetActiveScene().name); // **현재 씬에 맞는 BGM 재생**
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene Loaded: {scene.name}");
        PlayBGMForScene(scene.name);
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Scene2")
        {
            S2SceneManager s2 = FindObjectOfType<S2SceneManager>();
            if (s2 != null)
            {
                mirrorCount = s2.mirrorCount;

                if (mirrorCount > previousMirrorCount)
                {
                    Debug.Log($"mirrorCount가 {previousMirrorCount}에서 {mirrorCount}로 증가했습니다.");
                    IncreaseBGMVolume(0.1f); // **BGM 소리 1.2배 증가**
                    previousMirrorCount = mirrorCount; // **이전 값 갱신**
                }
            }
        }
    }

    private void PlayBGMForScene(string sceneName)
    {
        Debug.Log($"BGM 플레이 시작: {sceneName}"); // **디버그 로그 추가**
        AudioClip clipToPlay = null;

        switch (sceneName)
        {
            case "StartScene": // **LobbyScene 추가**
                clipToPlay = StartSceneBGM; // **LobbyScene의 BGM으로 StartSceneBGM 재생**
                break;
            case "LobbyScene": // **LobbyScene 추가**
                clipToPlay = StartSceneBGM; // **LobbyScene의 BGM으로 StartSceneBGM 재생**
                break;
            case "MenuScene":
                clipToPlay = StartSceneBGM;
                break;
            case "MainScene":
                clipToPlay = mainSceneBGM;
                break;
            case "Scene1":
                clipToPlay = scene1BGM;
                break;
            case "Scene2":
                clipToPlay = scene2BGM;
                break;
            case "Scene3-1":
                clipToPlay = scene3_1BGM;
                break;
            case "Scene3":
                clipToPlay = scene3BGM;
                break;
            case "Scene4":
                clipToPlay = scene4BGM;
                break;
            case "Scene5":
                clipToPlay = scene5BGM;
                break;
            default:
                Debug.LogWarning($"BGM not assigned for scene: {sceneName}");
                return;
        }

        if (clipToPlay != null)
        {
            PlayBGMWithFade(clipToPlay, 1f);
        }
    }


    public void PlayBGM(AudioClip clip)
    {
        if (audioSource.clip == clip && audioSource.isPlaying)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        audioSource.clip = clip;
        audioSource.Play();
    }

    public void PlayBGMWithFade(AudioClip clip, float fadeDuration = 1f)
    {
        if (audioSource.clip == clip && audioSource.isPlaying)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeInBGM(clip, fadeDuration));
    }

    private IEnumerator FadeInBGM(AudioClip newClip, float duration)
    {
        if (audioSource.isPlaying)
        {
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(0.5f, 0f, t / duration);
                yield return null;
            }
            audioSource.Stop();
        }

        audioSource.clip = newClip;
        audioSource.Play();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, 0.5f, t / duration);
            yield return null;
        }
    }

    public void PauseBGM()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void ResumeBGM()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopBGM()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp(volume, 0f, 1f);
    }

    public void IncreaseBGMVolume(float add)
    {
        float newVolume = audioSource.volume + add;
        audioSource.volume = Mathf.Clamp(newVolume, 0f, 1f);
        Debug.Log($"New BGM Volume: {audioSource.volume}");
    }
}
