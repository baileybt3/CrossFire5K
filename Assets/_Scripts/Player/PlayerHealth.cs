using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image healthFillImage;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI ammoText;

    public static PlayerHealth Instance { get; private set; }

    private PlayerController player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
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

    public void UpdateAmmo(int currentInMag, int reserve)
    {

        if (ammoText != null)
        {
            ammoText.text = $"{currentInMag} / {reserve}";
        }  
    }
    
}
