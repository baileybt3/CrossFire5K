using UnityEngine;

public class ItemDropManager : MonoBehaviour
{
    public enum PickupType { Health, Ammo }

    [Header("Health Drop")]
    [SerializeField] private PickupType type = PickupType.Health;
    [SerializeField] private int amount = 25;
    [SerializeField] private float rotateSpeed = 90f;

    // Update is called once per frame
    void Update()
    {
        // Item drop spin
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only trigger from player
        if (!other.CompareTag("Player"))
        {
            return;
        }

        switch (type)
        {
            case PickupType.Health:
                // Add health with PlayerHealth singleton
                if(PlayerHealth.Instance != null)
                {
                    PlayerHealth.Instance.AddHealth(amount);
                }
                break;

            case PickupType.Ammo:
                // Get current player weapon
                var player = other.GetComponent<PlayerController>();
                Weapon weapon = null;

                if(player != null)
                {
                    weapon = player.CurrentWeapon;
                }
                else
                {
                    weapon = other.GetComponentInChildren<Weapon>();
                }

                if (weapon != null)
                {
                    weapon.AddAmmo(amount);
                }
                break;
        }

        Destroy(gameObject);
    }
}
