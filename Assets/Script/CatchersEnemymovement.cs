using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CatchersEnemyMovement : MonoBehaviour
{
    protected GameObject fairy;
    private CatcherHealth _catcherHealth;

    protected Collider2D _collider;
    protected Rigidbody2D _rigidBody;

    public Vector2 inputDirection;
    public float acceleration = 5f;

    protected Vector2 m_Velocity = Vector2.zero;
    protected float m_MovementSmoothing = 0.05f;

    protected bool _isMoving = false;
    protected Vector2 _targetRotation = Vector2.zero;

    public float withFairyDistance;
    public bool wantToChase = true;
    public bool stopChasingBefore = false;

    

    

    protected Spawner _spawner;

    protected Vector3 exitLocation;
    






    void Start()
    {
        _collider = GetComponent<Collider2D>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _catcherHealth = GetComponent<CatcherHealth>();
        _spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();
        fairy = GameObject.FindWithTag("Fairy");

        // Set their place should die on the Start()
        _spawner.RandomChooseOneScreenEdge();
        _spawner.RandomChooseOnePosition();
        exitLocation = _spawner._randomSidePosition;

    }


    void FixedUpdate()
    {

        
        if (stopChasingBefore != true && wantToChase == true)// Catched successful before? if no then do the normal Movement
        {
            HandleInput();
            HandleMovement();
            HandleRotation();
        }
        else if (wantToChase == false && _catcherHealth.isDead == false)// When catchers touch fairy
        {
            _rigidBody.linearVelocity = Vector2.zero;
        }
        else if (_catcherHealth.isDead == true&& wantToChase == false) // When catchers override punya HaveToDie() is called, use the Dead sereis funcion
        {
            if (exitLocation == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }
            HandleDeadInput();
            HandleDeadMovement();
            HandleDeadRotation();
            
        }
        

    }

    public void HandleInput()
    {
        if (fairy == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }
       

        // Let enemy face to fairy direction
        inputDirection =
        new Vector2(fairy.transform.position.x - transform.position.x, fairy.transform.position.y - transform.position.y).normalized;

        Debug.DrawRay(transform.position, inputDirection, Color.yellow);
    }
    public void HandleMovement()
    {

        

        if ( _rigidBody  == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }

        if (_collider == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }

        

        Vector2 targetVelocity = Vector2.zero;

        targetVelocity = new Vector2(inputDirection.x * acceleration, inputDirection.y * acceleration);

        // use linearVelocity to let gameObject move
        // use SmoothDamp to smooth between 2 Vector2
        _rigidBody.linearVelocity = Vector2.SmoothDamp(_rigidBody.linearVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        // either (targetVelocity.x != 0) or (targetVelocity.y != 0) is true, _isMoving is true
        _isMoving = targetVelocity.x != 0 || targetVelocity.y != 0;

        _targetRotation = targetVelocity;
    }
    public virtual void HandleRotation()
    {
        

        if (inputDirection == Vector2.zero)
            return;


        float angle = (Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg) - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    
    private void OnTriggerEnter2D(Collider2D _fairyCollider)// When already OnTriggerenter2D, stop chasing fairy by modify the boolean variable
    {
        

        if (_fairyCollider.gameObject.CompareTag("Fairy") == false)
        {
            return;
        }

        wantToChase = false;
        stopChasingBefore = true;
    }

    

    // Dead series basicly just change the inputDirention to exitLocation
    public void HandleDeadInput()
    {
        

        


        // Let enemy face find fairy direction
        inputDirection = exitLocation.normalized;
        

        Debug.DrawRay(transform.position, inputDirection, Color.yellow);
    }

    public void HandleDeadMovement()
    {



        if (_rigidBody == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }

        if (_collider == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }



        Vector2 targetVelocity = Vector2.zero;

        targetVelocity = new Vector2(inputDirection.x * acceleration, inputDirection.y * acceleration);

        // use linearVelocity to let gameObject move
        // use SmoothDamp to smooth between 2 Vector2
        _rigidBody.linearVelocity = Vector2.SmoothDamp(_rigidBody.linearVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        // either (targetVelocity.x != 0) or (targetVelocity.y != 0) is true, _isMoving is true
        _isMoving = targetVelocity.x != 0 || targetVelocity.y != 0;

        _targetRotation = targetVelocity;
    }

    public virtual void HandleDeadRotation()
    {


        if (inputDirection == Vector2.zero)
            return;


        float angle = (Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg) - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
