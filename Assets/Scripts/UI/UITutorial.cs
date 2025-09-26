using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UITutorial : UIPanel
{
    [SerializeField] private List<Sprite> sprites = new List<Sprite>();
    [SerializeField] private List<string> texts = new List<string>();
    [SerializeField] private Image mainImage;
    [SerializeField] private GameObject TextPanel;
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject TutorialPanel;

    private int index;

    private void OnEnable()
    {
        index = 0;
        mainImage.sprite = sprites[0];
        TextPanel.SetActive(false);
    }

    public void TutorialSkip()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        SceneManager.LoadScene(2);
    }

    public void TutorialUnSkip()
    {
        TutorialPanel.SetActive(false);
        if (index < sprites.Count - 1)
        {
            index++;
            UpdateView();
        }
    }

    private void UpdateView()
    {
        if(sprites == null | sprites.Count == 0)
        {
            return;
        }

        if(index < 0)
        {
            index = 0;
        }
        if(index >= sprites.Count)
        {
            index = sprites.Count - 1;
        }

        mainImage.sprite = sprites[index];

        if(tutorialText != null)
        {
            string desc = (texts != null && index < texts.Count) ? texts[index] : string.Empty;
            tutorialText.text = desc;
        }
    }

    public void OnClickNext()
    {
        if(index < sprites.Count - 1)
        {
            index++;
            UpdateView();
        }
    }

    public void OnClickPrev()
    {
        if(index > 0)
        {
            index--;
            UpdateView();
        }
    }
}
