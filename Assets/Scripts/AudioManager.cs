using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer mixer;
    private string bgmParam = "BGM";
    private string sfxParam = "SFX";

    [SerializeField] private AudioSource sampleSource;
    [SerializeField] private AudioClip deadSound;
    [SerializeField] private AudioClip sailSound;
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private AudioClip synthesisSound;

    private Queue<AudioSource> audios = new Queue<AudioSource>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        float bgm = PlayerPrefs.GetFloat("BGM", 1f);
        float sfx = PlayerPrefs.GetFloat("SFX", 1f);

        SetVolume(bgmParam, bgm);
        SetVolume(sfxParam, sfx);

        ApplySavedVolumes();

        for(int i = 0; i < 10; i++)
        {
            var s = Instantiate(sampleSource, transform);
            s.gameObject.SetActive(false);
            audios.Enqueue(s);
        }
    }

    public void ApplySavedVolumes()
    {
        var save = SaveManager.Load();
        SetVolume("BGM", save.BgmVolume);
        SetVolume("SFX", save.SfxVolume);
    }

    public void PlayDead(Vector3 pos)
    {
        PlaySfx(deadSound, pos);
    }

    public void PlaySkill(AudioClip clip, Vector3 pos)
    {
        if(clip != null)
        {
            PlaySfx(clip, pos);
        }
    }

    public void PlaySail(Vector3 pos)
    {
        PlaySfx(sailSound, pos);
    }

    public void PlaySpawn(Vector3 pos)
    {
        PlaySfx(spawnSound, pos);
    }

    public void PlaySynthesis(Vector3 pos)
    {
        PlaySfx(synthesisSound, pos);
    }

    public void SetMasterVolume(float vol)
    {
        SetBgmVolume(vol);
        SetSfxVolume(vol);
    }
    public void SetBgmVolume(float vol) => SetVolume("BGM", vol);
    public void SetSfxVolume(float vol) => SetVolume("SFX", vol);

    private void SetVolume(string param, float vol)
    {
        // 0~1 슬라이더
        float t = Mathf.Clamp01(vol);

        // 아주 낮은 구간은 '묵음 스냅' (원하면 0.02~0.05 사이로 조정)
        if (t < 0.02f)
        {
            mixer.SetFloat(param, -80f);
            return;
        }

        // 저역 민감도 눌러주기 (k > 1 이면 처음 구간이 완만해짐)
        const float k = 2.2f;    // 1.8~3.0 사이에서 취향대로 조절
        t = Mathf.Pow(t, k);

        // 로그 스케일 유지 + 바닥값으로 급상승 방지
        const float vMin = 0.0001f; // 실제 무음에 가까운 바닥
        float v = Mathf.Lerp(vMin, 1f, t);

        float dB = 20f * Mathf.Log10(v); // 0dB ~ -80dB 사이 자연스러운 체감
        mixer.SetFloat(param, dB);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mod)
    {
        ApplySavedVolumes();
    }

    public void MuteAll(bool mute)
    {
        if (mute)
        {
            mixer.SetFloat("Master", -80f);
        }
        else
        {
            mixer.SetFloat("Master", 0f);
        }
    }

    private AudioSource GetSource()
    {
        if(audios.Count > 0)
        {
            var s = audios.Dequeue();
            s.gameObject.SetActive(true);
            return s;
        }

        return Instantiate(sampleSource, transform);
    }

    private void ReturnSource(AudioSource s)
    {
        s.gameObject.SetActive(false);
        audios.Enqueue(s);
    }

    public void PlaySfx(AudioClip clip, Vector3 pos)
    {
        var s = GetSource();
        s.transform.position = pos;
        s.PlayOneShot(clip);
        StartCoroutine(ReturnAfterPlay(s, clip.length));
    }

    private IEnumerator ReturnAfterPlay(AudioSource s, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnSource(s);
    }
}
