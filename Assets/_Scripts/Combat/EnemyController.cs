using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // HP
    [SerializeField] public float maxHP = 100f;
    private float currentHP;

    // Movement
    private Transform player;
    [SerializeField] private float moveSpeed = 3f;



    private void Start()
    {
        currentHP = maxHP;

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
    

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. HP left: {currentHP}");

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");
        Destroy(gameObject);
    }
}