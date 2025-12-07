using UnityEngine;
using UnityEngine.UI;

public class MainMenuInstructions : MonoBehaviour
{

    [Header("references")]
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private Button okButton;

    [SerializeField] private bool showOnce = true;

    private const string HasSeenKey = "HasSeenInstructions";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // button wire up
        if (okButton != null)
        {
            okButton.onClick.RemoveListener(OnOkClicked); // safety
            okButton.onClick.AddListener(OnOkClicked);
        }

        bool hasSeen = PlayerPrefs.GetInt(HasSeenKey, 0) == 1;

        if(instructionsPanel != null)
        {
            instructionsPanel.SetActive(!showOnce || !hasSeen);
        }
    }

    // Okay button
    public void OnOkClicked()
    {
        if(instructionsPanel != null)
        {
            instructionsPanel.SetActive(false);
        }

        if (showOnce)
        {
            PlayerPrefs.SetInt(HasSeenKey, 1);
            PlayerPrefs.Save();
        }

        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUIClick();
        }
    }

}
