using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDTicketsUI : MonoBehaviour
{
    [Header("Player Tickets")]
    [SerializeField] private Image playerTicketsFillImage;
    [SerializeField] private TextMeshProUGUI playerTicketsText;

    [Header("Enemy Tickets")]
    [SerializeField] private Image enemyTicketsFillImage;
    [SerializeField] private TextMeshProUGUI enemyTicketsText;

    private int maxPlayerTickets;
    private int maxEnemyTickets;

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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnTicketsChanged -= HandleTicketsChanged;
        }

        isSubscribed = false;
    }

    private void TrySubscribe()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        // Subscribe once
        GameManager.Instance.OnTicketsChanged += HandleTicketsChanged;

        maxPlayerTickets = GameManager.Instance.StartingPlayerTickets;
        maxEnemyTickets = GameManager.Instance.StartingEnemyTickets;

        HandleTicketsChanged(GameManager.Instance.PlayerTickets, GameManager.Instance.EnemyTickets);

        isSubscribed = true;
    }

    private void HandleTicketsChanged(int playerTickets, int enemyTickets)
    {
        if (maxPlayerTickets <= 0) maxPlayerTickets = Mathf.Max(1, playerTickets);
        if (maxEnemyTickets <= 0) maxEnemyTickets = Mathf.Max(1, enemyTickets);

        float playerRatio = (float)playerTickets / maxPlayerTickets;
        float enemyRatio = (float)enemyTickets / maxEnemyTickets;

        // Clamp just to be safe
        playerRatio = Mathf.Clamp01(playerRatio);
        enemyRatio = Mathf.Clamp01(enemyRatio);

        if (playerTicketsFillImage != null)
            playerTicketsFillImage.fillAmount = playerRatio;

        if (enemyTicketsFillImage != null)
            enemyTicketsFillImage.fillAmount = enemyRatio;

        if (playerTicketsText != null)
            playerTicketsText.text = playerTickets.ToString();

        if (enemyTicketsText != null)
            enemyTicketsText.text = enemyTickets.ToString();
    }
}
