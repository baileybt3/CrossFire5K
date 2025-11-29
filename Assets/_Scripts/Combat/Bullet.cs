using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Bullet : MonoBehaviour
{
    public float damage = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        var enemy = collision.gameObject.GetComponent<EnemyController>();

        if(enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        
        Destroy(gameObject);
    }

}