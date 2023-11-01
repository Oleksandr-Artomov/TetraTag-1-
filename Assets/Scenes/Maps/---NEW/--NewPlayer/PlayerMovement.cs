using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : Player
{
    public float speed = 8f;
    public float jumpingPower = 16f;
    public float doubleJumpingPower = 12f;
    public float dashPower = 10f;
    public bool isFacingRight = true;
    public bool hasDoubleJumped = false;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private bool isDashing = false;

    private Vector2 movementInput; // Store movement input

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        dashAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        dashAction.Disable();
    }

    private void Awake()
    {
        moveAction = new InputAction(type: InputActionType.PassThrough);
        moveAction.AddBinding("<Gamepad>/leftStick").WithProcessor("normalize");
        moveAction.AddBinding("<Keyboard>/a").WithProcessor("normalize");
        moveAction.AddBinding("<Keyboard>/d").WithProcessor("normalize");

        jumpAction = new InputAction(type: InputActionType.Button, binding: "<Gamepad>/buttonSouth");
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.performed += _ => Jump();

        dashAction = new InputAction(type: InputActionType.Button, binding: "<Gamepad>/buttonWest");
        dashAction.AddBinding("<Keyboard>/x");
        dashAction.performed += _ => Dash();
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            hasDoubleJumped = false;
        }
        else if (!hasDoubleJumped)
        {
            rb.velocity = new Vector2(rb.velocity.x, doubleJumpingPower);
            hasDoubleJumped = true;
        }
    }

    private void Dash()
    {
        if (!isDashing)
        {
            rb.velocity = new Vector2(isFacingRight ? dashPower : -dashPower, rb.velocity.y);
            isDashing = true;
        }
    }

    private void Update()
    {
        // Read the movement input
        movementInput = moveAction.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        // Apply the movement velocity
        rb.velocity = new Vector2(movementInput.x * speed, rb.velocity.y);
        // Reset dash state when on the ground
        if (IsGrounded())
        {
            isDashing = false;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && movementInput.x < 0f || !isFacingRight && movementInput.x > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
