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
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        switch (type)
        {
            case PickupType.Health:
                if(PlayerHealth.Instance != null)
                {
                    PlayerHealth.Instance.AddHealth(amount);
                }
                break;

            case PickupType.Ammo:
                var weapon = other.GetComponentInChildren<Weapon>();
                if (weapon != null)
                {
                    weapon.AddReserveAmmo(amount);
                }
                break;
        }

        Destroy(gameObject);
    }
}
