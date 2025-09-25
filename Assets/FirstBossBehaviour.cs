using System;
using System.Collections;
using UnityEngine;

public class FirstBossBehaviour : MonoBehaviour
{

    public Vector2 playerPosition;
    public Transform bossHighPosition;
    
    public Rigidbody2D rb;
    public GameObject shadow;
    
    public float jumpForce = 10f;
    public float endJumpForce = 10f;
    public float followSpeed = 5f;

    public bool canFollow = false;
    public bool canStartOnAirCoroutine = true;

    public enum BossState
    {
        onAir,
        onGround
    }
    
    public BossState curretnState = BossState.onGround;

    private bool canStart = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPosition =  GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>().position;

        if (canStart == false)
        {
            StartCoroutine(StartDelay());
        }
        else
        {
            switch (curretnState)
            {
           
                case BossState.onGround:
                    Jump();
                    break;
                case BossState.onAir:
                    OnAir();
                    break;
            }
        }
    }

    public IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(2);
        canStart = true;
    }

    public IEnumerator FollowDelay()
    {
        yield return new WaitForSeconds(1);
        
        rb.linearVelocity = Vector2.zero;
        transform.position = bossHighPosition.position;
        canFollow = true;
    }

    public void Jump()
    {
        curretnState = BossState.onAir;
        canStartOnAirCoroutine = true;
        canFollow = false;
        rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        
        
    }

    public void OnAir()
    {
        if (canStartOnAirCoroutine == true)
        {
            StartCoroutine(FollowDelay());
            canStartOnAirCoroutine = false;
        }
        
        if (canFollow == false)
            return;

        if (Vector2.Distance(transform.position, playerPosition) > 15)
        {
            transform.position = Vector2.MoveTowards(transform.position, playerPosition, followSpeed *  Time.deltaTime);
        }
        else
        {
            rb.AddForce((playerPosition - (Vector2)transform.position).normalized * endJumpForce, ForceMode2D.Impulse);
            curretnState = BossState.onGround;
            canStart = false;
            
        }


    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, playerPosition);
    }
}
