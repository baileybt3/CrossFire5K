using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 3f;
    private float timer;
    public int maxEnemies = 5;
   
    
    void Start()
    {
        timer = spawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;

        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (timer <= 0 && currentEnemies < maxEnemies)
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            timer = spawnInterval;
        }
    }

    
}
