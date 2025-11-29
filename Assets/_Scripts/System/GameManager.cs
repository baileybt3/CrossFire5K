using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private PauseMenuUI pauseMenuUI;
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
            // Destroy old player after death
            if (currentPlayer != null)
            {
                Destroy(currentPlayer);
            }
            
            GameObject player = Instantiate(playerPrefab, respawnPoint.position, respawnPoint.rotation);
            currentPlayer = player;

            // Inject PauseMenu into PlayerController
            PlayerController controller = player.GetComponent<PlayerController>();
            if(controller != null && pauseMenuUI != null)
            {
                controller.Initialize(pauseMenuUI);
            }

            // Hook up camera
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