using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [System.Serializable]
    public class Wave
    {
        [Min(0)]
        public int enemyCount = 5;

        [Min(0f)]
        public float spawnDelayBetweenEnemies = 0.5f;
    }

    [Header("Spawners")]
    [SerializeField] private EnemySpawner[] spawners;

    [Header("Waves")]
    [SerializeField] private Wave[] waves;
    [SerializeField] private float delayBetweenWaves = 3f;

    private int currentWaveIndex = -1;
    private int enemiesAlive = 0;
    private bool isSpawningWave = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        // Auto-find spawners if not set in Inspector
        if (spawners == null || spawners.Length == 0)
        {
            spawners = Object.FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None);
        }

        ValidateTicketsMatchGameManager();
        StartNextWave();
    }

    private void ValidateTicketsMatchGameManager()
    {
        if (GameManager.Instance == null || waves == null) return;

        int totalEnemies = 0;
        foreach (var wave in waves)
        {
            if (wave != null)
                totalEnemies += Mathf.Max(0, wave.enemyCount);
        }

        if (totalEnemies != GameManager.Instance.StartingEnemyTickets)
        {
            Debug.LogWarning(
                $"[WaveManager] Total enemies across waves ({totalEnemies}) " +
                $"does NOT match GameManager StartingEnemyTickets ({GameManager.Instance.StartingEnemyTickets}). " +
                $"For tickets to hit 0 on the last kill, set them equal.",
                this);
        }
    }

    private void StartNextWave()
    {
        currentWaveIndex++;

        if (waves == null || waves.Length == 0)
        {
            Debug.LogWarning("[WaveManager] No waves configured.", this);
            return;
        }

        if (currentWaveIndex >= waves.Length)
        {
            // All waves spawned; GameManager will end match when tickets hit 0.
            Debug.Log("[WaveManager] All waves completed.");
            return;
        }

        Wave wave = waves[currentWaveIndex];
        Debug.Log($"[WaveManager] Starting wave {currentWaveIndex + 1} with {wave.enemyCount} enemies.");

        StopAllCoroutines();
        StartCoroutine(SpawnWaveCoroutine(wave));
    }

    private IEnumerator SpawnWaveCoroutine(Wave wave)
    {
        isSpawningWave = true;

        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnFromRandomSpawner();

            if (wave.spawnDelayBetweenEnemies > 0f)
                yield return new WaitForSeconds(wave.spawnDelayBetweenEnemies);
            else
                yield return null;
        }

        isSpawningWave = false;

        // If they somehow killed everything during the spawn, check immediately.
        if (enemiesAlive == 0)
        {
            yield return new WaitForSeconds(delayBetweenWaves);
            if (GameManager.Instance != null &&
                GameManager.Instance.CurrentMatchState == GameManager.MatchState.Playing)
            {
                StartNextWave();
            }
        }
    }

    private void SpawnFromRandomSpawner()
    {
        if (spawners == null || spawners.Length == 0)
        {
            Debug.LogWarning("[WaveManager] No spawners assigned.", this);
            return;
        }

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
        enemiesAlive = Mathf.Max(0, enemiesAlive - 1);

        if (isSpawningWave) return;
        if (waves == null || currentWaveIndex >= waves.Length) return;

        if (enemiesAlive == 0 &&
            GameManager.Instance != null &&
            GameManager.Instance.CurrentMatchState == GameManager.MatchState.Playing)
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
