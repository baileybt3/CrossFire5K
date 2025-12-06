using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float lifeTime = 3f;
    public float damage = 10f;
    public float speed = 20f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // Move bullet
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Collision with player
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player != null)
        {
            player.TakeDamage(damage); // Player take damage
        }

        Destroy(gameObject);
    }
}
