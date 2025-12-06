using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour    
{
    public static SceneLoader Instance { get; private set; }

    public string previousSceneName; // Last scene for back button

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

    // Open Armory button
    public void OpenArmory()
    {
        LoadSceneAndRemember("02_Armory");
    }

    // Button open map select scene
    public void LoadMapSelect()
    {
        LoadSceneAndRemember("03_MapSelect");
    }

    // MapSelectUI map choice
    public void LoadArena(string sceneName)
    {
        LoadSceneAndRemember(sceneName);
    }

    // Go back button call
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
