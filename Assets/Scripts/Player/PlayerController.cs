using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NUnit.Framework;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float crouchSpeed = 3f;
    public float groundDrag = 4f;
    public float airMultiplier = 0.4f;

    private float moveSpeed;

    [Header("Jumping")]
    public float jumpForce = 6f;
    public float jumpCooldown = 0.25f;
    private bool readyToJump = true;

    [Header("Crouching")]
    public float crouchYScale = 0.5f;
    public float crouchSmoothSpeed = 8f;
    private float startYScale;
    private float targetYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight = 10f;
    public LayerMask whatIsGround;
    private bool grounded;

    public Transform orientation;

    [Header("UI")]
    public TMP_Text speedText;
    public GameObject EndScreen;

    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepSource;

    [SerializeField] private List<AudioClip> walkSteps = new List<AudioClip>();
    [SerializeField] private List<AudioClip> sprintSteps = new List<AudioClip>();
    [SerializeField] private List<AudioClip> crouchSteps = new List<AudioClip>();

    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.35f;
    [SerializeField] private float crouchStepInterval = 0.7f;

    private float stepTimer;


    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;
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

        startYScale = transform.localScale.y;
        targetYScale = startYScale;
 
    }

    private void Update()
    {
        float rayLength = (playerHeight * 0.5f) + 0.2f;

        grounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            rayLength,
            whatIsGround
        );

        Debug.DrawRay(transform.position, Vector3.down * rayLength, grounded ? Color.green : Color.red);

        //Debug.DrawRay(transform.position, Vector3.down * ((playerHeight * transform.localScale.y * 0.5f) + 0.3f));
        MyInput();
        StateHandler();
        SpeedControl();
        rb.linearDamping = grounded ? groundDrag : 0f;
        float currentY = transform.localScale.y;
        float newY = Mathf.Lerp(currentY, targetYScale, Time.deltaTime * crouchSmoothSpeed);
        transform.localScale = new Vector3(transform.localScale.x, newY, transform.localScale.z);
        if (speedText != null)
            speedText.text = "Speed: " + rb.linearVelocity.magnitude.ToString("F2") +
                             "\nState: " + state;
        if (Input.GetKeyDown(KeyCode.T))
        {
            Time.timeScale = 0.5f;
        }

        HandleFootsteps();

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // JUMP
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // CROUCH
        if (Input.GetKey(crouchKey))
        {
            targetYScale = crouchYScale;
        }
        else
        {
            targetYScale = startYScale;
        }
    }

    private void StateHandler()
    {
        if (!grounded)
        {
            state = MovementState.air;
            return;
        }

        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void HandleFootsteps()
    {
        if (!grounded)
            return;

        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVelocity.magnitude < 0.1f)
            return;

        stepTimer -= Time.deltaTime;
        if (stepTimer > 0f)
            return;

        AudioClip clip = null;

        switch (state)
        {
            case MovementState.walking:
                clip = GetRandomClip(walkSteps);
                stepTimer = walkStepInterval;
                break;

            case MovementState.sprinting:
                clip = GetRandomClip(sprintSteps);
                stepTimer = sprintStepInterval;
                break;

            case MovementState.crouching:
                clip = GetRandomClip(crouchSteps);
                stepTimer = crouchStepInterval;
                break;
        }

        if (clip != null)
            footstepSource.PlayOneShot(clip);
    }

    private AudioClip GetRandomClip(List<AudioClip> clips)
    {
        if (clips == null || clips.Count == 0)
            return null;

        return clips[Random.Range(0, clips.Count)];
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("WinGate"))
        {
            EndScreen.SetActive(true);
            Time.timeScale = 0f;
        }
    }


}
