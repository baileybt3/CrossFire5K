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
        // When Bootstrap runs, immediately load Main Menu
        if (SceneManager.GetActiveScene().name == "00_Bootstrap")
        {
            SceneManager.LoadScene("01_MainMenu");
        }
    }

    // Public method for UI Buttons
    public void LoadArena()
    {
        SceneManager.LoadScene("02_Arena");
    }
}
