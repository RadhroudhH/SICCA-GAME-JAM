using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 3f;
    public float airMultiplier = 0.4f;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    [Header("Jumping")]
    public float jumpForce = 6f;
    public float jumpCooldown = 0.25f;
    private bool readyToJump = true;

    [Header("Crouching")]
    public float crouchYScale = 0.5f;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask whatIsGround;
    private bool grounded;

    public Transform orientation;

    [Header("UI")]
    public TMP_Text speedText;

    private float horizontalInput;
    private float verticalInput;

    private Rigidbody rb;
    private Vector3 moveDirection;

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
            speedText.text = "Speed: " + rb.linearVelocity.magnitude.ToString("F2") + "\nState: " + state;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(
                transform.localScale.x,
                crouchYScale,
                transform.localScale.z
            );
            playerHeight = crouchYScale * 2f;
        }

        if (Input.GetKeyUp(crouchKey))
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
        if (grounded && Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
            desiredMoveSpeed = walkSpeed;
        }
        moveSpeed = desiredMoveSpeed;


        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

 

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        Vector3 targetVelocity = moveDirection.normalized * moveSpeed;

        if (!grounded)
            targetVelocity *= airMultiplier;

        // Preserve vertical velocity for jump/gravity
        rb.linearVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.linearVelocity += Vector3.up * jumpForce;
    }   

    private void ResetJump()
    {
        readyToJump = true;
    }
}
