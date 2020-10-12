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

    private Animator animator;
    private Vector2 moveInput = new Vector2(0f, 0f);
    private Rigidbody2D rb;
    private bool canMove = true;
    private bool shouldJump;
    private bool isJumping;
    private bool wasOnGround;

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
            if (!wasOnGround && CheckIsOnGround())
                isJumping = false;
        }
        wasOnGround = CheckIsOnGround();
    }

    private void Update()
    {
        UpdateAnims();
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
        if (!isJumping && callbackContext.performed)
            shouldJump = true;
    }

    private void Move()
    {
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
    }

    private void Jump()
    {
        if(shouldJump && !isJumping && CheckIsOnGround())
        {
            isJumping = true;
            shouldJump = false;
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
