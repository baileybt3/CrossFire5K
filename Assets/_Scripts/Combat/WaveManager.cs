using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; } //singleton

    [System.Serializable]
    public class Wave
    {
        [Min(0)]
        public int enemyCount = 5; // enemies in wave

        [Min(0f)]
        public float spawnDelayBetweenEnemies = 0.5f;
    }

    [Header("Spawners")]
    [SerializeField] private EnemySpawner[] spawners; // All spawn points

    [Header("Waves")]
    [SerializeField] private Wave[] waves;
    [SerializeField] private float delayBetweenWaves = 3f;

    private int currentWaveIndex = -1;
    private int enemiesAlive = 0;
    private bool isSpawningWave = false; // True while spawning, false while not spawning

    private void Awake()
    {
        // Initialize singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // Spawner not found helper
        if (spawners == null || spawners.Length == 0)
        {
            spawners = Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        }

        ValidateTicketsMatchGameManager();
        StartNextWave();
    }

    private void ValidateTicketsMatchGameManager()
    {
        if (GameManager.Instance == null || waves == null)
        {
            return;
        }

        // Count enemies in all waves
        int totalEnemies = 0;
        foreach (var wave in waves)
        {
            if (wave != null)
            {
                totalEnemies += Mathf.Max(0, wave.enemyCount);
            }
        }

    }

    private void StartNextWave()
    {
        currentWaveIndex++;

        // Get current waves stats
        Wave wave = waves[currentWaveIndex];
        Debug.Log($"[WaveManager] Starting wave {currentWaveIndex + 1} with {wave.enemyCount} enemies.");

        // Stop previous spawn coroutines and restart
        StopAllCoroutines();
        StartCoroutine(SpawnWaveCoroutine(wave));
    }

    private IEnumerator SpawnWaveCoroutine(Wave wave)
    {
        isSpawningWave = true;

        // Spawn enemyCount enemies with delay
        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnFromRandomSpawner();

            if (wave.spawnDelayBetweenEnemies > 0f)
            {
                yield return new WaitForSeconds(wave.spawnDelayBetweenEnemies);
            }
            else
            {
                yield return null;
            }
        }

        isSpawningWave = false;

        // Start next wave after enemies killed
        if (enemiesAlive == 0)
        {
            yield return new WaitForSeconds(delayBetweenWaves);
            if (GameManager.Instance != null && GameManager.Instance.CurrentMatchState == GameManager.MatchState.Playing)
            {
                StartNextWave();
            }
        }
    }

    private void SpawnFromRandomSpawner()
    {
        // Check spawners exist
        if (spawners == null || spawners.Length == 0)
        {
            Debug.LogWarning("No spawners assigned.");
            return;
        }

        // Pick random spawner and spawn enemy
        int index = Random.Range(0, spawners.Length);
        EnemySpawner chosenSpawner = spawners[index];

        GameObject enemyGO = chosenSpawner.SpawnEnemy();
        if (enemyGO != null)
        {
            enemiesAlive++;
        }
    }

    public void OnEnemyDied()
    {
        // Clamp to 0
        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);

        if (isSpawningWave)
        {
            return;
        }

        if (waves == null || currentWaveIndex >= waves.Length)
        {
            return;
        }

        // Start last wave enemy dies and match is still active start next wave
        if (enemiesAlive == 0 && GameManager.Instance != null && GameManager.Instance.CurrentMatchState == GameManager.MatchState.Playing)
        {
            StartCoroutine(StartNextWaveDelayed());
        }
    }

    private IEnumerator StartNextWaveDelayed()
    {
        yield return new WaitForSeconds(delayBetweenWaves);
        StartNextWave();
    }
}
