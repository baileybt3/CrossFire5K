using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float maxHP = 100f;
    private float currentHP;
    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;
    
    [Header("Movement")]
    [SerializeField] float moveSpeed = 7f;

    [Header("Animation")]
    private Animator anim;
    public float maxSpeed = 4f;
    public bool isDead;
    Vector3 lastPos;

    [Header("Utils")]
    private Rigidbody rb;
    private PlayerInputSystem input;
    private Vector2 moveInput;
    private Camera cam;

    [Header("Weapon")]
    [SerializeField] private Transform weaponSpawnPoint;
    [SerializeField] private Transform primaryHolsterPoint;
    [SerializeField] private Transform secondaryHolsterPoint;
    [SerializeField] private Transform utilityHolsterPoint;

    [SerializeField] private GameObject gun;

    private GameObject primaryInstance;
    private GameObject secondaryInstance;
    private GameObject utilityInstance;

    private Weapon primaryWeapon;
    private Weapon secondaryWeapon;
    private Weapon utilityWeapon;

    private GameObject currentWeaponInstance;
    private Weapon currentWeapon;
    public Weapon CurrentWeapon => currentWeapon;
    private enum WeaponSlot { None, Primary, Secondary, Utility }
    private WeaponSlot currentSlot = WeaponSlot.None;

    private PauseMenuUI pauseMenu;

    void OnEnable() => input.Enable();
    void OnDisable() => input.Disable();
    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;

        input = new PlayerInputSystem();

        cam = Camera.main;

    }

    private void Start()
    {
        currentHP = maxHP;
        if (!anim) anim = GetComponentInChildren<Animator>(true);
        lastPos = transform.position;
        isDead = false;

        EquipPrimaryFromLoadout();
        
    }

    void Update()
    {
        if (input.Player.System.WasPressedThisFrame())
        {
            if (pauseMenu != null)
            {
                pauseMenu.TogglePause();
            }
        }

        if (pauseMenu != null && pauseMenu.IsPaused)
        {
            anim.SetFloat("Speed", 0f);
            return;
        }
        
        if(!isDead){
            // Movement input
            moveInput = input.Player.Movement.ReadValue<Vector2>();

            // Aim towards mouse position
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 lookDir = hit.point - transform.position;
                lookDir.y = 0f;

                if (lookDir.sqrMagnitude > 0.01f)
                {
                    transform.forward = lookDir.normalized;
                }
            }

            // Fire input
            if(currentWeapon != null)
            {
                bool fireHeld = input.Player.Combat.IsPressed();
                bool firePressed = input.Player.Combat.WasPressedThisFrame();
                currentWeapon.HandleFireInput(fireHeld, firePressed, Time.deltaTime);
            }

            // Reload input
            if (input.Player.Reload.WasPressedThisFrame())
            {
                if (currentWeapon != null)
                {
                    currentWeapon.Reload();
                }
            }

            // Weapon swap input
            if (input.Player.PrimaryWeapon.WasPressedThisFrame())
            {
                EquipWeaponSlot(WeaponSlot.Primary);
            }

            if (input.Player.SecondaryWeapon.WasPressedThisFrame())
            {
                EquipWeaponSlot(WeaponSlot.Secondary);
            }

            if (input.Player.UtilityWeapon.WasPressedThisFrame())
            {
                EquipWeaponSlot(WeaponSlot.Utility);
            }

            float mps = (transform.position - lastPos).magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
            lastPos = transform.position;
            float speed01 = Mathf.Clamp01(mps / Mathf.Max(maxSpeed, 0.0001f));
            anim.SetFloat("Speed", speed01);
        }
    }

    void FixedUpdate()
    {
        if (!isDead)
        {
            Vector3 dir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            rb.linearVelocity = new Vector3(
                dir.x * moveSpeed,
                rb.linearVelocity.y,
                dir.z * moveSpeed
            );
        }
    }

    public void Initialize(PauseMenuUI pauseMenuUI)
    {
        pauseMenu = pauseMenuUI;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. HP left: {currentHP}");

        if (currentHP <= 0f)
        {
            anim.SetBool("isDead", true);

            var rbGun = gun.AddComponent<Rigidbody>();
            gun.AddComponent<BoxCollider>();
            rbGun.useGravity = true;
            rbGun.isKinematic = false;

            isDead = true;
            Destroy(secondaryInstance);
            Destroy(utilityInstance);
            Invoke("Die", 2f);

        }
    }

    private void Die()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDeath();
        }

        Debug.Log($"{gameObject.name} has died!");

        Destroy(gameObject);
    }

    private void EquipPrimaryFromLoadout()
    {
        if(weaponSpawnPoint == null)
        {
            Debug.LogWarning("WeaponSpawnPoint not assigned on PlayerController.");
            return;
        }

        if (ArmoryManager.Instance == null)
        {
            Debug.LogWarning("No ArmoryManager found, using default gun.");
            return;
        }

        // Clean old instances for respawn
        if (primaryInstance != null) Destroy(primaryInstance);
        if (secondaryInstance != null) Destroy(secondaryInstance);
        if (utilityInstance != null) Destroy(utilityInstance);

        // Get current loadout prefabs
        GameObject primaryPrefab = ArmoryManager.Instance.GetActivePrimary();
        GameObject secondaryPrefab = ArmoryManager.Instance.GetActiveSecondary();
        GameObject utilityPrefab = ArmoryManager.Instance.GetActiveUtility();

        // Spawn prefabs at holster points
        if (primaryPrefab != null && primaryHolsterPoint != null)
        {
            primaryInstance = Instantiate(
                    primaryPrefab,
                    primaryHolsterPoint.position,
                    primaryHolsterPoint.rotation,
                    primaryHolsterPoint
            );
            primaryWeapon = primaryInstance.GetComponentInChildren<Weapon>();
        }

        if (secondaryPrefab != null && secondaryHolsterPoint != null)
        {
            secondaryInstance = Instantiate(
                    secondaryPrefab,
                    secondaryHolsterPoint.position,
                    secondaryHolsterPoint.rotation,
                    secondaryHolsterPoint
            );
            secondaryWeapon = secondaryInstance.GetComponentInChildren<Weapon>();
        }

        if (utilityPrefab != null && utilityHolsterPoint != null)
        {
            utilityInstance = Instantiate(
                    utilityPrefab,
                    utilityHolsterPoint.position,
                    utilityHolsterPoint.rotation,
                    utilityHolsterPoint
            );
            utilityWeapon = utilityInstance.GetComponentInChildren<Weapon>();
        }

        if (primaryPrefab == null)
        {
            Debug.LogWarning("Active loadout has no primary weapon set.");
            return;
        }

        if(primaryWeapon != null)
        {
            EquipWeaponSlot(WeaponSlot.Primary);
        }
        else if (secondaryWeapon != null)
        {
            EquipWeaponSlot(WeaponSlot.Secondary);
        }
        else if (utilityWeapon != null)
        {
            EquipWeaponSlot(WeaponSlot.Utility);
        }
        else
        {
            Debug.LogWarning("No weapons found in active loadout.");
        }
    }

    private void EquipWeaponSlot(WeaponSlot slot)
    {
        currentSlot = slot;

        // Put everything back on holsters first
        if (primaryInstance != null && primaryHolsterPoint != null)
        {
            primaryInstance.transform.SetParent(primaryHolsterPoint);
            primaryInstance.transform.localPosition = Vector3.zero;
            primaryInstance.transform.localRotation = Quaternion.identity;
        }

        if (secondaryInstance != null && secondaryHolsterPoint != null)
        {
            secondaryInstance.transform.SetParent(secondaryHolsterPoint);
            secondaryInstance.transform.localPosition = Vector3.zero;
            secondaryInstance.transform.localRotation = Quaternion.identity;
        }

        if (utilityInstance != null && utilityHolsterPoint != null)
        {
            utilityInstance.transform.SetParent(utilityHolsterPoint);
            utilityInstance.transform.localPosition = Vector3.zero;
            utilityInstance.transform.localRotation = Quaternion.identity;
        }

        currentWeaponInstance = null;
        currentWeapon = null;

        // Move chosen one to hands
        switch (slot)
        {
            case WeaponSlot.Primary:
                if (primaryInstance != null)
                {
                    primaryInstance.transform.SetParent(weaponSpawnPoint);
                    primaryInstance.transform.localPosition = Vector3.zero;
                    primaryInstance.transform.localRotation = Quaternion.identity;
                    currentWeaponInstance = primaryInstance;
                    currentWeapon = primaryWeapon;
                }
                break;

            case WeaponSlot.Secondary:
                if (secondaryInstance != null)
                {
                    secondaryInstance.transform.SetParent(weaponSpawnPoint);
                    secondaryInstance.transform.localPosition = Vector3.zero;
                    secondaryInstance.transform.localRotation = Quaternion.identity;
                    currentWeaponInstance = secondaryInstance;
                    currentWeapon = secondaryWeapon;
                }
                break;

            case WeaponSlot.Utility:
                if (utilityInstance != null)
                {
                    utilityInstance.transform.SetParent(weaponSpawnPoint);
                    utilityInstance.transform.localPosition = Vector3.zero;
                    utilityInstance.transform.localRotation = Quaternion.identity;
                    currentWeaponInstance = utilityInstance;
                    currentWeapon = utilityWeapon;
                }
                break;
        }

        // Keep gun reference in sync for death ragdoll logic
        gun = currentWeaponInstance;

        // Refresh ammo HUD for the newly equipped weapon
        if (currentWeapon != null && PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.UpdateAmmo(currentWeapon.CurrentAmmo, currentWeapon.ReserveAmmo);
        }
    }

}

