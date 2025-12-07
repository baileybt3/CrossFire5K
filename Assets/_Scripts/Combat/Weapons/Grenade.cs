using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Grenade : MonoBehaviour
{
    [Header("Grenade Settings")]
    public float explosionRadius = 4f;
    public float damage = 50f;
    public GameObject explosionVFX; //particle prefab
    public float lifeTime = 5f; //safety
    public float forwardSpeed = 12f;
    public float upwardSpeed = 5f;

    private bool hasExploded = false;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Launch direction
    public void Launch(Vector3 direction)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        rb.useGravity = true;

        //use weapons fire direction for forward
        rb.linearVelocity = direction.normalized * forwardSpeed + Vector3.up * upwardSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasExploded)
        {
            return;
        }
        Explode();
    }

    private void Explode()
    {
        hasExploded = true;

        // Spawn particles
        if(explosionVFX != null)
        {
            Instantiate(explosionVFX, transform.position, Quaternion.identity);
        }

        //play sound
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayExplosionAt(transform.position);
        }

        // Damage radius
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach(var hit in hits)
        {
            EnemyController enemy = hit.GetComponent<EnemyController>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
