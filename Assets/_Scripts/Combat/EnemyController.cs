using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] public float maxHP = 100f;
    private float currentHP;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    private Transform player;
    private NavMeshAgent agent;

    [Header("AI Settings")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float attackRange = 12f;

    [Header("Wander Points")]
    public Transform[] wanderPoints;
    private Transform currentWanderTarget;
    private float wanderCooldown = 0f;
    [SerializeField] private float wanderDelay = 3f;

    [Header("Attacking")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float shootForce = 20f;
    private float fireCooldown = 0f;

    private enum State { Wandering, Chasing, Attacking }
    private State currentState = State.Wandering;



    private void Start()
    {
        currentHP = maxHP;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        //Find player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        PickNewWanderPoint();
    }

    void Update()
    {
        if (currentHP <= 0) return;

        if (player == null)
        {
            //Find player by tag
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;

        }

        if (player != null)
        {

            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= attackRange && HasLineOfSight())
            {
                currentState = State.Attacking;
            }
            else if (distance <= detectionRange)
            {
                currentState = State.Chasing;
            }
            else
                currentState = State.Wandering;
        }

        switch (currentState)
        {
            case State.Wandering: Wander(); break;
            case State.Chasing: Chase(); break;
            case State.Attacking: Attack(); break;
        }

        if (fireCooldown > 0f)
        {
            fireCooldown -= Time.deltaTime;
        }

    }

    void Wander()
    {
        wanderCooldown -= Time.deltaTime;

        if (currentWanderTarget == null || agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            
            if (wanderCooldown <= 0f)
            {
                PickNewWanderPoint();
                wanderCooldown = wanderDelay;
            }

        }
    }

    void PickNewWanderPoint()
    {
        if (wanderPoints.Length == 0) return;

        int index = Random.Range(0, wanderPoints.Length);
        currentWanderTarget = wanderPoints[index];
        agent.isStopped = false;
        agent.SetDestination(currentWanderTarget.position);
    }
    void Chase()
    {
        if (player == null) return;
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void Attack()
    {
        if (player == null) return;

        //Stop to shoot
        agent.isStopped = true;

        Vector3 lookDir = (player.position - transform.position);
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }

        TryShoot(lookDir.normalized);
    }

    bool HasLineOfSight()
    {
        if (player == null) return false;

        Vector3 dir = (player.position - transform.position).normalized;
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, attackRange))
        {
            return hit.collider.CompareTag("Player");
        }
        return false;
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}