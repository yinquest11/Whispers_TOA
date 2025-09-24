using UnityEngine;
using System.Collections;

public class Lv3BossController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public Transform[] stopPoints;
    public float waitTime = 1.5f;
    public float roamTime = 3f;
    public Vector2 roamAreaMin;
    public Vector2 roamAreaMax;
    public int shotsPerCycle = 3;

    [Header("Smooth Movement Options")]
    [SerializeField] private MovementType movementType = MovementType.WaypointCurves;
    [SerializeField] private float waypointRadius = 2f;
    [SerializeField] private float curveSmoothing = 2f;
    [SerializeField] private float orbitRadius = 3f;
    [SerializeField] private float spiralTightness = 0.5f;

    private Transform player;
    private Lv3BossShooter shooter;

    // For waypoint-based movement
    private Vector2[] waypoints;
    private int currentWaypointIndex = 0;

    // For time-based movements
    private Vector2 startPosition;
    private float movementTimer;

    public enum MovementType
    {
        WaypointCurves,     // Smooth curves between random waypoints
        Spiral,             // Expanding/contracting spiral
        Lemniscate,         // Figure-8 with proper curves
        OrganicFloat,       // Perlin noise positions (not directions)
        CircularDrift,      // Circles that drift around
        RandomWalk          // Smooth random walk with momentum
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        shooter = GetComponent<Lv3BossShooter>();

        startPosition = transform.position;
        GenerateWaypoints();

        StartCoroutine(BossRoutine());
    }

    void GenerateWaypoints()
    {
        waypoints = new Vector2[5]; // 5 random waypoints
        Vector2 center = Vector2.Lerp(roamAreaMin, roamAreaMax, 0.5f);

        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = new Vector2(
                Random.Range(roamAreaMin.x, roamAreaMax.x),
                Random.Range(roamAreaMin.y, roamAreaMax.y)
            );
        }
    }

    IEnumerator BossRoutine()
    {
        while (true)
        {
            // 🔹 Roam Phase with chosen movement type
            yield return StartCoroutine(RoamPhase());

            // 🔹 Move to Stop Point
            Transform target = stopPoints[Random.Range(0, stopPoints.Length)];
            yield return StartCoroutine(SmoothMoveToTarget(target.position));

            // 🔹 Shooting Phase
            for (int i = 0; i < shotsPerCycle; i++)
            {
                shooter.ShootMissiles();
                yield return new WaitForSeconds(waitTime);
            }
        }
    }

    IEnumerator RoamPhase()
    {
        movementTimer = 0f;
        startPosition = transform.position;

        if (movementType == MovementType.WaypointCurves)
        {
            GenerateWaypoints(); // Generate new waypoints each roam phase
            currentWaypointIndex = 0;
        }

        while (movementTimer < roamTime)
        {
            Vector2 targetPosition = GetTargetPosition(movementTimer);

            // Smooth movement toward target position
            transform.position = Vector2.Lerp(transform.position, targetPosition,
                Time.deltaTime * curveSmoothing);

            movementTimer += Time.deltaTime;
            yield return null;
        }
    }

    Vector2 GetTargetPosition(float time)
    {
        Vector2 center = Vector2.Lerp(roamAreaMin, roamAreaMax, 0.5f);

        switch (movementType)
        {
            case MovementType.WaypointCurves:
                return GetWaypointCurvePosition(time);

            case MovementType.Spiral:
                return GetSpiralPosition(time, center);

            case MovementType.Lemniscate:
                return GetLemniscatePosition(time, center);

            case MovementType.OrganicFloat:
                return GetOrganicFloatPosition(time, center);

            case MovementType.CircularDrift:
                return GetCircularDriftPosition(time, center);

            case MovementType.RandomWalk:
                return GetRandomWalkPosition(time);

            default:
                return center;
        }
    }

    Vector2 GetWaypointCurvePosition(float time)
    {
        // Progress through waypoints over time
        float progress = (time / roamTime) * waypoints.Length;
        int index = Mathf.FloorToInt(progress);
        float t = progress - index;

        // Get current and next waypoint
        Vector2 current = waypoints[index % waypoints.Length];
        Vector2 next = waypoints[(index + 1) % waypoints.Length];

        // Add curve using a control point
        Vector2 control = Vector2.Lerp(current, next, 0.5f) +
                         Random.insideUnitCircle.normalized * waypointRadius;

        // Quadratic Bezier curve
        return QuadraticBezier(current, control, next, t);
    }

    Vector2 GetSpiralPosition(float time, Vector2 center)
    {
        float angle = time * 2f * Mathf.PI;
        float radius = orbitRadius * (1f + Mathf.Sin(time * spiralTightness) * 0.5f);

        return center + new Vector2(
            Mathf.Cos(angle) * radius,
            Mathf.Sin(angle) * radius
        );
    }

    Vector2 GetLemniscatePosition(float time, Vector2 center)
    {
        float t = time * 0.5f;
        float scale = orbitRadius;

        // Lemniscate (figure-8) parametric equations
        float x = scale * Mathf.Sin(t) / (1 + Mathf.Cos(t) * Mathf.Cos(t));
        float y = scale * Mathf.Sin(t) * Mathf.Cos(t) / (1 + Mathf.Cos(t) * Mathf.Cos(t));

        return center + new Vector2(x, y);
    }

    Vector2 GetOrganicFloatPosition(float time, Vector2 center)
    {
        // Multiple octaves of Perlin noise for organic movement
        float scale1 = orbitRadius;
        float scale2 = orbitRadius * 0.5f;

        float x = Mathf.PerlinNoise(time * 0.3f, 0f) * scale1 +
                  Mathf.PerlinNoise(time * 0.8f, 100f) * scale2;
        float y = Mathf.PerlinNoise(time * 0.4f, 200f) * scale1 +
                  Mathf.PerlinNoise(time * 0.7f, 300f) * scale2;

        // Center the noise around our center point
        x = (x - scale1 * 0.75f);
        y = (y - scale1 * 0.75f);

        return center + new Vector2(x, y);
    }

    Vector2 GetCircularDriftPosition(float time, Vector2 center)
    {
        // Circle that drifts around
        float circleSpeed = 2f;
        float driftSpeed = 0.3f;

        // Main circular motion
        Vector2 circlePos = new Vector2(
            Mathf.Cos(time * circleSpeed) * orbitRadius,
            Mathf.Sin(time * circleSpeed) * orbitRadius
        );

        // Drifting center
        Vector2 driftOffset = new Vector2(
            Mathf.Sin(time * driftSpeed) * orbitRadius * 0.5f,
            Mathf.Cos(time * driftSpeed * 0.7f) * orbitRadius * 0.3f
        );

        return center + circlePos + driftOffset;
    }

    Vector2 GetRandomWalkPosition(float time)
    {
        // This one needs to accumulate position changes
        // For now, use a simplified approach
        Vector2 center = Vector2.Lerp(roamAreaMin, roamAreaMax, 0.5f);

        float noiseX = Mathf.PerlinNoise(time * 0.5f, 0f);
        float noiseY = Mathf.PerlinNoise(0f, time * 0.5f);

        Vector2 offset = new Vector2(
            (noiseX - 0.5f) * (roamAreaMax.x - roamAreaMin.x) * 0.8f,
            (noiseY - 0.5f) * (roamAreaMax.y - roamAreaMin.y) * 0.8f
        );

        return center + offset;
    }

    Vector2 QuadraticBezier(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        float u = 1 - t;
        return u * u * a + 2 * u * t * b + t * t * c;
    }

    IEnumerator SmoothMoveToTarget(Vector2 targetPosition)
    {
        while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition,
                moveSpeed * 1.5f * Time.deltaTime); // Slightly faster for attack positioning
            yield return null;
        }
    }
}