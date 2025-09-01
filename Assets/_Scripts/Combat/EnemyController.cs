using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    private Transform player;

    private void Start()
    {
        //Find player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Direction toward player
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0f;

        // Move toward player
        transform.position += dir * moveSpeed * Time.deltaTime;

        // Face player
        if (dir.sqrMagnitude > 0f)
        {
            transform.forward = dir;
        }
    }
}