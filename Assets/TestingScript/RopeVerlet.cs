
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RopeVerlet : MonoBehaviour
{
    
    [SerializeField] private int _numOfRopeSegment = 50; //
    [SerializeField] private float _ropeSegmentLength = 0.225f; // 
    [SerializeField] private Vector2 _gravityForce = new Vector2(0f, -2f); //
    private LineRenderer _lineRenderer; //
    private List<RopeSegment> _ropeSegments = new List<RopeSegment>(); //


    [SerializeField] private float _dampingFactor = 0.98f;

    [SerializeField] private LayerMask _collisionMask;

    [SerializeField] private float _collisionRadius = 0.1f;

    [SerializeField] private float _bounceFactor = 0.1f;    

    [SerializeField] private int _numOfConstraintRuns = 50;


    // My temperorily variable
    public Vector3 ropeStartPoint;
    public Vector3 ropeEndPoint;

    public Transform playerTransform;
    public Transform boxTransform;

    
    

    private void Awake()
    {
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();

        _lineRenderer = GetComponent<LineRenderer>(); //
        _lineRenderer.positionCount = _numOfRopeSegment;


        for (int i = 0; i < _numOfRopeSegment; ++i) //
        {
            _ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= _ropeSegmentLength;
        }
    }


    private void Update()
    {
        ropeStartPoint = playerTransform.position; 
        ropeEndPoint = boxTransform.position;
        
        DrawRope(); //

        
    }

    private void FixedUpdate()
    {
        Simulate(); //

        for(int i = 0; i < _numOfConstraintRuns; ++i)
        {         
            ApplyConstraints();
            HandleCollision();
        }
    }



    private void DrawRope()
    {
        Vector3[] ropePosition = new Vector3[_numOfRopeSegment]; //

        for (int i = 0; i < _ropeSegments.Count; ++i)  //
        {
            ropePosition[i] = _ropeSegments[i].CurrentPosition;
        }

        _lineRenderer.SetPositions(ropePosition); //
    }


    private void Simulate() //
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
        RopeSegment firstSegment = _ropeSegments[0]; //
        firstSegment.CurrentPosition = ropeStartPoint; //

        _ropeSegments[0] = firstSegment; //

        for(int i = 0; i < _numOfRopeSegment-1; ++i)
        {
            RopeSegment currentSeg = _ropeSegments[i]; // 
            RopeSegment nextSeg = _ropeSegments[i + 1];  //


            Vector2 direction = currentSeg.CurrentPosition - nextSeg.CurrentPosition; //

            if (direction.magnitude == 0f) continue; //

            float difference = direction.magnitude - _ropeSegmentLength; //
    
            Vector2 changeVector = direction.normalized * difference; //

            if (i == 0) //
            {
                nextSeg.CurrentPosition += changeVector;
                _ropeSegments[i + 1] = nextSeg;
            }
            else if (i == _ropeSegments.Count - 2) // The rope head and end also have to connect, can refine the rope constraints
            {
                currentSeg.CurrentPosition -= changeVector;
                _ropeSegments[i] = currentSeg;

            }
            else //
            {
                currentSeg.CurrentPosition -= (changeVector * 0.5f);
                _ropeSegments[i] = currentSeg;

                nextSeg.CurrentPosition += (changeVector * 0.5f);
                _ropeSegments[i + 1] = nextSeg;
            }

            
            
        }

        // Constraints the last point position
        RopeSegment lastSegment = _ropeSegments[_ropeSegments.Count - 1];
        lastSegment.CurrentPosition = ropeEndPoint;

        _ropeSegments[_ropeSegments.Count - 1] = lastSegment;

        

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
                        // fallback method
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

    public struct RopeSegment //
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
