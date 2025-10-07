using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "00_Bootstrap")
        {
            SceneManager.LoadScene("01_MainMenu");
        }
    }

    public void OpenArmory()
    {
        SceneManager.LoadScene("02_Armory");
    }

    public void LoadArena()
    {
        SceneManager.LoadScene("03_Arena");
    }

    public void BackButton()
    {
        SceneManager.LoadScene("01_MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
