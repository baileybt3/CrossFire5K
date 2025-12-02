using UnityEngine;

public class Weapon : MonoBehaviour
{

    public enum FireMode { SemiAuto, FullAuto}

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletLifetime = 3f;

    [Header("Stats")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private FireMode fireMode = FireMode.SemiAuto;
    [SerializeField] private float fireRate = 10f;

    [Header("Ammo")]
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private int startingReserveAmmo = 90;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip emptyMagazineSound;

    public int CurrentAmmo { get; private set; }
    public int ReserveAmmo { get; private set; }
    public FireMode Mode => fireMode;

    public float fireCooldown;

    private void Awake()
    {
        CurrentAmmo = magazineSize;
        ReserveAmmo = startingReserveAmmo;

        if(audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        fireRate = Mathf.Max(0.01f, fireRate);

        UpdateHUDAmmo();
    }

    private void OnEnable()
    {
        fireCooldown = 0f;
        UpdateHUDAmmo();
    }

    public void HandleFireInput(bool isHeld, bool pressedThisFrame, float deltaTime)
    {
        if(fireCooldown > 0f)
        {
            fireCooldown -= deltaTime;
            return;
        }

        switch (fireMode)
        {
            case FireMode.SemiAuto:
                if (pressedThisFrame && fireCooldown <= 0f)
                {
                    TryFire();
                }
                break;

            case FireMode.FullAuto:
                if (isHeld && fireCooldown <= 0f)
                {
                    TryFire();
                }
                break;
        }
    }
    public void TryFire()
    {
        if (bulletPrefab == null || firePoint == null) return;

        if (CurrentAmmo <= 0)
        {
            PlayOneShot(emptyMagazineSound);
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

        if(bullet.TryGetComponent<Bullet>(out var bulletScript))
        {
            bulletScript.damage = damage;
        }

        Destroy(bullet, bulletLifetime);

        fireCooldown = 1f / fireRate;

        PlayOneShot(fireSound);
        UpdateHUDAmmo();
    }

    public void Reload()
    {
        int needed = magazineSize - CurrentAmmo;
        if (needed <= 0 || ReserveAmmo <= 0) return;

        int toLoad = Mathf.Min(needed, ReserveAmmo);
        CurrentAmmo += toLoad;
        ReserveAmmo -= toLoad;

        PlayOneShot(reloadSound);
        UpdateHUDAmmo();
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    private void UpdateHUDAmmo()
    {
        if (PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.UpdateAmmo(CurrentAmmo, ReserveAmmo);
        }
    }
}
