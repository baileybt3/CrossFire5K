using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public float maxHP = 100f;
    private float currentHP;


    [Header("Movement")]
    private Transform player;
    private NavMeshAgent agent;
    [SerializeField] private float moveSpeed = 3f;


    [Header("Wandering")]
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float wanderTimer = 3f;
    private float wanderCountdown;


    [Header("Attacking")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float attackRange = 5f;
    public float shootForce = 20f;
    public System.Action OnDeath;
    private float fireCooldown =0f;
  


    private void Start()
    {
        currentHP = maxHP;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.updateRotation = true;
        wanderCountdown = wanderTimer;

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

        if (player == null)
        {
            //Find player by tag
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (player != null)
        {

            float distance = Vector3.Distance(transform.position, player.position);

            if (distance > attackRange)
            {
                Wander();
            }
            else
            {
                AttackPlayer();
            }
        }

        if(fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
        }

    }
    
    void Wander()
    {
        wanderCountdown -= Time.deltaTime;

        if(wanderCountdown <= 0f)
        {
            //Pick random point on NavMesh
            Vector3 randomDir = Random.insideUnitSphere * wanderRadius;
            randomDir += transform.position;

            if (NavMesh.SamplePosition(randomDir, out NavMeshHit hit, wanderRadius, 1))
            {
                agent.isStopped = false;
                agent.SetDestination(hit.position);
            }

            wanderCountdown = wanderTimer;
        }
    }

    void AttackPlayer()
    {
        if (player == null) return;

        agent.isStopped = true;

        Vector3 lookDir = (player.position - transform.position);
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }

        if (Physics.Raycast(transform.position, lookDir.normalized, out RaycastHit hit, attackRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                TryShoot(lookDir.normalized);
            }
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