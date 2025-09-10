using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // HP
    [SerializeField] public float maxHP = 100f;
    private float currentHP;

    // Movement
    private Transform player;
    [SerializeField] private float moveSpeed = 3f;

    // Attacking
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float attackRange = 15f;
    public float shootForce = 20f;

    public System.Action OnDeath;

    private float fireCooldown =0f;
  
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
        if (currentHP <= 0) return;

        if (player != null)
        {

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

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, attackRange))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    TryShoot(dir);
                }
            }
        }

        if(fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
        }

    }
    

    void TryShoot(Vector3 direction)
    {
        if(fireCooldown <= 0f)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            rb.linearVelocity = direction.normalized * shootForce;
            bullet.transform.forward = direction;

            fireCooldown = 1f / fireRate;
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * attackRange);
    }
}