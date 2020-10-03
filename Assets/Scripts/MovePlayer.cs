using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayer : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    private Vector2 moveInput;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.valueType.Equals(typeof(Vector2)))
        {
            moveInput = callbackContext.ReadValue<Vector2>();
            rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        }
    }
}
