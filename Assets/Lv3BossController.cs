using UnityEngine;
using System.Collections;

public class Lv3BossController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Transform[] stopPoints;   // Left, Right, Top positions
    public float waitTime = 1.5f;    // Delay between missile shots
    public float roamTime = 3f;      // Time to roam before stopping
    public Vector2 roamAreaMin;      // Bottom-left roam boundary
    public Vector2 roamAreaMax;      // Top-right roam boundary
    public int shotsPerCycle = 3;    // How many times to shoot before roaming again

    private Transform player;
    private Lv3BossShooter shooter;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        shooter = GetComponent<Lv3BossShooter>();

        StartCoroutine(BossRoutine());
    }

    IEnumerator BossRoutine()
    {
        while (true)
        {
            // 🔹 Roam Phase
            float elapsed = 0f;
            while (elapsed < roamTime)
            {
                Vector2 randomPos = new Vector2(
                    Random.Range(roamAreaMin.x, roamAreaMax.x),
                    Random.Range(roamAreaMin.y, roamAreaMax.y)
                );

                while (Vector2.Distance(transform.position, randomPos) > 0.1f && elapsed < roamTime)
                {
                    transform.position = Vector2.MoveTowards(transform.position, randomPos, moveSpeed * Time.deltaTime);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                yield return null;
            }

            // 🔹 Move to Stop Point
            Transform target = stopPoints[Random.Range(0, stopPoints.Length)];
            while (Vector2.Distance(transform.position, target.position) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // 🔹 Shooting Phase (3 shots)
            for (int i = 0; i < shotsPerCycle; i++)
            {
                shooter.ShootMissiles();
                yield return new WaitForSeconds(waitTime);
            }

            // 🔁 Then loop back to roaming
        }
    }
}