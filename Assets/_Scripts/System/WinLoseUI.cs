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

    private void HandleMatchEnded(bool playerWon)
    {
        // hide both panels initially
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        if (playerWon)
        {
            if (winPanel != null)
            {
                winPanel.SetActive(true);
            }
        }
        else
        {
            if (losePanel != null)
            {
                losePanel.SetActive(true);
            }
        }
    }

    public void OnPlayAgainClicked()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    public void OnMainMenuClicked()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("01_MainMenu");
    }
}