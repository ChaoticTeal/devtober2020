using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayer : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float jumpForce = 5f;

    [SerializeField]
    private float groundDetectRadius = 0.5f;

    [SerializeField]
    private LayerMask whatCountsAsGround;

    [SerializeField]
    private Transform groundDetectPoint;

    [SerializeField]
    private float minGravityScale = 12f;

    [SerializeField]
    private float maxGravityScale;

    [SerializeField]
    private float maxJumpHoldDuration = 0.5f;

    private Animator animator;
    private Vector2 moveInput = new Vector2(0f, 0f);
    private Rigidbody2D rb;
    private bool canMove = true;
    private bool jumpButtonPressed;
    private bool isJumping;
    private bool canJumpAgain = true;
    private bool wasOnGround;
    private float jumpHeldTime;
    private float jumpPressedTime;
    private float gravityDif;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if(canMove)
        {
            Move();
            Jump();
            UpdateGravity();
            if (!wasOnGround && CheckIsOnGround())
                isJumping = false;
        }
        wasOnGround = CheckIsOnGround();
    }

    private void Update()
    {
        UpdateAnims();
    }

    private void UpdateGravity()
    {
        if (isJumping)
        {
            float gravityDif = maxGravityScale - minGravityScale;
            if(jumpButtonPressed)
                jumpHeldTime = Time.time - jumpPressedTime;
            float gravityRatio = (maxJumpHoldDuration - jumpHeldTime) / maxJumpHoldDuration;
            if (gravityRatio < 0)
                gravityRatio = 0;
            rb.gravityScale = minGravityScale + (gravityDif * gravityRatio);
        }
        else
            rb.gravityScale = maxGravityScale;
    }

    /// <summary>
    /// Get movement input from InputSystem
    /// </summary>
    /// <param name="callbackContext"></param>
    public void GetMoveInput(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.valueType.Equals(typeof(Vector2)))
        {
            moveInput = callbackContext.ReadValue<Vector2>();
        }
    }

    /// <summary>
    /// Get jump input from InputSystem
    /// </summary>
    /// <param name="callbackContext"></param>
    public void GetJumpInput(InputAction.CallbackContext callbackContext)
    {
        if (canJumpAgain && !isJumping && callbackContext.performed)
        {
            jumpButtonPressed = true;
            canJumpAgain = false;
            jumpPressedTime = Time.time;
            Debug.Log("Jump started: " + jumpPressedTime);
        }
        if (jumpButtonPressed && callbackContext.canceled)
        {
            jumpButtonPressed = false;
            canJumpAgain = true;
            Debug.Log("Jump ended: " + jumpHeldTime);
        }
    }

    private void Move()
    {
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        if (jumpButtonPressed && !isJumping && CheckIsOnGround())
        {
            isJumping = true;
            rb.AddForce(Vector2.up * jumpForce);
            wasOnGround = false;
        }
    }

    private bool CheckIsOnGround()
    {
        Collider2D[] groundCollider = Physics2D.OverlapCircleAll(groundDetectPoint.position, groundDetectRadius, whatCountsAsGround);
        return (groundCollider.Length > 0);
    }

    /// <summary>
    /// Update animator parameters
    /// </summary>
    private void UpdateAnims()
    {
        animator.SetFloat("speed", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("velocityY", rb.velocity.y);
        if (rb.velocity.x > 0.1)
            animator.SetBool("isFacingRight", true);
        else if (rb.velocity.x < -0.1)
            animator.SetBool("isFacingRight", false);
        animator.SetBool("isOnGround", CheckIsOnGround());
    }
}
