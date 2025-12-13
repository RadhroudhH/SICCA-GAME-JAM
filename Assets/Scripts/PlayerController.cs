using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Speeds")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 3f;
    public float airControlMultiplier = 0.4f;

    [Header("Jumping")]
    public float jumpForce = 6f;
    public float jumpCooldown = 0.25f;
    private bool readyToJump = true;

    [Header("Crouching")]
    public float crouchYScale = 0.5f;
    private float startYScale;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    private bool grounded;

    [Header("Orientation")]
    public Transform orientation;

    [Header("UI")]
    public TMP_Text speedText;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;

    private float moveSpeed;
    private Rigidbody rb;

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    public MovementState state;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = true;

        startYScale = transform.localScale.y;
        moveSpeed = walkSpeed;
    }

    private void Update()
    {
        grounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            (playerHeight * 0.5f) + 0.3f,
            whatIsGround
        );

        MyInput();
        StateHandler();

        if (speedText != null)
            speedText.text = $"Speed: {rb.linearVelocity.magnitude:F1}\nState: {state}";
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.Space) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(
                transform.localScale.x,
                crouchYScale,
                transform.localScale.z
            );
            playerHeight = crouchYScale * 2f;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(
                transform.localScale.x,
                startYScale,
                transform.localScale.z
            );
            playerHeight = startYScale * 2f;
        }
    }

    private void StateHandler()
    {
        if (grounded && Input.GetKey(KeyCode.LeftControl))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (grounded && Input.GetKey(KeyCode.LeftShift))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput +
                        orientation.right * horizontalInput;

        Vector3 targetVelocity = moveDirection.normalized * moveSpeed;

        if (!grounded)
            targetVelocity *= airControlMultiplier;

        // Preserve Y velocity (gravity, jump, abilities)
        rb.linearVelocity = new Vector3(
            targetVelocity.x,
            rb.linearVelocity.y,
            targetVelocity.z
        );
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        rb.linearVelocity += Vector3.up * jumpForce;
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
