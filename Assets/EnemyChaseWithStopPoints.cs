//Full Code: EnemyChaseWithStopPoints.cs
using UnityEngine;

public class EnemyChaseWithStopPoints : MonoBehaviour
{
    public float chaseSpeed = 3f;
    public float detectionRange = 5f;
    public Transform chasePointA;
    public Transform chasePointB;

    private Transform player;
    private Animator animator;
    private float leftBound;
    private float rightBound;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();

        leftBound = Mathf.Min(chasePointA.position.x, chasePointB.position.x);
        rightBound = Mathf.Max(chasePointA.position.x, chasePointB.position.x);
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool playerInRange = distanceToPlayer < detectionRange;
        bool playerInBounds = player.position.x > leftBound && player.position.x < rightBound;

        if (playerInRange && playerInBounds)
        {
            Vector2 targetPos = new Vector2(player.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPos, chaseSpeed * Time.deltaTime);

            if ((player.position.x < transform.position.x && transform.localScale.x > 0) ||
                (player.position.x > transform.position.x && transform.localScale.x < 0))
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

    void OnDrawGizmosSelected()
    {
        if (chasePointA != null && chasePointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(chasePointA.position, chasePointA.position + Vector3.up * 2);
            Gizmos.DrawLine(chasePointB.position, chasePointB.position + Vector3.up * 2);
        }
    }
}
