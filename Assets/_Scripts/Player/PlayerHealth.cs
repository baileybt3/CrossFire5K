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

        if (reloadPromptText != null)
        {
            reloadPromptText.gameObject.SetActive(false);
        }

        Instance = this;
    }

    private void Update()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>();
        }

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

        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = ratio;
        }

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

        if (reloadPromptText != null)
        {
            bool shouldShow = currentAmmo <= 0 && reserveAmmo > 0;
            reloadPromptText.gameObject.SetActive(shouldShow);
        }
    }
    
}
