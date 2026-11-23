using System;
using System.Collections;
using UnityEngine;

public class CustomPlayerController : MonoBehaviour
{
    [Header("Movement")]
    float MoveSpeed;
    public float WalkSpeed;
    public float SprintSpeed;

    public float groundDrag;

    public float jumpForce;
    public float HoverForce;
    public float jumpCooldown;
    public float airMultiplier;

    public float dashTime;
    public float dashSpeed;
    public float dashForce;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool Grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    StaminaController stamina;

    private void Start()
    {
        MoveSpeed = WalkSpeed;
        rb = GetComponent<Rigidbody>();
        stamina = GetComponent<StaminaController>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // ground check
        Grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();

        // handle drag
        if (Grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.LeftShift) && stamina.HasStamina(.3f) && Grounded)
        {
            MoveSpeed = SprintSpeed;
            PlayerCamera.Instance.IsSprinting = true;
            stamina.UpdateStamina(.3f * Time.deltaTime);
        }
        else
        {
            MoveSpeed = WalkSpeed;
            PlayerCamera.Instance.IsSprinting = false;
        }

        // when to jump
        if (Input.GetKeyDown(jumpKey) && Grounded && stamina.HasStamina(10))
        {
            Debug.Log("Jumped");
            Jump();
        }

        else if (Input.GetKey(jumpKey) && !Grounded && stamina.HasStamina(1f))
        {
            rb.AddForce(Vector3.up * HoverForce, ForceMode.Acceleration);
            stamina.UpdateStamina(.3f * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Q) && stamina.HasStamina(20f))
        {
            Dash();
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        //CameraShake.CameraTilt(moveDirection.z);

        // on ground
        if (Grounded)
            rb.AddForce(moveDirection.normalized * MoveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!Grounded)
            rb.AddForce(moveDirection.normalized * MoveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > MoveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * MoveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        stamina.UpdateStamina(10f);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        CameraShake.Shake(0.5f, 0.5f);
    }

    void Dash()
    {
        PlayerCamera.Instance.StartDashTilt();
        CameraShake.Shake(0.5f, 0.5f);
        StartCoroutine(DashCoroutine());
        stamina.UpdateStamina(20f);
    }
    IEnumerator DashCoroutine()
    {
        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {
            rb.AddForce(moveDirection * dashForce, ForceMode.Impulse);
            yield return null;
        }

        PlayerCamera.Instance.StopDashTilt();
    }
}
