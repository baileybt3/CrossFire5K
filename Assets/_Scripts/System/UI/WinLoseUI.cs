using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinLoseUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    private bool isSubscribed = false;

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void Update()
    {
        if (!isSubscribed)
        {
            TrySubscribe();
        }
    }

    private void OnDisable()
    {
        if (isSubscribed && GameManager.Instance != null)
        {
            GameManager.Instance.OnMatchEnded -= HandleMatchEnded;
        }

        isSubscribed = false;
    }

    private void TrySubscribe()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.OnMatchEnded += HandleMatchEnded;
        isSubscribed = true;
    }

    // Show win or lose panel
    private void HandleMatchEnded(bool playerWon)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // hide both panels initially
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        
        if (playerWon)
        {          
            winPanel.SetActive(true); 
        }
        else
        {
            losePanel.SetActive(true);
        }
    }

    // Play again button
    public void OnPlayAgainClicked()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    // Main menu button
    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("01_MainMenu");
    }

    // Unfreeze gameplay when destroyed
    private void OnDestroy()
    {
        if(Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
    }
}