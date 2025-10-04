using UnityEngine;

public class SideLaserManager : MonoBehaviour
{
    [Header("Navigation Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement Speed")]
    public float speed = 2f;

    [Header("HorizontalLaserPrefab")]
    public GameObject horizontalLaser;

    private Transform target;

    [Header("HorizontalLaser")]

    public float horizontalOffset = 8f;
    public float laserDuration = 2f; 

    void Start()
    {
        target = pointB; 
    }

    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // switch direction if close
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            target = (target == pointA) ? pointB : pointA; // Swap target
        }
    }

    public void SpawnWithOffset(float xOffset)
    {
        if (horizontalLaser == null) return;

        // calculate spawn position
        Vector3 spawnPos = transform.position + new Vector3(xOffset, 0f, 0f);
        spawnPos.x -= horizontalOffset;

        // spawn and store reference
        GameObject laserInstance = Instantiate(horizontalLaser, spawnPos, Quaternion.identity);

        // destroy the spawned laser after a delay
        Destroy(laserInstance, laserDuration);
    }
}
