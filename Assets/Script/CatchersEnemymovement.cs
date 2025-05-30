using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CatchersEnemyMovement : MonoBehaviour
{
    protected GameObject fairy;

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

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _collider = GetComponent<Collider2D>();
        _rigidBody = GetComponent<Rigidbody2D>();
        

        fairy = GameObject.FindWithTag("Fairy");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        

        if (wantToChase == false) 
        {
            _rigidBody.linearVelocity = Vector2.zero;
            return;
        }

        if (stopChasingBefore != true)
        {
            HandleInput();
            HandleMovement();
            HandleRotation();
        }
        
    }

    private float GetWithFairyDistance()
    {
        return Vector2.Distance(transform.position,fairy.transform.position);
    }

    private void HandleInput()
    {
        if (fairy == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }
       

        // Let enemy face find fairy direction
        inputDirection =
        new Vector2(fairy.transform.position.x - transform.position.x, fairy.transform.position.y - transform.position.y).normalized;

        Debug.DrawRay(transform.position, inputDirection, Color.yellow);
    }

    private void HandleMovement()
    {

        

        if ( _rigidBody  == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }

        if (_collider == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }

        

        Vector2 targetVelocity = Vector2.zero;

        targetVelocity = new Vector2(inputDirection.x * acceleration, inputDirection.y * acceleration);

        //use linearVelocity to let gameObject move
        //use SmoothDamp to smooth between 2 Vector2
        _rigidBody.linearVelocity = Vector2.SmoothDamp(_rigidBody.linearVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

        //either (targetVelocity.x != 0) or (targetVelocity.y != 0) is true, _isMoving is true
        _isMoving = targetVelocity.x != 0 || targetVelocity.y != 0;

        _targetRotation = targetVelocity;
    }

    protected virtual void HandleRotation()
    {
        

        if (inputDirection == Vector2.zero)
            return;


        float angle = (Mathf.Atan2(inputDirection.y, inputDirection.x) * Mathf.Rad2Deg) - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void OnTriggerEnter2D(Collider2D _fairyCollider)
    {
        if (_fairyCollider.gameObject.CompareTag("Fairy") == false)
        {
            return;
        }

        wantToChase = false;
        stopChasingBefore = true;
    }
    private void OnTriggerExit2D(Collider2D _fairyCollider)
    {
        if (_fairyCollider.gameObject.CompareTag("Fairy") == false)
        {
            return;
        }

        wantToChase = true;
    }
}
