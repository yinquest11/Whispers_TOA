using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    //private Animator animator;

    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float runSpeed = 8f;

    private bool isGrounded;
    private int jumpCount;
    private int maxJumps = 2;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
        jumpCount = maxJumps;

    }

    
    void Update()
    {
        HandleMovement();
        HandleJumping();
        //UpdateAnimatorStates();
    }

    void HandleMovement()
    {
            
            //rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        float moveInput = Input.GetAxisRaw("Horizontal");
        float currentSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = moveSpeed;
        }

        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);

        //animator.SetBool("IsRunning", moveInput != 0);

    }

    void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount--;
            isGrounded = false;
        }
    }

    //void UpdateAnimatorStates()
    //{
    //    //animator.SetBool("IsGrounded", isGrounded);
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = maxJumps;
            Debug.Log("hits");
        }

    }
}