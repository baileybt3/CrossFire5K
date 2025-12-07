using UnityEngine;

public class Weapon : MonoBehaviour
{

    public enum FireMode { SemiAuto, FullAuto}
    public enum WeaponType { Rifle, Pistol, SMG, Utility }

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float bulletLifetime = 3f;

    [Header("Stats")]
    [SerializeField] private WeaponType weaponType = WeaponType.Rifle;
    [SerializeField] private FireMode fireMode = FireMode.SemiAuto;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float fireRate = 10f;
    

    [Header("Ammo")]
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private int startingReserveAmmo = 90;


    public int CurrentAmmo { get; private set; }
    public int ReserveAmmo { get; private set; }
    public FireMode Mode => fireMode;

    // time until next fire
    public float fireCooldown;

    private void Start()
    {
        CurrentAmmo = magazineSize;
        ReserveAmmo = startingReserveAmmo;
        UpdateHUDAmmo();
    }

    private void OnEnable()
    {
        fireCooldown = 0f;
        UpdateHUDAmmo();
    }

    public void HandleFireInput(bool isHeld, bool pressedThisFrame, float deltaTime)
    {
        // reduce cooldown each frame
        if (fireCooldown > 0f)
        {
            fireCooldown -= deltaTime;
            return;
        }

        // Determine which firemode to use
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

    public void Fire()
    {
        if (fireCooldown <= 0f)
        {
            TryFire();
        }
    }

    public void TryFire()
    {
        // if mag empty
        if (CurrentAmmo <= 0)
        {
            UpdateHUDAmmo();
            return;
        }

        CurrentAmmo--;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Grenade grenade = bullet.GetComponentInChildren<Grenade>();

        // if Grenade, use grenade script
        if(grenade != null)
        {
            grenade.damage = damage; // explosion damage
            grenade.Launch(firePoint.forward); //launch direction
        }
        else
        {
            // Push bullet
            if (bullet.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.useGravity = false;
                rb.linearVelocity = firePoint.forward * bulletSpeed;
            }

            // Bullet damage
            if (bullet.TryGetComponent<Bullet>(out var bulletScript))
            {
                bulletScript.damage = damage;
            }


            Destroy(bullet, bulletLifetime);
        }
        
        fireCooldown = 1f / fireRate;

        PlayFireSound();
        UpdateHUDAmmo();
    }

    public void Reload()
    {
        // Full mag
        if (CurrentAmmo >= magazineSize)
        {
            return;
        }

        // No reserve ammo
        if(ReserveAmmo <= 0)
        {
            return;
        }

        int needed = magazineSize - CurrentAmmo; //Amount needed to fill mag
        int toLoad = Mathf.Min(needed, ReserveAmmo); // How many we need to load

        CurrentAmmo += toLoad;
        ReserveAmmo -= toLoad;

        UpdateHUDAmmo();

        // Play reload sound
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayReload();
        }
    }

    // Pickup call for ammo
    public void AddAmmo(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        ReserveAmmo += amount;
        UpdateHUDAmmo();
    }

    private void PlayFireSound()
    {
        if (AudioManager.Instance == null)
        {
            return;
        }

        switch (weaponType)
        {
            case WeaponType.Rifle:
                AudioManager.Instance.PlayRifleShot();
                break;

            case WeaponType.SMG:
                AudioManager.Instance.PlaySMGShot();
                break;

            case WeaponType.Pistol:
                AudioManager.Instance.PlayPistolShot();
                break;            
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
