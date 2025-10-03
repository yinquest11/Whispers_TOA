using UnityEngine;
using System.Collections;


public class Boss2Script : MonoBehaviour
{
    [Header("Points")]
    public Transform pointA;
    public Transform pointB;
    public Transform topPoint;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Attack")]
    public GameObject laserPrefab;  // assign in Inspector
    public float laserDuration = 0.5f;

    private bool goingToA = true;

    void Start()
    {
        StartCoroutine(BossRoutine());
    }

    private IEnumerator BossRoutine()
    {
        while (true)
        {
            // 1. Go to either point A or B
            Transform startPoint = goingToA ? pointA : pointB;
            yield return StartCoroutine(MoveToPoint(startPoint.position));

            // 2. Go to top point
            yield return StartCoroutine(MoveToPoint(topPoint.position));

            // 3. Pick random X between A and B, go down
            float randomX = Random.Range(pointA.position.x, pointB.position.x);
            float lowerY = (pointA.position.y + pointB.position.y) / 2f;
            Vector2 attackPos = new Vector2(randomX, lowerY);
            yield return StartCoroutine(MoveToPoint(attackPos));

            // Attack here
            yield return StartCoroutine(FireLaser());

            // 4. Return to top
            yield return StartCoroutine(MoveToPoint(topPoint.position));

            // 5. Toggle target
            goingToA = !goingToA;
        }
    }

    private IEnumerator MoveToPoint(Vector2 target)
    {
        while (Vector2.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = target;
    }

    private IEnumerator FireLaser()
    {
        if (laserPrefab != null)
        {
            // Start from boss position
            Vector3 spawnPos = transform.position;

            // Offset downwards (adjust this number to move the laser lower)
            float verticalOffset = 8f; // 2 Unity units below the boss
            spawnPos.y -= verticalOffset;

            GameObject laser = Instantiate(laserPrefab, spawnPos, Quaternion.identity);

            // Make sure laser doesn't push the boss
            Rigidbody2D rb = laser.GetComponent<Rigidbody2D>();
            if (rb != null) rb.bodyType = RigidbodyType2D.Kinematic;

            yield return new WaitForSeconds(laserDuration);

            Destroy(laser);
        }
    }
}



