using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SlimeChase : MonoBehaviour
{
    public Transform player;
    public float jumpForce = 6f;
    public float jumpInterval = 1.5f;
    public float moveForce = 3f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private float jumpTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        jumpTimer = jumpInterval;
    }

    void Update()
    {
        jumpTimer -= Time.deltaTime;

        if (IsGrounded() && jumpTimer <= 0f && player != null)
        {
            JumpTowardsPlayer();
            jumpTimer = jumpInterval;
        }
    }

    void JumpTowardsPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        Vector2 jump = new Vector2(dir.x * moveForce, jumpForce);
        rb.linearVelocity = jump;
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
