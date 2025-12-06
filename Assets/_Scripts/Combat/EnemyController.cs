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
    [SerializeField] private float detectionRange = 8f; //chasing range
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

    [Header("Animation")]
    private Animator anim;
    public float maxSpeed = 4f;
    public bool isDead = false;
    Vector3 lastPos;

    [Header("Collision")]
    [SerializeField] private CapsuleCollider enemyCollider;

    [Header("Weapon")]
    [SerializeField] private GameObject gun;

    [Header("Player Pickup")]
    [SerializeField, Range(0f, 1f)] private float dropChance = 0.4f;
    [SerializeField] private GameObject[] pickupPrefabs;

    [Header("XP Reward")]
    [SerializeField] private int xpOnDeath = 5;

    private enum State { Wandering, Chasing, Attacking }
    private State currentState = State.Wandering;

    
    private void Start()
    {
        currentHP = maxHP;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
    

        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Get animator
        if (!anim)
        {
            anim = GetComponentInChildren<Animator>(true);
        }
        lastPos = transform.position;

        PickNewWanderPoint();
    }

    void Update()
    {
        if (currentHP <= 0)
        {
            return;
        }

        // Find player safety (if player dies)
        if (player == null)
        {
         
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // Determine state based on player distance
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

        // Run determined state
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

        // Update movement speed for animation
        float mps = (transform.position - lastPos).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        lastPos = transform.position;
        float speed01 = Mathf.Clamp01(mps / Mathf.Max(maxSpeed, 0.0001f));
        anim.SetFloat("Speed", speed01);

    }

    void Wander()
    {
        wanderCooldown -= Time.deltaTime;

        // if we reach wander target, wait to pick new wander target
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
        if (wanderPoints.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, wanderPoints.Length);
        currentWanderTarget = wanderPoints[index];

        agent.isStopped = false;
        agent.SetDestination(currentWanderTarget.position);
    }

    void Chase()
    {
        if (player == null)
        {
            return;
        }
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    void Attack()
    {
        if (player == null)
        {
            return;
        }

        // Stop moving while shooting
        agent.isStopped = true;

        // Face player
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
        if (player == null)
        {
            return false;
        }

        // Enemy to player raycast
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
            //spawn bullet
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            //launch bullet
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
            //Disable dead enemy collider and play animation
            enemyCollider.enabled = false;
            anim.SetBool("isDead", true);   
            
            // Drop current gun
            var rbGun = gun.AddComponent<Rigidbody>();
            gun.AddComponent<BoxCollider>();
            rbGun.useGravity = true;
            rbGun.isKinematic = false;

            isDead = true;
            // Enemy death with delay for animation
            Invoke("Die", 2f);

        }
    }

    private void Die()
    {
        // Notify game manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnEnemyDeath();
        }

        if(WaveManager.Instance != null)
        {
            WaveManager.Instance.OnEnemyDied();
        }

        // give xp
        if (PlayerProgression.Instance != null)
        {
            PlayerProgression.Instance.AddXP(xpOnDeath);
        }

        TryDropPickup();
        Debug.Log($"{gameObject.name} has died!");
        Destroy(gameObject);
    }

    // Ammo  & Health drops
    private void TryDropPickup()
    {
        if (pickupPrefabs == null || pickupPrefabs.Length == 0)
        {
            return;
        }

        if (Random.value <= dropChance)
        {
            GameObject prefab = pickupPrefabs[Random.Range(0, pickupPrefabs.Length)];
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}