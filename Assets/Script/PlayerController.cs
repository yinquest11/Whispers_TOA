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

    private float lastAPressedTime = float.NegativeInfinity; // 初始化为负无穷，确保第一次按下时有效
    private float lastDPressedTime = float.NegativeInfinity;

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

        // 判断当前哪些方向键被按住
        bool aHeld = Input.GetKey(KeyCode.A);
        bool dHeld = Input.GetKey(KeyCode.D);

        // 根据“后按键优先”的逻辑来决定_moveInput
        if (aHeld && dHeld)
        {
            // 如果A和D同时被按住，比较哪个键是最后按下的
            if (lastDPressedTime > lastAPressedTime)
            {
                _moveInput = 1f; // D键是最后按下的，向右移动
            }
            else
            {
                _moveInput = -1f; // A键是最后按下的（或同时按下），向左移动
            }
        }
        else if (aHeld)
        {
            _moveInput = -1f; // 只有A键被按住，向左移动
        }
        else if (dHeld)
        {
            _moveInput = 1f; // 只有D键被按住，向右移动
        }
        // 如果A和D都没有被按住，_moveInput 保持 0，角色停止

        // 判断是否按住Shift键进行奔跑
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

        //animator.SetBool("IsRunning", _moveInput != 0); // 使用新的_moveInput

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
            //Debug.Log("hits");
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