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

    public enum SfxChoice { World, Button}

    [SerializeField] private AudioMixer mixer;
    private string bgmParam = "BGM";
    private string sfxParam = "SFX";

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sampleSource;
    [SerializeField] private AudioClip deadSound;
    [SerializeField] private AudioClip sailSound;
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private AudioClip synthesisSound;
    [SerializeField] private AudioClip enforceSound;
    [SerializeField] private AudioClip clickSound;

    private const int POOLSIZE = 20;
    private const int ReservedPOOLSIZE = 8;

    private Queue<AudioSource> audios = new Queue<AudioSource>();
    private Queue<AudioSource> buttonAudios = new Queue<AudioSource>();
    private Dictionary<AudioSource, AudioClip> playing = new Dictionary<AudioSource, AudioClip>();
    private Dictionary<AudioSource, SfxChoice> sourceKinds = new Dictionary<AudioSource, SfxChoice>();

    private SaveData save;

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
        save = SaveManager.Load();

        float bgm = PlayerPrefs.GetFloat(bgmParam, 1f);
        float sfx = PlayerPrefs.GetFloat(sfxParam, 1f);

        ApplySavedVolumes();

        for(int i = 0; i < POOLSIZE; i++)
        {
            var s = Instantiate(sampleSource, transform);
            s.gameObject.SetActive(false);
            audios.Enqueue(s);
        }

        for(int i = 0; i < ReservedPOOLSIZE; i++)
        {
            var s = Instantiate(sampleSource, transform);
            s.gameObject.SetActive(false);
            buttonAudios.Enqueue(s);
        }

        bgmSource.priority = 0;
        
    }

    public void ApplySavedVolumes()
    {
        SetVolume(bgmParam, save.BgmVolume);
        SetVolume(sfxParam, save.SfxVolume);
        MuteAll(save.IsMute);
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
        PlaySfx(sailSound, pos, SfxChoice.Button);
    }

    public void PlaySpawn(Vector3 pos)
    {
        PlaySfx(spawnSound, pos, SfxChoice.Button);
    }

    public void PlaySynthesis(Vector3 pos)
    {
        PlaySfx(synthesisSound, pos, SfxChoice.Button);
    }

    public void Playenforce()
    {
        PlaySfx(enforceSound, Vector3.zero, SfxChoice.Button);
    }

    public void PlayClick()
    {
        PlaySfx(clickSound, Vector3.zero, SfxChoice.Button);
    }

    public void SetMasterVolume(float vol)
    {
        SetBgmVolume(vol);
        SetSfxVolume(vol);
    }
    public void SetBgmVolume(float vol)
    {
        save.BgmVolume = vol;
        SetVolume(bgmParam, vol);
        SaveManager.Save(save);
    }
    public void SetSfxVolume(float vol)
    {
        save.SfxVolume = vol;
        SetVolume(sfxParam, vol);
        SaveManager.Save(save);
    }

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
        save = SaveManager.Load();
        ApplySavedVolumes();
    }

    public void MuteAll(bool mute)
    {
        save.IsMute = mute;
        if (mute)
        {
            mixer.SetFloat("Master", -80f);
        }
        else
        {
            mixer.SetFloat("Master", 0f);
        }

        SaveManager.Save(save);
    }

    private AudioSource GetSource(SfxChoice kind)
    {
        Queue<AudioSource> q = (kind == SfxChoice.World) ? audios : buttonAudios;
        if(q.Count > 0)
        {
            var s = q.Dequeue();
            s.gameObject.SetActive(true);
            sourceKinds[s] = kind;
            return s;
        }

        return null;
    }

    private void ReturnSource(AudioSource s)
    {
        if(s == null)
        {
            return;
        }

        playing.Remove(s); //현재 재생중 목록에서 제거

        if(!sourceKinds.TryGetValue(s, out var kind))
        {
            kind = SfxChoice.World;
        }

        s.gameObject.SetActive(false);
        (kind == SfxChoice.World ? audios : buttonAudios).Enqueue(s); //비활성 목록에 추가
    }

    public void PlaySfx(AudioClip clip, Vector3 pos, SfxChoice kind = SfxChoice.World)
    {
        if(clip == null)
        {
            return;
        }

        int poolSize = (kind == SfxChoice.World) ? POOLSIZE : ReservedPOOLSIZE;
        Queue<AudioSource> q = (kind == SfxChoice.World) ? audios : buttonAudios;

        int activeCount = poolSize - q.Count;

        if(activeCount >= poolSize)
        {
            return;
        }

        //19개 재생중 -> 같은 클립 재생할거면 무시, 다른 클립 허용
        if(activeCount == poolSize - 1)
        {
            foreach(var kv in playing)
            {
                if(kv.Value == clip)
                {
                    return;
                }
            }
        }

        var s = GetSource(kind);
        if(s == null)
        {
            return;
        }

        s.transform.position = pos;
        playing[s] = clip;
        s.PlayOneShot(clip);
        StartCoroutine(ReturnAfterPlay(s, clip));
    }

    private IEnumerator ReturnAfterPlay(AudioSource s, AudioClip clip)
    {
        float est = clip.length / Mathf.Max(0.01f, Mathf.Abs(s.pitch));

        est += 0.02f; //보정용 버퍼

        yield return new WaitForSeconds(est);

        while(s != null && s.isPlaying)
        {
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.03f);

        ReturnSource(s);
    }
}
