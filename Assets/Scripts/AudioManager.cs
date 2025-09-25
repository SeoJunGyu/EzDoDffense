using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioMixer mixer;
    private string bgmParam = "BGM";
    private string sfxParam = "SFX";

    [SerializeField] private AudioSource AreaSource;
    [SerializeField] private AudioClip deadSound;
    [SerializeField] private AudioClip sailSound;
    [SerializeField] private AudioClip spawnSound;
    [SerializeField] private AudioClip synthesisSound;

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

    private void Start()
    {
        float bgm = PlayerPrefs.GetFloat("BGM", 1f);
        float sfx = PlayerPrefs.GetFloat("SFX", 1f);

        SetVolume(bgmParam, bgm);
        SetVolume(sfxParam, sfx);
    }

    public void ApplySavedVolumes()
    {
        var save = SaveManager.Load();
        SetVolume("BGM", save.BgmVolume);
        SetVolume("SFX", save.SfxVolume);
    }

    public void PlayDead()
    {
        AreaSource.PlayOneShot(deadSound);
    }

    public void PlaySkill(AudioClip clip)
    {
        if(clip != null)
        {
            AreaSource.PlayOneShot(clip);
        }
    }

    public void PlaySail()
    {
        AreaSource.PlayOneShot(sailSound);
    }

    public void PlaySpawn()
    {
        AreaSource.PlayOneShot(spawnSound);
    }

    public void PlaySynthesis()
    {
        AreaSource.PlayOneShot(synthesisSound);
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
        float value = Mathf.Clamp(vol, 0.001f, 1f);
        mixer.SetFloat(param, Mathf.Log10(value) * 20);
    }
}
