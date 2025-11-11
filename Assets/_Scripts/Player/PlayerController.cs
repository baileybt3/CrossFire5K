using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] float maxHP = 100f;
    private float currentHP;
    
    [Header("Movement")]
    [SerializeField] float moveSpeed = 7f;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;

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
    [SerializeField]private GameObject gun;
    
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
        
    }

    void Update()
    {
        if(!isDead){
            moveInput = input.Player.Movement.ReadValue<Vector2>();

            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 lookDir = hit.point - transform.position;
                lookDir.y = 0f;

                if (lookDir.sqrMagnitude > 0.01f)
                {
                    transform.forward = lookDir.normalized;
                }

                if (input.Player.Combat.WasPressedThisFrame())
                {
                    Shoot();
                }
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

    void Shoot()
    {
        if (!isDead)
        {
            if (bulletPrefab == null || firePoint == null) return;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            bulletRb.linearVelocity = firePoint.forward * bulletSpeed;
            Destroy(bullet, 3f);
        }
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
            Invoke("Die", 2f);

        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died!");

        GameManager gm = FindFirstObjectByType<GameManager>();

        if(gm != null)
        {
            gm.RespawnPlayer(3f);
        }

        Destroy(gameObject);
    }
}
