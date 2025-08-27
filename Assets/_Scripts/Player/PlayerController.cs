using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 7f;

    private Rigidbody rb;
    private PlayerInputSystem input; 
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // initialize input
        input = new PlayerInputSystem();
    }

    void OnEnable() => input.Enable();
    void OnDisable() => input.Disable();

    void Update()
    {
        // read Move action (from WASD 2D Vector composite)
        moveInput = input.Player.Movement.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // translate 2D input into XZ movement
        Vector3 dir = new Vector3(moveInput.x, 0f, moveInput.y);

        rb.linearVelocity = new Vector3(
            dir.x * moveSpeed,
            rb.linearVelocity.y,
            dir.z * moveSpeed
        );
    }
}
