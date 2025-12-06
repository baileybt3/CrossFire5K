 using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    public bool IsPaused { get; private set; }

    private void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false); // Hide menu

            Time.timeScale = 1f; 
            IsPaused = false;
        }

    }

    // Input = (Esc) button
    public void TogglePause()
    {
        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (IsPaused) return;

        IsPaused = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        // Stop gameplay
        Time.timeScale = 0f;

        // Free cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        if (!IsPaused)
        {
            return;
        }

        IsPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);

            Time.timeScale = 1f;

        }
    }

    // Exit to menu button
    public void ExitToMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("01_MainMenu");
    }

    // Settings button
    public void OpenSettings()
    {
        Debug.Log("Settings menu in development");
    }

    // Unfreeze game if destroyed
    private void OnDestroy()
    {
       if(Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
