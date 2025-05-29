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

    private Transform _transform;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
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
        float _moveInput = Input.GetAxisRaw("Horizontal");
        float currentSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = moveSpeed;
        }

        rb.linearVelocity = new Vector2(_moveInput * currentSpeed, rb.linearVelocity.y);

        FlipPlayerSprite(_moveInput);

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

    private void FlipPlayerSprite(float _moveInput)
    {
        if (_moveInput == 1 && transform.localScale.x < 0)
        {
            _transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);

        }
        else if (_moveInput == -1 && transform.localScale.x > 0)
        {
            _transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }
}