using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private CatcherHealth[] _catcherHealths;


    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float runSpeed = 8f;

    
    private int jumpCount;
    private int maxJumps = 2;

    private Transform _transform;

    private float lastAPressedTime = float.NegativeInfinity;
    private float lastDPressedTime = float.NegativeInfinity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();
        
        jumpCount = maxJumps;

    }


    void Update()
    {
        HandleMovement();
        HandleJumping();
        
    }

    void HandleMovement()
    {
        if (Input.GetButton("Fire2"))
        {
            _catcherHealths = Object.FindObjectsByType<CatcherHealth>(FindObjectsSortMode.None);

            if (_catcherHealths == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }

            foreach (CatcherHealth _catcherHealth in _catcherHealths)
            {
                _catcherHealth.HaveToDie();
            }
            
        }

        float _moveInput = 0f;
        float currentSpeed;

        
        if (Input.GetKeyDown(KeyCode.A))
        {
            lastAPressedTime = Time.time;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            lastDPressedTime = Time.time;
        }

       
        bool aHeld = Input.GetKey(KeyCode.A);
        bool dHeld = Input.GetKey(KeyCode.D);

        
        if (aHeld && dHeld)
        {
            
            if (lastDPressedTime > lastAPressedTime)
            {
                _moveInput = 1f;
            }
            else
            {
                _moveInput = -1f; 
            }
        }
        else if (aHeld)
        {
            _moveInput = -1f; 
        }
        else if (dHeld)
        {
            _moveInput = 1f; 
        }
        
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

        

    }

    void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount--;
            
        }
    }

    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //isGrounded = true;
            jumpCount = maxJumps;
           
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