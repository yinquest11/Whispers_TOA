using UnityEngine;
using System.Collections;

public class FinalBossController : MonoBehaviour
{
    [Header("Boss 1 - Jump Attack")]
    public Transform bossHighPosition;
    public Rigidbody2D rb;
    public float jumpForce = 10f;
    public float endJumpForce = 10f;
    public float followSpeed = 5f;

    [Header("Boss 2 - Laser Attack")]
    public Transform pointA;
    public Transform pointB;
    public Transform topPoint;
    public GameObject laserPrefab;
    public float laserDuration = 0.5f;
    public float verticalOffset = 8f;
    public SideLaserManager spawner;

    [Header("Boss 3 - Missile Attack")]
    public Transform[] stopPoints;
    public float moveSpeed = 3f;
    public float waitTime = 1.5f;
    public GameObject missilePrefab;
    public int missileCount = 3;
    public float spreadAngle = 30f;
    public int shotsPerCycle = 3;

    [Header("Shared")]
    public GameObject throwObjectToSpawn;
    private Transform player;
    public float waitTimeBetweenAttacks = 2f;

    // Boss 1 variables
    private bool canFollow = false;
    private bool canStartOnAirCoroutine = true;

    // Boss 2 variables
    private bool goingToA = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        StartCoroutine(FinalBossRoutine());
    }

    IEnumerator FinalBossRoutine()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            // Attack 1: Jump Slam (Boss 1)
            yield return StartCoroutine(Boss1Attack());
            yield return new WaitForSeconds(waitTimeBetweenAttacks);

            // Attack 2: Laser Dive (Boss 2)
            yield return StartCoroutine(Boss2Attack());
            yield return new WaitForSeconds(waitTimeBetweenAttacks);

            // Attack 3: Missile Barrage (Boss 3)
            yield return StartCoroutine(Boss3Attack());
            yield return new WaitForSeconds(waitTimeBetweenAttacks);
        }
    }

    // ===== BOSS 1: EXACT COPY =====
    IEnumerator Boss1Attack()
    {
        // Reset Boss 1 variables
        canStartOnAirCoroutine = true;
        canFollow = false;

        // Jump
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);

        // Wait and follow logic (exact from FirstBossBehaviour)
        yield return StartCoroutine(Boss1FollowDelay());

        // Wait for OnAir behavior to complete
        while (canFollow == true)
        {
            Boss1OnAir();
            yield return null;
        }

        // Wait a bit before next attack
        yield return new WaitForSeconds(1f);
    }

    IEnumerator Boss1FollowDelay()
    {
        yield return new WaitForSeconds(1f);
        
        rb.linearVelocity = Vector2.zero;
        transform.position = bossHighPosition.position;
        canFollow = true;
    }

    void Boss1OnAir()
    {
        if (canFollow == false)
            return;

        if (Vector2.Distance(transform.position, player.position) > 15)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, followSpeed * Time.deltaTime);
        }
        else
        {
            rb.AddForce((player.position - transform.position).normalized * endJumpForce, ForceMode2D.Impulse);
            
            // Instantiate Throw Object
            if (throwObjectToSpawn != null)
            { 
                Instantiate(throwObjectToSpawn, Vector3.zero, Quaternion.identity);
            }
            
            canFollow = false;
        }
    }

    // ===== BOSS 2: EXACT COPY =====
    IEnumerator Boss2Attack()
    {
        // Ensure kinematic for Boss 2
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;

        // Go to either point A or B
        Transform startPoint = goingToA ? pointA : pointB;
        yield return StartCoroutine(MoveToPoint(startPoint.position));

        // Go to top point
        yield return StartCoroutine(MoveToPoint(topPoint.position));

        // Pick random X between A and B, go down
        float randomX = Random.Range(pointA.position.x, pointB.position.x);
        float lowerY = (pointA.position.y + pointB.position.y) / 2f;
        Vector2 attackPos = new Vector2(randomX, lowerY);
        yield return StartCoroutine(MoveToPoint(attackPos));

        // Attack here
        yield return StartCoroutine(FireLaser());

        // Return to top
        yield return StartCoroutine(MoveToPoint(topPoint.position));

        goingToA = !goingToA;
    }

    IEnumerator FireLaser()
    {
        if (laserPrefab != null)
        {
            // Start from boss position
            Vector3 spawnPos = transform.position;

            // Offset downwards
            spawnPos.y -= verticalOffset;

            GameObject laser = Instantiate(laserPrefab, spawnPos, Quaternion.identity);

            // Trigger side spawner to spawn additional lasers
            if (spawner != null)
            {
                spawner.SpawnWithOffset(2f);
            }

            if (throwObjectToSpawn != null)
            {
                Instantiate(throwObjectToSpawn, Vector3.zero, Quaternion.identity);
            }

            // Prevent physics interaction
            Rigidbody2D laserRb = laser.GetComponent<Rigidbody2D>();
            if (laserRb != null) laserRb.bodyType = RigidbodyType2D.Kinematic;

            yield return new WaitForSeconds(laserDuration);

            Destroy(laser);
        }
    }

    // ===== BOSS 3: EXACT COPY =====
    IEnumerator Boss3Attack()
    {
        // Ensure kinematic for Boss 3
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;

        // Move to Stop Point
        Transform target = stopPoints[Random.Range(0, stopPoints.Length)];
        yield return StartCoroutine(SmoothMoveToTarget(target.position));

        // Shooting Phase
        for (int i = 0; i < shotsPerCycle; i++)
        {
            ShootMissiles();
            yield return new WaitForSeconds(waitTime);
        }
    }

    void ShootMissiles()
    {
        if (player == null) return;

        for (int i = 0; i < missileCount; i++)
        {
            GameObject missile = Instantiate(missilePrefab, transform.position, Quaternion.identity);

            // Offset angles so missiles don't overlap perfectly
            float angleOffset = (i - (missileCount - 1) / 2f) * spreadAngle;

            missile.GetComponent<Missile>().Initialize(player, angleOffset);
        }
    }

    IEnumerator SmoothMoveToTarget(Vector2 targetPosition)
    {
        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition,
                moveSpeed * 1.5f * Time.deltaTime);
            yield return null;
        }
    }

    // ===== HELPER: MOVEMENT (Boss 2 style) =====
    IEnumerator MoveToPoint(Vector2 target)
    {
        while (Vector2.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }
}