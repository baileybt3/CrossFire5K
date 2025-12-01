using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [Header("Enemy")]
    [SerializeField] private GameObject enemyPrefab;

    public GameObject SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning($"EnemySpawner on {name} has no enemyPrefab assigned.", this);
            return null;
        }
    

        GameObject enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
        return enemy;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.25f);
    }


}
