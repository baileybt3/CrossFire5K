using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image healthFillImage;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI reloadPromptText;

    public static PlayerHealth Instance { get; private set; }

    private PlayerController player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Hide reload prompt
        if (reloadPromptText != null)
        {
            reloadPromptText.gameObject.SetActive(false);
        }

        Instance = this;
    }

    private void Update()
    {
        // Get player reference
        player = FindAnyObjectByType<PlayerController>();
   
        // Update player healthUI
        if (player != null)
        {
            UpdateHealthUI();
        }
    }

    private void UpdateHealthUI()
    {
        float current = player.CurrentHP;
        float max = Mathf.Max(1f, player.MaxHP);

        float ratio = current / max;

        // Update heatlh bar amount
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = ratio;
        }

        // Update health text
        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        }
    }

    public void UpdateAmmo(int currentAmmo, int reserveAmmo)
    {

        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo} / {reserveAmmo}";
        }  

        // Ammo prompts
        if (reloadPromptText != null)
        {
            if(currentAmmo <= 0)
            {
                if(reserveAmmo > 0)
                {
                    reloadPromptText.text = "Press R to Reload";
                    reloadPromptText.gameObject.SetActive(true);
                }
                else
                {
                    reloadPromptText.text = "Out of Ammo";
                    reloadPromptText.gameObject.SetActive(true);
                }
            }
            else
            {
                reloadPromptText.gameObject.SetActive(false);
            }
        }
    }

    public void AddHealth(float amount)
    {
        if(player == null)
        {
            player = FindAnyObjectByType<PlayerController>();
        }

        if (player == null)
        {
            return;
        }

        player.AddHealth(amount);

        UpdateHealthUI();
    }
    
}
