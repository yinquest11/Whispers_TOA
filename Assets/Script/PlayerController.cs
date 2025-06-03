using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private CatcherHealth[] _catcherHealths;


    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float runSpeed = 8f;

    
    private int _jumpCount;
    private int _maxJumps = 2;

    private Transform _transform;

    private float _lastAPressedTime = float.NegativeInfinity;
    private float _lastDPressedTime = float.NegativeInfinity;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _transform = GetComponent<Transform>();


        if (_rb == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }
        if (_transform == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }

        _jumpCount = _maxJumps;

    }

    // tommy edit1
    // this is a test for merging conflicts

    void Update()
    {
        HandleMovement();
        HandleJumping();
        
    }

    void HandleMovement()
    {
        if (Input.GetButton("Fire2")) // temperarily method to let catcher fly away
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

        currentSpeed = SmartMovement(ref _moveInput);

        _rb.linearVelocity = new Vector2(_moveInput * currentSpeed, _rb.linearVelocity.y);

        FlipPlayerSprite(_moveInput);



    }

    private float SmartMovement(ref float _moveInput) // Avoid get 0 from GetAxisRaw and cause player stop moving
    {
        float currentSpeed;
        if (Input.GetKeyDown(KeyCode.A))
        {
            _lastAPressedTime = Time.time;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _lastDPressedTime = Time.time;
        }


        bool aHeld = Input.GetKey(KeyCode.A);
        bool dHeld = Input.GetKey(KeyCode.D);


        if (aHeld && dHeld)
        {

            if (_lastDPressedTime > _lastAPressedTime)
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

        return currentSpeed;
    }

    void HandleJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _jumpCount > 0)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            _jumpCount--;
            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //isGrounded = true;
            _jumpCount = _maxJumps;
           
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