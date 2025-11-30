using UnityEngine;
using System.Collections;
using System;

public class GameManager : MonoBehaviour
{
    // Singleton so other scripts can call GameManager.Instance
    public static GameManager Instance { get; private set; }
    
    public enum MatchState { Playing, GameOver}

    [Header("Player & UI")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private PauseMenuUI pauseMenuUI;
    public Transform respawnPoint;

    public MatchState CurrentMatchState { get; private set; } = MatchState.Playing;

    [Header("Tickets")]
    [SerializeField] private int startingEnemyTickets = 15;
    [SerializeField] private int startingFriendlyTickets = 15;
    private int currentPlayerTickets;
    private int currentEnemyTickets;

    // Read-only hud properties
    public int PlayerTickets => currentPlayerTickets;
    public int EnemyTickets => currentEnemyTickets;

    //Event HUD / UI
    public event Action<int, int> OnTicketsChanged;
    public event Action<bool> OnMatchEnded;

    
    private GameObject currentPlayer;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        StartMatch();
    }

    public void StartMatch()
    {
        CurrentMatchState = MatchState.Playing;

        currentPlayerTickets = startingFriendlyTickets;
        currentEnemyTickets = startingEnemyTickets;
        RaiseTicketsChanged();

        SpawnPlayer();
    }

    public void OnPlayerDeath()
    {
        if (CurrentMatchState != MatchState.Playing)
            return;

        currentPlayerTickets = Mathf.Max(0, currentPlayerTickets - 1);
        RaiseTicketsChanged();

        if(currentPlayerTickets > 0)
        {
            RespawnPlayer(2f);
        }
        else
        {
            EndMatch(false);
        }
    }

    public void OnEnemyDeath()
    {
        if (CurrentMatchState != MatchState.Playing)
            return;

        currentEnemyTickets = Mathf.Max(0, currentEnemyTickets - 1);
        RaiseTicketsChanged();

        if(currentEnemyTickets <= 0)
        {
            EndMatch(true);
        }
    }

    private void EndMatch(bool playerWon)
    {
        if(CurrentMatchState == MatchState.GameOver)
        {
            return;
        }

        CurrentMatchState = MatchState.GameOver;

        OnMatchEnded?.Invoke(playerWon);
    }

    private void RaiseTicketsChanged()
    {
        OnTicketsChanged?.Invoke(currentPlayerTickets, currentEnemyTickets);
    }

    public void SpawnPlayer()
    {
        if (playerPrefab != null && respawnPoint != null)
        {
            // Destroy old player after death
            if (currentPlayer != null)
            {
                Destroy(currentPlayer);
            }

            if (CurrentMatchState != MatchState.Playing)
                return;

            GameObject player = Instantiate(playerPrefab, respawnPoint.position, respawnPoint.rotation);
            currentPlayer = player;

            // Inject PauseMenu into PlayerController
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null && pauseMenuUI != null)
            {
                controller.Initialize(pauseMenuUI);
            }

            // Hook up camera
            CameraFollow camFollow = FindAnyObjectByType<CameraFollow>();
            if (camFollow != null)
            {
                camFollow.SetTarget(player.transform);
            }
        }
    }

    public void RespawnPlayer(float delay)
    {
        StartCoroutine(RespawnCoroutine(delay));
    }

    private IEnumerator RespawnCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if(respawnPoint != null && playerPrefab != null)
        {
            SpawnPlayer();
        }
    }
}