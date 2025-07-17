using UnityEngine;

public class EnemyChaseWithRaycast : MonoBehaviour
{
    public float chaseSpeed = 3f;
    public float detectionRange = 6f;
    public LayerMask playerLayer;

    private Transform player;
    private Animator animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 directionToPlayer = player.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer.normalized, detectionRange, playerLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            Vector2 target = new Vector2(player.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, target, chaseSpeed * Time.deltaTime);

            if (
                (player.position.x < transform.position.x && transform.localScale.x > 0) ||
                (player.position.x > transform.position.x && transform.localScale.x < 0)
            )
            {
                Flip();
            }

            animator.SetBool("isChasing", true);
        }
        else
        {
            animator.SetBool("isChasing", false);
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        float direction = transform.localScale.x > 0 ? 1f : -1f;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * direction * 5f);
    }
}
