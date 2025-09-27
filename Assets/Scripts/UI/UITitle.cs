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
    }

    private void SetVolume(string param, float vol)
    {
        float value = Mathf.Clamp(vol, 0.001f, 1f);
        mixer.SetFloat(param, Mathf.Log10(value) * 20);
    }

    public void StartGame()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        switch (save.Tutorial)
        {
            case 0:
                SceneManager.LoadScene(1);
                break;
            case 1:
                SceneManager.LoadScene(2);
                break;
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
}
