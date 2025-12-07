using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Grenade : MonoBehaviour
{
    [Header("Grenade Settings")]
    public float explosionRadius = 4f;
    public float damage = 50f;
    public GameObject explosionVFX; //particle prefab
    public float lifeTime = 5f; //safety

    private bool hasExploded = false;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Arc
        if(rb != null)
        {
            rb.useGravity = true;
            rb.linearVelocity += Vector3.up * 3f;
        }
        Destroy(gameObject, lifeTime);
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
