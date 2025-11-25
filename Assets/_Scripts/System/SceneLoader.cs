using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour    
{
    public static SceneLoader Instance { get; private set; }

    public string previousSceneName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "00_Bootstrap")
        {
            SceneManager.LoadScene("01_MainMenu");
        }
    }

    private void LoadSceneAndRemember(string sceneName)
    {
        previousSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    public void OpenArmory()
    {
        LoadSceneAndRemember("02_Armory");
    }

    public void LoadMapSelect()
    {
        LoadSceneAndRemember("03_MapSelect");
    }

    public void LoadArena(string sceneName)
    {
        LoadSceneAndRemember(sceneName);
    }

    public void GoBack()
    {
        if (!string.IsNullOrEmpty(previousSceneName))
        {
            SceneManager.LoadScene(previousSceneName);
        }
        else
        {
            SceneManager.LoadScene("01_MainMenu");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
