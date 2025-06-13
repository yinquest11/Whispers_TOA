
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RopeVerlet : MonoBehaviour
{
    [Header("Rope")]
    [SerializeField] private int _numOfRopeSegment = 50;
    [SerializeField] private float _ropeSegmentLength = 0.225f;

    [Header("Physics")]
    [SerializeField] private Vector2 _gravityForce = new Vector2(0f, -2f);
    [SerializeField] private float _dampingFactor = 0.98f; // optional  
    [SerializeField] private LayerMask _collisionMask;
    [SerializeField] private float _collisionRadius = 0.1f;
    [SerializeField] private float _bounceFactor = 0.1f;
    //[SerializeField] private float _correctionClampAmount = 0.1f;
    private Vector3 viewportPos;


    [Header("Constraints")]
    [SerializeField] private int _numOfConstraintRuns = 50;

    [Header("Optimazation")]
    [SerializeField] private int _collisionSegmentInterval = 2;

    private LineRenderer _lineRenderer;
    private List<RopeSegment> _ropeSegments = new List<RopeSegment>();

    public Vector3 ropeStartPoint;
    public Vector3 ropeEndPoint;

    public Transform playerTransform;
    public Transform boxTransform;

    
    

    private void Awake()
    {
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = _numOfRopeSegment;

        // setting the rope start and end point
        


        for (int i = 0; i < _numOfRopeSegment; ++i)
        {
            _ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= _ropeSegmentLength;

        }
    }


    private void Update()
    {
        ropeStartPoint = playerTransform.position;
        ropeEndPoint = boxTransform.position;

        viewportPos = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height,10);
        DrawRope();

        
    }

    private void FixedUpdate()
    {
        Simulate();

        for(int i = 0; i < _numOfConstraintRuns; ++i)
        {
            
            ApplyConstraints();
            HandleCollision();




        }


    }



    private void DrawRope()
    {
        Vector3[] ropePosition = new Vector3[_numOfRopeSegment];
        for (int i = 0; i < _ropeSegments.Count; ++i) 
        {
            ropePosition[i] = _ropeSegments[i].CurrentPosition;
        }

        _lineRenderer.SetPositions(ropePosition);
    }


    private void Simulate()
    {
        for(int i = 0; i < _ropeSegments.Count; ++i)
        {
            RopeSegment segment = _ropeSegments[i];
            Vector2 velocity = (segment.CurrentPosition - segment.OldPosition) * _dampingFactor;

            segment.OldPosition = segment.CurrentPosition;
            segment.CurrentPosition += velocity;
            segment.CurrentPosition += _gravityForce * Time.fixedDeltaTime;

            _ropeSegments[i] = segment;
        }
    }

    private void ApplyConstraints()
    {
        // keep the first point attached to the mouse
        RopeSegment firstSegment = _ropeSegments[0];
        firstSegment.CurrentPosition = ropeStartPoint;

        _ropeSegments[0] = firstSegment;

        for(int i = 0; i < _numOfRopeSegment-1; ++i)
        {
            RopeSegment currentSeg = _ropeSegments[i];
            RopeSegment nextSeg = _ropeSegments[i + 1];

            float dist = (currentSeg.CurrentPosition - nextSeg.CurrentPosition).magnitude;
            float difference = (dist - _ropeSegmentLength);

            Vector2 changeDir = (currentSeg.CurrentPosition - nextSeg.CurrentPosition).normalized;
            Vector2 changeVector = changeDir * difference;

            if (i != 0)
            {
                currentSeg.CurrentPosition -= (changeVector * 0.5f);
                nextSeg.CurrentPosition += (changeVector * 0.5f);
            }
            else
            {
                nextSeg.CurrentPosition += changeVector;
            }

            _ropeSegments[i] = currentSeg;
            _ropeSegments[i + 1] = nextSeg;
        }

        RopeSegment lastSegment = _ropeSegments[_ropeSegments.Count - 1];
        lastSegment.CurrentPosition = ropeEndPoint;
        _ropeSegments[_ropeSegments.Count - 1] = lastSegment;

        //boxTransform.position = lastSegment.CurrentPosition;

    }


    private void HandleCollision()
    {
        for(int i = 1; i < _ropeSegments.Count; ++i)
        {
            RopeSegment segment = _ropeSegments[i];
            Vector2 velocity = segment.CurrentPosition - segment.OldPosition;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(segment.CurrentPosition, _collisionRadius, _collisionMask);

            foreach(Collider2D collider in colliders)
            {
                Vector2 clossestPoint = collider.ClosestPoint(segment.CurrentPosition);
                float distance = Vector2.Distance(segment.CurrentPosition, clossestPoint);

                // if within the collision radius, resolve
                if (distance < _collisionRadius)
                {
                    Vector2 normal = (segment.CurrentPosition - clossestPoint).normalized;
                    if(normal == Vector2.zero)
                    {
                        // fallback method\
                        normal =  (segment.CurrentPosition - (Vector2)collider.transform.position).normalized;
                    }

                    float depth = _collisionRadius - distance;
                    segment.CurrentPosition += normal * depth;

                    velocity = Vector2.Reflect(velocity, normal) * _bounceFactor;


                }

            }

            segment.OldPosition = segment.CurrentPosition-velocity;
            _ropeSegments[i] = segment;

        }
    }

    public struct RopeSegment
    {
        public Vector2 CurrentPosition;
        public Vector2 OldPosition;

        public RopeSegment(Vector2 pos)
        {
            CurrentPosition = pos;
            OldPosition = pos;
        }
    }
}
