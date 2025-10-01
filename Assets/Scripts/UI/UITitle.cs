using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UITitle : MonoBehaviour
{
    public GameObject Option;

    [SerializeField] private AudioMixer mixer;

    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private Button tutorialBtn;

    private const string KEY_BGM = "BGM";
    private const string KEY_SFX = "SFX";

    private SaveData save;

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;

        Option.SetActive(false);
    }

    private void Start()
    {
        save = SaveManager.Load();
        bgmSlider.SetValueWithoutNotify(save.BgmVolume);
        sfxSlider.SetValueWithoutNotify(save.SfxVolume);

        SetVolume(KEY_BGM, save.BgmVolume);
        SetVolume(KEY_SFX, save.SfxVolume);

        MuteAll(save.IsMute);

        bgmSlider.onValueChanged.AddListener(v => {
            save.BgmVolume = v;
            SaveManager.Save(save);
            SetVolume(KEY_BGM, v);
        });

        sfxSlider.onValueChanged.AddListener(v => {
            save.SfxVolume = v;
            SaveManager.Save(save);
            SetVolume(KEY_SFX, v);
        });

        if(!save.Tutorial)
        {
            tutorialBtn.gameObject.SetActive(false);
        }
        else
        {
            tutorialBtn.gameObject.SetActive(true);
        }
    }

    private void SetVolume(string param, float vol)
    {
        float value = Mathf.Clamp(vol, 0.001f, 1f);
        mixer.SetFloat(param, Mathf.Log10(value) * 20);
    }

    public void StartGame()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        if (save.Tutorial)
        {
            SceneManager.LoadScene(2);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ActiveOption()
    {
        Option.SetActive(true);
    }

    public void UnActiveOption()
    {
        Option.SetActive(false);
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

    public void LoadTutorial()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        SceneManager.LoadScene(1);
    }
}
