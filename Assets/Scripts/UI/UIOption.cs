using UnityEngine;
using UnityEngine.UI;

public class UIOption : UIPanel
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    private SaveData save;

    private void Start()
    {
        save = SaveManager.Load();
        bgmSlider.SetValueWithoutNotify(save.BgmVolume);
        sfxSlider.SetValueWithoutNotify(save.SfxVolume);

        AudioManager.Instance.SetBgmVolume(save.BgmVolume);
        AudioManager.Instance.SetSfxVolume(save.SfxVolume);

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
}
