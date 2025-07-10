using UnityEngine;
using System.Collections;

public class RopeController : MonoBehaviour
{
    
    // camera
    [HideInInspector] public Camera m_camera;

    // rope dependency
    [HideInInspector] public Tutorial_GrapplingRope grappleRope;
    public LineRenderer lineRenderer;

    [Header("Layers Settings:")]
    public LayerMask interstedLayer;

    [Header("Rope Max Distance:")]
    public float maxDistnace = 20;

    [Header("Holding Status")]
    public bool gotHold = false; // observe the hold status at inspector

    [Header("Physics Ref:")]
    public PlayerController m_playerController;
    public Rigidbody2D      m_rigidbody;

    public Rigidbody2D      throwPivotRigidbody;

    public SpringJoint2D    m_springJoint2D;
    public DistanceJoint2D  m_distanceJoint2D;
    

    // Hit information

     public  Vector2       grapplePoint;
     public  Vector2       grappleDistanceVector;
     private RaycastHit2D  _hitInformation;
     private int           _targetLayer;
     private string        _targetLayerName;
     private bool          _holdBefore = false;

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
    private ViewportRuler _viewportRuler;
    public  Rigidbody2D targetRigidbody;
    public  GameObject throwRope;
    public  bool canThrow = true; // cooldown
    public  bool isThrowing = false;
    
    public  float throwCooldown = 0.03f;
    public  float forceAmount = 111;
    public Vector2 playerDirectionToBlock;
    private Vector2 _blockPrependicularDirectionToPlayer;
    public  Vector2 forceDirection;
    private Coroutine _coroutine;
    private Coroutine _coroutine2;

    // Use movement helper to pull enemy to me  
    public AnimationCurve acceleration;
    public MovementHelper movementHelper;
    public SpriteRenderer spriteRenderer;
    public Vector3 target;
    public Vector2 pullDestination; // Setting in inspector
    public float moveSpeed;

    // Record target rigidbody properties
    private bool _reocrdPropertiesBefore;
    [SerializeField] private CollisionDetectionMode2D _targetOriginalDetectionMode;
    [SerializeField] private RigidbodyInterpolation2D _targetOriginalInterepolation;
    [SerializeField] private RigidbodyType2D _targetBpdyType;
    [SerializeField] private bool _targetOriginalFreezeRotation;
    [SerializeField] private float _targetOriginalGravity;
    [SerializeField] private float _targetOriginalMass;

    //
    public float distaceJointInitialDistance = 5f;
    public float whenThrowDistance;

    // player controller
    public PlayerController playerController;

    // light enemy movement script
    public ICanMove _enemyICanMove;

    // auto aim
    public float angle;
    public int rayCountEvenNumber;

    // for heavy enemy need to update anchor
    public Transform heavyEnemyTransform;
    public Vector3 heavyEnemyOriginalPosition;

    private void Awake()
    {
        // get my ruler for Throw()
        _viewportRuler = GameObject.FindWithTag("ViewportRuler").GetComponent<ViewportRuler>(); 
    }


    void Start()
    {
        // set my grapple rope
        grappleRope = GetComponent<Tutorial_GrapplingRope>();
        grappleRope.enabled = false;
        ropeMode = RopeMode.Nothing;

        // get main camera
        m_camera = Camera.main;

        // get dependecy form player self
        m_playerController = transform.parent.GetComponent<PlayerController>();
        m_springJoint2D = transform.parent.GetComponent<SpringJoint2D>();
        m_distanceJoint2D = GetComponentInChildren<DistanceJoint2D>();
        throwPivotRigidbody = GetComponentInChildren<Rigidbody2D>();
        m_rigidbody = transform.parent.GetComponent<Rigidbody2D>();
        lineRenderer = GetComponentInChildren<LineRenderer>();

        //  disable all the joint first
        m_springJoint2D.enabled = false;
        m_distanceJoint2D.enabled = false;
        _reocrdPropertiesBefore = false;

        

    }

    
    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.Mouse1) == true)
        {
            SetGrapplePoint();
        }


        UpdateGotHold();

       if (_hitInformation == false)
            return;

        
        TryDetermineRopeMode();

        SwitchRopeMode();

    }

    

    private void SwitchRopeMode()
    {
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
                InitializePlayerAndTarget();
                break;
        }
    }

    void PullEnemy()
    {
        movementHelper.MoveToBySpeed(_hitInformation.collider.transform, PullOffSet(), moveSpeed, acceleration); // move coroutine

        // special case, close the rope and set to Nothing yourself
        CloseGrappleRopeAndInitialize();
    }

    Vector2 PullOffSet()
    {
        pullDestination.x = spriteRenderer.flipX ? -Mathf.Abs(pullDestination.x) : Mathf.Abs(pullDestination.x);

        Vector2 _pullDestination = (Vector2)transform.position + pullDestination;

        return _pullDestination;
    }
    
    private void RopeSwing() // this need continuos update anchor
    {
        if (heavyEnemyTransform == null)
        {
            heavyEnemyTransform = _hitInformation.collider.transform;
            heavyEnemyOriginalPosition = heavyEnemyTransform.position;
        }
        

        if (swingType == SwingType.Physic)
        {
            m_springJoint2D.connectedAnchor = grapplePoint + ((Vector2)(heavyEnemyTransform.position - heavyEnemyOriginalPosition)); // connect to transform as point
            m_springJoint2D.distance = 0.5f; // set spring joint distance
            m_springJoint2D.enabled = true; // enable spring joint

            m_playerController.enabled = false; // disable player movement
        }
        else if (swingType == SwingType.Transform)
        {
           
        }
    }

    void RopeThrow()
    {
        
        lineRenderer.enabled = false; // close own line renderer

        // get the hit target punya rigid body
        if (_hitInformation.collider.GetComponent<Rigidbody2D>() != null)
        {
            targetRigidbody = _hitInformation.collider.GetComponent<Rigidbody2D>();
        }
        else
        {
            Debug.Log("Enemy no have rigid body ?");
        }

        // set distance joint
        m_distanceJoint2D.autoConfigureConnectedAnchor = false;
        m_distanceJoint2D.connectedBody = targetRigidbody;
        
        m_distanceJoint2D.enabled = true;

        // record the initial properties
        if (_reocrdPropertiesBefore == false)
        {        
            _targetOriginalDetectionMode = targetRigidbody.collisionDetectionMode;
            _targetOriginalInterepolation = targetRigidbody.interpolation;
            _targetBpdyType = targetRigidbody.bodyType;
            _targetOriginalFreezeRotation = targetRigidbody.freezeRotation;
            _targetOriginalGravity = targetRigidbody.gravityScale;
            _targetOriginalMass = targetRigidbody.mass;


            _reocrdPropertiesBefore = true;
        }
       
        // set target punya rigid body
        targetRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // continous collision detec
        targetRigidbody.interpolation = RigidbodyInterpolation2D.Interpolate; // interpolate mode
        targetRigidbody.bodyType = RigidbodyType2D.Dynamic;
        targetRigidbody.freezeRotation = true; // freeze rotation
        targetRigidbody.gravityScale = 5f; // set to suitable gravity for throw
        targetRigidbody.mass = 2f; // set to suitable mass  for throw



        //  add a box collider detector to him
        if (targetRigidbody.gameObject.GetComponent<BlockCollisionDetector>() == false)
        {
            targetRigidbody.gameObject.AddComponent<BlockCollisionDetector>();
        }

        // disable hit target punya ICanMove type script
        if (_hitInformation.collider.GetComponent<ICanMove>() != null)
        {
            _enemyICanMove = _hitInformation.collider.GetComponent<ICanMove>();
            ((MonoBehaviour)_enemyICanMove).enabled = false;
        }
        else
        {
            Debug.Log("Enemy Movement does not apply ICanMove interface");
        }

        // show my verlet rope
        throwRope.SetActive(true);

        
        // acess my verlet rope Game Object and access the boxTransform (Rope end position), set to target punya transform
        throwRope.GetComponent<RopeVerlet>().boxTransform = targetRigidbody.gameObject.transform;
        

        // calculate perpendicular direction
        playerDirectionToBlock = (targetRigidbody.transform.position - gameObject.transform.position).normalized;
        Debug.DrawRay(transform.position, playerDirectionToBlock, Color.red);
        _blockPrependicularDirectionToPlayer = Vector2.Perpendicular(playerDirectionToBlock);


        // check if need to throw, then throw
        if (_viewportRuler.HasDirectionReversed == true && canThrow == true ) 
        {
            Throw(_viewportRuler.GetMouseMoveDirection);        
        }

        // check if need to acceleration the box
        if (isThrowing == true && targetRigidbody.linearVelocity.y < 0 && m_distanceJoint2D.enabled == true )
        {
            targetRigidbody.linearVelocity *= new Vector2(1.05f, 1.2f);     
        }

    }


    public void Throw(int i)
    {
        canThrow = false;
        isThrowing = true;

        MaxDistanceOnly(false);

        // 1 = to right, -1 = to left
        forceDirection = i == 1 ? -_blockPrependicularDirectionToPlayer : _blockPrependicularDirectionToPlayer; // check force direction

        targetRigidbody.linearVelocity = Vector2.zero;// refresh target speed first
     
        targetRigidbody.AddForce(forceDirection * forceAmount, ForceMode2D.Impulse); // then i add force with no resistance

        _coroutine = StartCoroutine(ThrowCooldown()); // strat colddown
    }

    public void MaxDistanceOnly(bool equalTo)
    {     
        m_distanceJoint2D.maxDistanceOnly = equalTo;
    }

    private IEnumerator ThrowCooldown()
    {
        yield return new WaitForSeconds(throwCooldown);

        canThrow = true;
    }

    

    private void InitializePlayerAndTarget()
    {
        
        if (m_playerController.enabled == false) 
        {
            m_playerController.enabled = true; // enable my player movement
        }

        if (m_rigidbody != null)
        {
            m_rigidbody.gravityScale = 5; // set back the original gravity for player
        }

        if (m_springJoint2D.enabled != false)
        {
            m_springJoint2D.connectedAnchor = Vector2.zero; // initialize the anchor position

            m_springJoint2D.enabled = false; // disable spring joint
        }
          
        if(m_distanceJoint2D.enabled != false)
        {
            m_distanceJoint2D.connectedBody = null; // clean the connected body

            m_distanceJoint2D.enabled = false; // disable distance joint

            m_distanceJoint2D.maxDistanceOnly = true; // max distance only
        }

        if (heavyEnemyTransform != null)
        {
            heavyEnemyTransform = null; // clean target
        }

        if (targetRigidbody != null)
        {
            
            // set back target rigid body properties
            targetRigidbody.collisionDetectionMode = _targetOriginalDetectionMode;
            targetRigidbody.interpolation = _targetOriginalInterepolation;
            targetRigidbody.bodyType = _targetBpdyType;
            targetRigidbody.freezeRotation = _targetOriginalFreezeRotation;
            targetRigidbody.gravityScale = _targetOriginalGravity;
            targetRigidbody.mass = _targetOriginalMass;

            _reocrdPropertiesBefore = false;


            if (targetRigidbody.GetComponent<BlockCollisionDetector>() == true)
            {
                Destroy(targetRigidbody.GetComponent<BlockCollisionDetector>()); // destroy the block collision detector that i add to him
            }

            targetRigidbody = null; // clean my target rigid body is who
        }

        

        if (_enemyICanMove != null) // if im not throwing and i release a hold a light enemy, let him walk back
        {
            if(isThrowing == false)
            {
                ((MonoBehaviour)_enemyICanMove).enabled = true;
            }
            else
            {
                _hitInformation.collider.enabled = false;
                StartCoroutine(_hitInformation.collider.GetComponent<Health>().DelayDieCoroutine(5f));

                isThrowing = false; // false isThrowing my slef, because the target is been throwing out, wont colliding and false isThrowing them self
            }
            
            _enemyICanMove = null;

        }

        if (throwRope != null)
        {
            throwRope.SetActive(false); // disable my verlet rope for throw
        }

    }

    private void TryDetermineRopeMode()
    {

        if (grappleRope.daZhong == true && gotHold == true && grappleRope.enabled == true) // Hold
        {
            _holdBefore = true;

            // Hold a grabable
            if (_targetLayerName == "Grabable")
            {          
                ropeMode = RopeMode.Swing;
            }
            
            else if (_targetLayerName == "Enemy")
            {
                // Hold a light enemy
                if (_hitInformation.collider.CompareTag("LightEnemy"))
                {               
                    ropeMode = RopeMode.ThrowEnemy;
                }

                // Hold a heavy enemy
                else if (_hitInformation.collider.CompareTag("HeavyEnemy"))
                {             
                    ropeMode = RopeMode.Swing;
                }

            }

        }

        

        
        else if (_holdBefore == false && gotHold == false && _hitInformation.collider != null && grappleRope.enabled == true) // Press
        {
            // Press a grabable
            if (_targetLayerName == "Grabable")
            {
                CloseGrappleRopeAndInitialize();
            }
            else if(_targetLayerName == "Enemy")
            {
                // Press a light enemy
                if (_hitInformation.collider.CompareTag("LightEnemy"))
                {
                    ropeMode = RopeMode.PullEnemyToMe;
                }

                // Press a heavy enemy
                else if (_hitInformation.collider.CompareTag("HeavyEnemy"))
                {
                    CloseGrappleRopeAndInitialize();
                }
                
            }
            
            

        }
        
        else if (_holdBefore == true && gotHold == false && grappleRope.enabled == true) // if I release my hold
        { 
            _holdBefore = false;
            CloseGrappleRopeAndInitialize();
        }
    }

    void CloseGrappleRopeAndInitialize()
    {
        grappleRope.enabled = false;
        ropeMode = RopeMode.Nothing;
    }

    private void UpdateGotHold()
    {
        if (gotHold == true && Input.GetKeyUp(KeyCode.Mouse1) == true)
        {
            gotHold = false;
        }
    }

    
    void SetGrapplePoint()
    {
        Vector2 fireDirection = (m_camera.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized; // set fire direction

        RaycastHit2D successfulHit = Physics2D.Raycast(transform.position, fireDirection, maxDistnace, interstedLayer); // raycast to the direction

        gotHold = true;

        if (successfulHit == true) // if got hit the thing I want, use the hit point
        {
            OnGrappleHit(successfulHit);
        }
        else // no have hit any thing
        {
            RaycastHit2D autoAimHit = AutoAim(fireDirection); // try to use AutoAim() to return a cloest hit point

            if (autoAimHit == true)
            {          
                
                OnGrappleHit(autoAimHit);
            }

        }
    }

    private RaycastHit2D AutoAim(Vector2 originalDirection)
    {
        float baseAngle = Mathf.Atan2(originalDirection.y, originalDirection.x) * Mathf.Rad2Deg; // find the direction that I start with

        for (int i = 1; i <= rayCountEvenNumber; ++i)
        {
            float currentAngle = 0f;

            if (i % 2 != 0) // i is odd number
            {
                currentAngle = baseAngle + ((angle * 0.5f) / rayCountEvenNumber) * ((i * 0.5f) + 0.5f);

            }
            else if (i % 2 == 0) // i is even number
            {
                currentAngle = baseAngle - ((angle * 0.5f) / rayCountEvenNumber) * (i * 0.5f);

            }

            currentAngle *= Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));


            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, maxDistnace, interstedLayer);
            Debug.DrawRay(transform.position, direction * maxDistnace, Color.magenta, 0.5f);

            if (hit.collider != null)
            {
                return hit;
            }
        }

        return new RaycastHit2D();
    }


    private void OnGrappleHit(RaycastHit2D successfulHit)
    {
        // record the point
        _hitInformation = successfulHit; 

        // record the thing
        _targetLayer = _hitInformation.collider.gameObject.layer;
        _targetLayerName = LayerMask.LayerToName(_targetLayer);

        // record the hit point
        grapplePoint = _hitInformation.point;

        // record the length from hit point to this
        grappleDistanceVector = grapplePoint - (Vector2)transform.position;



        // enable the draw rope, when this script is enable, will enable the line renderer
        grappleRope.enabled = true;

        // enable jump
        playerController.CanJumpAgain();

    }

    

    private void OnDrawGizmosSelected()
    {
        // draw rope max distance
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxDistnace);
    }

}
