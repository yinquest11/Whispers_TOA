using UnityEngine;

public class TestAddForceToBlock : MonoBehaviour
{

    public Rigidbody2D _block;
    private DistanceJoint2D _distanceJoint;

    public float forceAmount = 200f;
    public Vector2 forceDirection;

    Vector2 _playerDirectionToBlock;
    Vector2 _blockPrependicularDirectionToPlayer;

    private bool isReverse = false;

    public bool isThrowing = false;




    private void Start()
    {
        _distanceJoint = GetComponentInChildren<DistanceJoint2D>();
        _block.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }



    
    void Update()
    {
        
        

        _playerDirectionToBlock = (_block.transform.position - gameObject.transform.position).normalized;
        _blockPrependicularDirectionToPlayer =  Vector2.Perpendicular(_playerDirectionToBlock);


        if (Input.GetKeyDown(KeyCode.C))
        {
            
            isThrowing = true;

            // use isReverse to determine want to use the opposite direction or not ?
            isReverse = !isReverse;

            forceDirection = isReverse ? -_blockPrependicularDirectionToPlayer: _blockPrependicularDirectionToPlayer;



            _block.linearVelocity = Vector2.zero;
            _block.AddForce(forceDirection * forceAmount, ForceMode2D.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {

            _distanceJoint.enabled = !_distanceJoint.enabled;
            _block.gravityScale = 0;


        }


        if (_block.linearVelocityY < 0 &&   isThrowing == true)
        {
            // When dropping, your velocity increase
            // _block 的 Mass 要再2-5之间， GravityScale 5
            
            _block.linearVelocity *= 1.1f;

        }
    }
}
