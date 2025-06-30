using UnityEngine;
using System.Collections;

public class RopeController : MonoBehaviour
{
    // 名字最后改

    [HideInInspector] public Camera m_camera;
    [HideInInspector] public Tutorial_GrapplingRope grappleRope;

    [HideInInspector] public Vector2 grapplePoint;
    [HideInInspector] public Vector2 grappleDistanceVector;

    public LineRenderer lineRenderer;

    [Header("Layers Settings:")]
    public LayerMask interstedLayer;

    [Header("Distance:")]
    public float maxDistnace = 20;

    [Header("Holding Status")]
    public bool gotHold = false;

    [Header("Physics Ref:")]
    public SpringJoint2D m_springJoint2D;
    public Rigidbody2D m_rigidbody;

    public DistanceJoint2D m_distanceJoint2D;
    public Rigidbody2D throwPivotRigidbody;

    public PlayerController m_playerController;
    

    // Hit information

    RaycastHit2D _hitTarget;
    int targetLayer;
    string targetLayerName;
    bool holdBefore = false;

    public enum RopeMode
    {
        Swing,
        PullEnemyToMe,
        ThrowEnemy,
        Nothing 
    }

    public RopeMode ropeMode;

    public enum SwingType
    {
        Physic,
        Transform
    }

    public SwingType swingType;

    // Move TestAddForce punya variable
    Rigidbody2D targetRigidbody;
    private ViewportRuler _viewportRuler;
    public GameObject throwRope;
    private float targetX;
    private float myX;
    Vector2 _playerDirectionToBlock;
    Vector2 _blockPrependicularDirectionToPlayer;
    [HideInInspector] public bool canThrow = true; // cooldown
    private Coroutine _coroutine;
    private Coroutine _coroutine2;
    public float throwCooldown = 0.03f;
    public bool isThrowing = false;
    public float forceAmount = 111;
    [HideInInspector] public Vector2 forceDirection;

    // Use movement helper to pull enemy to me  
    public AnimationCurve acceleration;
    public MovementHelper movementHelper;
    public Vector3 target;
    public float moveSpeed;
    public Vector2 pullDestination; // Setting in inspector
    public SpriteRenderer spriteRenderer;

    



    void Start()
    {
        m_camera = Camera.main;
        grappleRope = GetComponent<Tutorial_GrapplingRope>();
        grappleRope.enabled = false;
        ropeMode = RopeMode.Nothing;

        m_springJoint2D = transform.parent.GetComponent<SpringJoint2D>();
        m_springJoint2D.enabled = false;

        m_rigidbody = transform.parent.GetComponent<Rigidbody2D>();
        m_playerController = transform.parent.GetComponent<PlayerController>();

        throwPivotRigidbody = GetComponentInChildren<Rigidbody2D>();

        m_distanceJoint2D = GetComponentInChildren<DistanceJoint2D>();

        m_distanceJoint2D.enabled = false;

        lineRenderer = GetComponentInChildren<LineRenderer>();
    }

    
    void Update()
    {
        Debug.DrawRay(Vector2.zero, PullOffSet(), Color.cyan);
        

        if (Input.GetKeyDown(KeyCode.Mouse1) == true)
        {
            SetGrapplePoint();
        }

        if (_hitTarget == false)
            return;

        UpdateGotHold();

        TryDetermineRopeMode();

        switch (ropeMode)
        {
            case RopeMode.Swing:
                RopeSwing();
                break;

            case RopeMode.PullEnemyToMe:
                PullEnemy();
                break;

            case RopeMode.ThrowEnemy:

                RopeThrow();
                break;

            case RopeMode.Nothing: // break all rope connection
                InitializePlayer();
                break;
        }

    }


    void PullEnemy()
    {
        movementHelper.MoveToBySpeed(_hitTarget.collider.transform, PullOffSet(), moveSpeed, acceleration);

        // special case, have to close your self
        grappleRope.enabled = false;
        ropeMode = RopeMode.Nothing;
    }

    Vector2 PullOffSet()
    {
        pullDestination.x = spriteRenderer.flipX ? -Mathf.Abs(pullDestination.x) : Mathf.Abs(pullDestination.x);

        Vector2 _pullDestination = (Vector2)transform.position + pullDestination;

        return _pullDestination;
    }
    
    private void RopeSwing()
    {
        if (swingType == SwingType.Physic)
        {
            m_springJoint2D.connectedAnchor = grapplePoint;
            m_springJoint2D.distance = 0.5f;
            m_playerController.enabled = false;
            m_springJoint2D.enabled = true;
        }
        else if (swingType == SwingType.Transform)
        {
           
        }
    }

    void RopeThrow()
    {
        
        lineRenderer.enabled = false; // close own line renderer

        // get hit target punya rigid body
        if (_hitTarget.collider.GetComponent<Rigidbody2D>() != null) 
        {
            targetRigidbody = _hitTarget.collider.GetComponent<Rigidbody2D>();
        }
        else
        {
            Debug.Log("Enemy no have rigid body ?");
        }

        // setting distance joint
        m_distanceJoint2D.enabled = true;
        m_distanceJoint2D.autoConfigureConnectedAnchor = false;
        m_distanceJoint2D.connectedBody = targetRigidbody;

        // setting target rigid body
        targetRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        targetRigidbody.freezeRotation = true;
        targetRigidbody.gravityScale = 5f;
        targetRigidbody.mass = 2f;
        targetRigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;

        // add a tempChild to the target
        if (targetRigidbody.transform.Find("tempChild") == null)
        {
            GameObject child = new GameObject("tempChild");
            child.transform.SetParent(targetRigidbody.gameObject.transform, false);

            Rigidbody2D rb = child.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.bodyType = RigidbodyType2D.Kinematic;

        }

        //  add a box collider detedtor to him
        if (targetRigidbody.gameObject.GetComponent<BlockCollisionDetector>() == false)
        {
            targetRigidbody.gameObject.AddComponent<BlockCollisionDetector>();
        }

        // enable my verlet rope
        throwRope.SetActive(true);

        if (targetRigidbody.transform.Find("tempChild") != null)
        {
            throwRope.GetComponent<RopeVerlet>().boxTransform = targetRigidbody.transform.Find("tempChild");
        }
        else
        {
            Debug.Log("Enemy temp child transform cant find");
        }


        // update the x coordinates
        targetX = _hitTarget.collider.transform.position.x;
        myX = transform.position.x;

        // calculate perpendicular
        _playerDirectionToBlock = (_hitTarget.collider.transform.position - gameObject.transform.position).normalized;
        _blockPrependicularDirectionToPlayer = Vector2.Perpendicular(_playerDirectionToBlock);

        // check if need to throw, then throw
        if (_viewportRuler.HasDirectionReversed == true)
        {

            if (canThrow == true) 
            {
                Throw(_viewportRuler.GetMouseMoveDirection);
            }

        }

        // check if need to acceleration the box
        if (isThrowing == true)
        {
            if (targetRigidbody.linearVelocity.y < 0 && m_distanceJoint2D.enabled == true)
            {

                targetRigidbody.linearVelocity *= new Vector2(1.05f, 1.2f);
            }
        }
    }


    public void Throw(int i)
    {
        canThrow = false;
        _coroutine = StartCoroutine(ThrowCooldown());
        isThrowing = true;
        forceDirection = i == 1 ? -_blockPrependicularDirectionToPlayer : _blockPrependicularDirectionToPlayer;
        targetRigidbody.linearVelocity = Vector2.zero;
        targetRigidbody.AddForce(forceDirection * forceAmount, ForceMode2D.Impulse);
    }

    private IEnumerator ThrowCooldown()
    {
        yield return new WaitForSeconds(throwCooldown);
        canThrow = true;
    }

    private void Awake()
    {
        _viewportRuler = GameObject.FindWithTag("ViewportRuler").GetComponent<ViewportRuler>();
        
    }

    private void InitializePlayer()
    {
        

        if (m_playerController.enabled == false)
        {
            m_playerController.enabled = true;
        }

        m_springJoint2D.enabled = false;
        m_distanceJoint2D.enabled = false;
        m_rigidbody.gravityScale = 5; // the original Gravity Scale of player

        

        if (targetRigidbody != null)
        {
            
            targetRigidbody.gravityScale = 2f;
        }

        if (throwRope != null)
        {
            throwRope.SetActive(false);
        }
        
        if(_hitTarget == true)
        {
            if (_hitTarget.collider.gameObject.GetComponent<BlockCollisionDetector>() == true)
            {
                Destroy(_hitTarget.collider.gameObject.GetComponent<BlockCollisionDetector>());
            }
        }

    }

    private void TryDetermineRopeMode()
    {
        
            

        if (grappleRope.daZhong == true && gotHold == true && grappleRope.enabled == true)
        {
            holdBefore = true;

            // Hold a grabable
            if (targetLayerName == "Grabable")
            {
                
                ropeMode = RopeMode.Swing;
            }
            // Hold a enemy
            else if (targetLayerName == "Enemy")
            {
                if (_hitTarget.collider.CompareTag("LightEnemy"))
                {
                    
                    ropeMode = RopeMode.ThrowEnemy;
                }
                else if (_hitTarget.collider.CompareTag("HeavyEnemy"))
                {
                    
                    ropeMode = RopeMode.Swing;
                }

            }



        }

        // Press
        else if (holdBefore == false && gotHold == false && targetLayerName == "Enemy" && _hitTarget.collider.CompareTag("LightEnemy") && grappleRope.enabled == true)
        {
            // pull back
            
            ropeMode = RopeMode.PullEnemyToMe;

            
        }
        else if (holdBefore == false && gotHold == false && grappleRope.enabled == true)
        {


            if (_hitTarget.collider != null)
            {


                if (targetLayerName == "Grabable")
                {
                    
                    grappleRope.enabled = false;

                    ropeMode = RopeMode.Nothing;
                }
                else if (_hitTarget.collider.CompareTag("HeavyEnemy"))
                {
                    
                    grappleRope.enabled = false;
                    ropeMode = RopeMode.Nothing;
                }
            }

        }
        else if (holdBefore == true && gotHold == false && grappleRope.enabled == true)
        {
            holdBefore = false;
            grappleRope.enabled = false;

            ropeMode = RopeMode.Nothing;
        }
    }

    private void UpdateGotHold()
    {
        if (gotHold == true)
        {
            if (Input.GetKeyUp(KeyCode.Mouse1) == true)
            {
                gotHold = false;
                
            }
            
        }
    }

    void SetGrapplePoint()
    {
        Vector2 fireDirection = (m_camera.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized; // 

        if (Physics2D.Raycast(transform.position, fireDirection) == true)
        {
            _hitTarget = Physics2D.Raycast(transform.position, fireDirection, maxDistnace, interstedLayer);



            if (_hitTarget.collider != null)  // if got hit any thing base on max distance and intesdtedLayer when Raycast
            {

                

                gotHold = true;

                AnalysisTarget(_hitTarget.collider.gameObject); // the target layer will only include in interstedLayer, if not even pass the Raycast test

                grapplePoint = _hitTarget.point; // record the hit point

                grappleDistanceVector = grapplePoint - (Vector2)transform.position; // record the length from hit point to this

                grappleRope.enabled = true; // enable the draw rope, when this script is enable, will enable the line renderer 

            }
        }
    }

    void AnalysisTarget(GameObject target)
    {
        targetLayer = target.layer;
        targetLayerName = LayerMask.LayerToName(targetLayer);

        

        switch (targetLayerName)
        {
            case "Enemy":
                //Debug.Log($"Target is on Enemy layer")
                return;

            case "Grabable":
                //Debug.Log($"Target is on Grabable layer")
                return;
        }

    }

    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxDistnace);

        

    }

    
}
