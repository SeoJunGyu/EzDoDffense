using UnityEngine;
using UnityEngine.SceneManagement;

public class UITitle : MonoBehaviour
{
    public GameObject Option;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        Option.SetActive(false);
    }

    public void StartGame()
    {
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

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 0)
        {
            Screen.orientation = ScreenOrientation.Portrait;
        }
        else if(scene.buildIndex == 1)
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
