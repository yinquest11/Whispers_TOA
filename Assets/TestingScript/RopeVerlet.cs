
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
        for(int i = 1; i < _ropeSegments.Count; ++i) // start from 1, skip the first point
        {
            RopeSegment segment = _ropeSegments[i];

            Vector2 velocity = segment.CurrentPosition - segment.OldPosition; // 找出我上次移动的距离和方向

            // 找到 以segment.CurrentPosition为坐标，以_collisionRadius 为半径的圆圈内的所有 命中的 Collider，如果有，收在 colliders 数组里
            Collider2D[] colliders = Physics2D.OverlapCircleAll(segment.CurrentPosition, _collisionRadius, _collisionMask);

            foreach(Collider2D collider in colliders) // if the length of colliders is 0, then will not loop anything, nothing happen
            {
                Vector2 clossestPoint = collider.ClosestPoint(segment.CurrentPosition); // 找到离我最近的碰撞点（在撞到的东西上）

                float distance = Vector2.Distance(segment.CurrentPosition, clossestPoint); // 看下这个距离多少

                // if within the collision radius, resolve
                if (distance < _collisionRadius)// 如果小过我一开始判断的 _collisionRadius 大小，代表真正穿透了
                                                // 因为要避免完美接触的情况，OverlapCircleAll 会返回 <= _collisionRadius，实际上我们只要 <_collisionRadius
                                                // 通常只希望修复真正穿入（distance < radius） 的情况而非刚好接触边缘0.1000007
                {
                    Vector2 normal = (segment.CurrentPosition - clossestPoint).normalized; // 计算出 推开 或 逃离 的方向
                                                                                           // 这个方向是我远离 coolider  ClosestPoint 的方向

                    if (normal == Vector2.zero) // 如果被卡在 collider里面了
                                                // 只有当 segment.CurrentPosition - clossestPoint 等于 Vector2.zero 时，问题才会发生。
                    {
                        
                        normal = (segment.CurrentPosition - (Vector2)collider.transform.position).normalized; // 卡着的话就直接向物 Collider 的中心逃离
                    }

                    float depth = _collisionRadius - distance; // 绳子节段“钻入”碰撞体内部到底有多深

                    segment.CurrentPosition += normal * depth; // 利用上一步算出的“深度”，将绳段沿着“逃离方向”（normal）推出去，不多不少，正好推回到碰撞体的表面

                    velocity = Vector2.Reflect(velocity, normal) * _bounceFactor;   // 更新反弹之后应该有的 速度方向
                                                                                    // 我们确认了碰撞之后，才去调用 Reflect 函数。
                                                                                    // _bounceFactor 用来变大或变小整个方向的长度，看要不要弹更远





                }




            }
            // 我要骗系统，告诉它我上一帧的位置(OldPosition)其实是在 segment.CurrentPosition - velocity 这个地点。
            // 那么，在下一帧的模拟开始时，当系统自己去计算 velocity = CurrentPosition - OldPosition 时，它就会得出我们想要的那个反弹后的 移动向量。
            // 然后，系统会让我们继续沿着这个新计算出的 移动向量 的方向去移动，而这个方向，正是我们作为程序员在一开始就期望它拥有的下一帧应该有的移动方向。
            segment.OldPosition = segment.CurrentPosition-velocity;

            _ropeSegments[i] = segment; // 重新“保存”或“写回到”列表中原来的位置去

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
