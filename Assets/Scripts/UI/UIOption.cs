using UnityEngine;
using UnityEngine.UI;

public class UIOption : UIPanel
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle mute;
    [SerializeField] private Image bgmIcon;
    [SerializeField] private Image sfxIcon;
    [SerializeField] private Sprite muteIcon;
    [SerializeField] private Sprite soundIcon;
    private SaveData save;

    private void Start()
    {
        save = SaveManager.Load();
        bgmSlider.SetValueWithoutNotify(save.BgmVolume);
        sfxSlider.SetValueWithoutNotify(save.SfxVolume);

        AudioManager.Instance.SetBgmVolume(save.BgmVolume);
        AudioManager.Instance.SetSfxVolume(save.SfxVolume);

        mute.isOn = save.IsMute;

        bgmSlider.onValueChanged.AddListener(v => {
            save.BgmVolume = v;
            SaveManager.Save(save);
            AudioManager.Instance.SetBgmVolume(v);
        });

        sfxSlider.onValueChanged.AddListener(v => {
            save.SfxVolume = v;
            SaveManager.Save(save);
            AudioManager.Instance.SetSfxVolume(v);
        });

        
    }

    private void Update()
    {
        IconChange();
    }

    private void IconChange()
    {
        if (bgmSlider.value <= 0f)
        {
            bgmIcon.sprite = muteIcon;
        }
        else
        {
            bgmIcon.sprite = soundIcon;
        }

        if (sfxSlider.value <= 0f)
        {
            sfxIcon.sprite = muteIcon;
        }
        else
        {
            sfxIcon.sprite = soundIcon;
        }
    }
}
