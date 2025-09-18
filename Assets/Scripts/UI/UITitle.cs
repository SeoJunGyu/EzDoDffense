using UnityEngine;
using UnityEngine.SceneManagement;

public class UITitle : MonoBehaviour
{
    public GameObject Option;

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;

        Option.SetActive(false);
    }

    public void StartGame()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        SceneManager.LoadScene(1);
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
