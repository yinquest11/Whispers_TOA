using UnityEngine;
using System.Collections;

public class FinalBossController : MonoBehaviour
{
    [Header("Boss 1 - Jump Attack")]
    public Transform bossHighPosition;
    public float jumpForce = 10f;
    public float endJumpForce = 10f;
    public float followSpeed = 5f;
    private Rigidbody2D rb;

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
    public GameObject missilePrefab;
    public int missileCount = 3;
    public float spreadAngle = 30f;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Shared")]
    public GameObject throwObjectToSpawn;
    private Transform player;

    [Header("Attack Settings")]
    public int minAttacksPerCycle = 2;
    public int maxAttacksPerCycle = 4;
    public float waitTimeBetweenAttacks = 1f;

    private enum AttackType
    {
        JumpSlam,      // Boss 1
        LaserDive,     // Boss 2
        MissileBarrage // Boss 3
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        StartCoroutine(FinalBossRoutine());
    }

    IEnumerator FinalBossRoutine()
    {
        yield return new WaitForSeconds(2f); // Intro delay

        while (true)
        {
            // Decide how many attacks this cycle
            int attackCount = Random.Range(minAttacksPerCycle, maxAttacksPerCycle + 1);

            for (int i = 0; i < attackCount; i++)
            {
                // Randomly pick an attack
                AttackType chosenAttack = (AttackType)Random.Range(0, 3);

                switch (chosenAttack)
                {
                    case AttackType.JumpSlam:
                        yield return StartCoroutine(PerformJumpSlam());
                        break;

                    case AttackType.LaserDive:
                        yield return StartCoroutine(PerformLaserDive());
                        break;

                    case AttackType.MissileBarrage:
                        yield return StartCoroutine(PerformMissileBarrage());
                        break;
                }

                yield return new WaitForSeconds(waitTimeBetweenAttacks);
            }
        }
    }

    // ===== BOSS 1: JUMP SLAM =====
    IEnumerator PerformJumpSlam()
    {
        // Jump up
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(1f);

        // Move to high position
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        yield return StartCoroutine(MoveToPoint(bossHighPosition.position));

        // Follow player horizontally
        float followTime = 0f;
        while (followTime < 1.5f)
        {
            if (player != null)
            {
                Vector2 targetPos = new Vector2(player.position.x, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, targetPos, followSpeed * Time.deltaTime);
            }
            followTime += Time.deltaTime;
            yield return null;
        }

        // Slam down
        rb.bodyType = RigidbodyType2D.Dynamic;
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.AddForce(direction * endJumpForce, ForceMode2D.Impulse);
        }

        // Wait until boss hits the ground or reaches a low Y position
        yield return new WaitUntil(() => transform.position.y <= bossHighPosition.position.y - 5f);

        // Immediately reset to kinematic to prevent bouncing
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Spawn throw object
        if (throwObjectToSpawn != null)
        {
            Instantiate(throwObjectToSpawn, Vector3.zero, Quaternion.identity);
        }

        yield return new WaitForSeconds(0.3f);
    }

    // ===== BOSS 2: LASER DIVE =====
    IEnumerator PerformLaserDive()
    {
        // Go to either point A or B
        Transform startPoint = Random.value > 0.5f ? pointA : pointB;
        yield return StartCoroutine(MoveToPoint(startPoint.position));

        // Go to top
        yield return StartCoroutine(MoveToPoint(topPoint.position));

        // Dive to random X position
        float randomX = Random.Range(pointA.position.x, pointB.position.x);
        float lowerY = (pointA.position.y + pointB.position.y) / 2f;
        Vector2 attackPos = new Vector2(randomX, lowerY);
        yield return StartCoroutine(MoveToPoint(attackPos));

        // Fire laser
        yield return StartCoroutine(FireLaser());

        // Return to top
        yield return StartCoroutine(MoveToPoint(topPoint.position));
    }

    IEnumerator FireLaser()
    {
        if (laserPrefab != null)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.y -= verticalOffset;

            GameObject laser = Instantiate(laserPrefab, spawnPos, Quaternion.identity);

            // Trigger side spawner
            if (spawner != null)
            {
                spawner.SpawnWithOffset(2f);
            }

            // Spawn throw object
            if (throwObjectToSpawn != null)
            {
                Instantiate(throwObjectToSpawn, Vector3.zero, Quaternion.identity);
            }

            Rigidbody2D laserRb = laser.GetComponent<Rigidbody2D>();
            if (laserRb != null) laserRb.bodyType = RigidbodyType2D.Kinematic;

            yield return new WaitForSeconds(laserDuration);

            Destroy(laser);
        }
    }

    // ===== BOSS 3: MISSILE BARRAGE =====
    IEnumerator PerformMissileBarrage()
    {
        // Move to a random stop point
        Transform targetPoint = stopPoints[Random.Range(0, stopPoints.Length)];
        yield return StartCoroutine(MoveToPoint(targetPoint.position));

        // Fire missiles multiple times
        int burstCount = Random.Range(2, 4);
        for (int i = 0; i < burstCount; i++)
        {
            ShootMissiles();
            yield return new WaitForSeconds(0.8f);
        }
    }

    void ShootMissiles()
    {
        if (player == null || missilePrefab == null) return;

        for (int i = 0; i < missileCount; i++)
        {
            GameObject missile = Instantiate(missilePrefab, transform.position, Quaternion.identity);
            float angleOffset = (i - (missileCount - 1) / 2f) * spreadAngle;
            missile.GetComponent<Missile>().Initialize(player, angleOffset);
        }
    }

    // ===== HELPER: MOVEMENT =====
    IEnumerator MoveToPoint(Vector2 target)
    {
        while (Vector2.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }
}