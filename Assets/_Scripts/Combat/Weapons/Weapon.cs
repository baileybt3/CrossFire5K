using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletLifetime = 3f;

    [Header("Ammo")]
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private int startingReserveAmmo = 90;

    public int CurrentAmmo { get; private set; }
    public int ReserveAmmo { get; private set; }

    private void Awake()
    {
        CurrentAmmo = magazineSize;
        ReserveAmmo = startingReserveAmmo;

        UpdateHUDAmmo();
    }

    private void OnEnable()
    {
        UpdateHUDAmmo();
    }

    public void Fire()
    {
        if (bulletPrefab == null || firePoint == null) return;

        if (CurrentAmmo <= 0)
        {
            UpdateHUDAmmo();
            return;
        }

        CurrentAmmo--;

        
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if(bullet.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.useGravity = false;
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }

        Destroy(bullet, bulletLifetime);

        UpdateHUDAmmo();
    }

    public void Reload()
    {
        int needed = magazineSize - CurrentAmmo;
        if (needed <= 0 || ReserveAmmo <= 0) return;

        int toLoad = Mathf.Min(needed, ReserveAmmo);
        CurrentAmmo += toLoad;
        ReserveAmmo -= toLoad;

        UpdateHUDAmmo();
    }

    private void UpdateHUDAmmo()
    {
        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.UpdateAmmo(CurrentAmmo, ReserveAmmo);
        }
    }
}
