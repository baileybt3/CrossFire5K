using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform respawnPoint;

    private GameObject currentPlayer;

    private void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        if(playerPrefab != null && respawnPoint != null)
        {
            GameObject player = Instantiate(playerPrefab, respawnPoint.position, respawnPoint.rotation);

            CameraFollow camFollow = FindAnyObjectByType<CameraFollow>();
            if(camFollow != null)
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