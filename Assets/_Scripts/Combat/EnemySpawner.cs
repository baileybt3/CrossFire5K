using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [Header("Enemy")]
    [SerializeField] private GameObject enemyPrefab;

    public GameObject SpawnEnemy()
    {  
        GameObject enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
        return enemy;
    }
}
