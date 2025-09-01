using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 7f;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 20f;

    private Rigidbody rb;
    private PlayerInputSystem input;
    private Vector2 moveInput;
    private Camera cam;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.freezeRotation = true;

        input = new PlayerInputSystem();
        cam = Camera.main;
    }

    void OnEnable() => input.Enable();
    void OnDisable() => input.Disable();

    void Update()
    {
        // read Move action (from WASD 2D Vector composite)
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
    }

    void FixedUpdate()
    {
        // translate 2D input into XZ movement
        Vector3 dir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        rb.linearVelocity = new Vector3(
            dir.x * moveSpeed,
            rb.linearVelocity.y,
            dir.z * moveSpeed
        );
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.linearVelocity = firePoint.forward * bulletSpeed;
        Destroy(bullet, 3f);
    }
}
