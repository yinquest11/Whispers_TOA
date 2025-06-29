using UnityEngine;

public class Tutorial_GrapplingRope : MonoBehaviour
{

    public RopeController ropeController;
    public LineRenderer m_lineRenderer;

    [Header("General Settings:")]
    [SerializeField] private int percision = 40;
    [Range(0, 20)][SerializeField] private float straightenLineSpeed = 5;

    [Header("Rope Animation Settings:")]
    public AnimationCurve ropeAnimationCurve;
    [Range(0.01f, 4)][SerializeField] private float StartWaveSize = 2;
    float waveSize = 0;

    [Header("Rope Progression:")]
    public AnimationCurve ropeProgressionCurve;
    [SerializeField][Range(1, 50)] private float ropeProgressionSpeed = 1;

    // private helper
    float moveTime = 0;
    bool strightLine = false;
    [SerializeField] public bool _finishDrawingRope = false;

    public bool daZhong
    {
        get
        {
            return strightLine;
        }
    }

    [HideInInspector] public bool isGrappling = true;


    private void Awake()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
        ropeController = GetComponent<RopeController>();
    }

    private void OnEnable()
    {
        moveTime = 0;
        m_lineRenderer.positionCount = percision;
        waveSize = StartWaveSize;
        strightLine = false;

        LinePointsToFirePoint();

        m_lineRenderer.enabled = true;
        _finishDrawingRope = false;
    }

    private void OnDisable()
    {
        m_lineRenderer.enabled = false;
        isGrappling = false;
        _finishDrawingRope = false;
    }

    private void LinePointsToFirePoint()
    {
       

        for (int i = 0; i < percision; i++)
        {
            m_lineRenderer.SetPosition(i, ropeController.transform.position);
        }
    }

    private void Update()
    {
        moveTime += Time.deltaTime;
        DrawRope();
    }

    void DrawRope()
    {
        if (strightLine == false)
        {
            if (Vector2.Distance(m_lineRenderer.GetPosition(percision - 1), ropeController.grapplePoint) < 0.001f)
            {
                // When hit the point   
                strightLine = true;
            }
            else
            {
                // Before hit the point
                DrawRopeWaves();
            }
        }
        else // when hit the point, come here
        {
            
            if (waveSize > 0)
            {
                // Decrease the wave rope
                waveSize -= Time.deltaTime * straightenLineSpeed;
                DrawRopeWaves();
            }
            else
            {
                // Decrease wave size to 0, just draw simple staright line
                waveSize = 0;

                if (m_lineRenderer.positionCount != 2) { m_lineRenderer.positionCount = 2; }

                DrawRopeNoWaves();
            }
        }
    }

    void DrawRopeWaves()
    {
        for (int i = 0; i < percision; i++)
        {
            float delta = (float)i / ((float)percision - 1f);
            Vector2 offset = Vector2.Perpendicular(ropeController.grappleDistanceVector).normalized * ropeAnimationCurve.Evaluate(delta) * waveSize;
            Vector2 targetPosition = Vector2.Lerp(ropeController.transform.position, ropeController.grapplePoint, delta) + offset;
            Vector2 currentPosition = Vector2.Lerp(ropeController.transform.position, targetPosition, ropeProgressionCurve.Evaluate(moveTime) * ropeProgressionSpeed);

            m_lineRenderer.SetPosition(i, currentPosition);
        }

        
    }

    void DrawRopeNoWaves()
    {
        m_lineRenderer.SetPosition(0, ropeController.transform.position);
        m_lineRenderer.SetPosition(1, ropeController.grapplePoint);


        _finishDrawingRope = true;
    }
}