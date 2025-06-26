using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class TestAddForceToBlock : MonoBehaviour
{
    public Rigidbody2D block;
    public Collider2D blockCollider;
    public GameObject _rope;
    public float forceAmount = 2111;
    [HideInInspector] public Vector2 forceDirection;
    [HideInInspector] public bool blockIsCollisioned = false; // got collision ?
    [HideInInspector] public float blockX;
    [HideInInspector] public float myX;
    [HideInInspector] public bool canThrow = true; // cooldown
    public float throwCooldown = 0.03f;
    public bool isThrowing = false;

    private DistanceJoint2D _distanceJoint;
    private ViewportRuler _viewportRuler;
    
    private Coroutine _coroutine;
    private Coroutine _coroutine2;

    public float isThrowingDelay = 0.08f;

    Vector2 _playerDirectionToBlock;
    Vector2 _blockPrependicularDirectionToPlayer;

    private BlockCollisionDetector blockCollisionDetector;

    



    private void Start()
    {
        _distanceJoint = GetComponentInChildren<DistanceJoint2D>();
        
        block.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        blockCollisionDetector = block.GetComponent<BlockCollisionDetector>();
    }

    public void Awake()
    {
        _viewportRuler = GameObject.FindWithTag("ViewportRuler").GetComponent<ViewportRuler>();
        
    }


    void Update()
    {


        TryToCatch();
        ReleaseBlock();

        if (_distanceJoint.enabled  == false)
            return;

        UpdateX();
        CalculatePrependicular();

        //CheckBlockIsCollisioned();

        SwingToThrow();

        WantAccelerateBlockWhenDownMa();

        

        
    }

    private void SwingToThrow()
    {
        if (_viewportRuler.HasDirectionReversed == true)
        {

            if (canThrow == true) // 向右甩，且方块在左边
            {
                Throw(_viewportRuler.GetMouseMoveDirection);
            }

        }
    }

    

    private void TryToCatch()
    {
        
        _distanceJoint.enabled = true;
        _rope.SetActive(true);
        block.gravityScale = 5f;
        

    }

    private void ReleaseBlock()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            _distanceJoint.enabled = !_distanceJoint.enabled;
            // block.gravityScale = 0f;
            _rope.SetActive(false);

        }
    }

    private void UpdateX()
    {
        blockX = block.position.x;
        myX = transform.position.x;
    }

    private void CalculatePrependicular()
    {
        _playerDirectionToBlock = (block.transform.position - gameObject.transform.position).normalized;
        _blockPrependicularDirectionToPlayer = Vector2.Perpendicular(_playerDirectionToBlock);
    }

    private void WantAccelerateBlockWhenDownMa()
    {
        if(isThrowing == true)
        {
            if (block.linearVelocity.y < 0 && _distanceJoint.enabled == true)
            {
                
                block.linearVelocity *= new Vector2(1.05f, 1.2f);
            }
        }
        
    }



    public void Throw(int i)
    {

        canThrow = false;
        _coroutine = StartCoroutine(ThrowCooldown());

        isThrowing = true;
        


        forceDirection = i == 1 ? -_blockPrependicularDirectionToPlayer : _blockPrependicularDirectionToPlayer;

        block.linearVelocity = Vector2.zero;
        block.AddForce(forceDirection * forceAmount, ForceMode2D.Impulse);

        

    }


    private IEnumerator ThrowCooldown()
    {
        yield return new WaitForSeconds(throwCooldown);
        canThrow = true; 

    }

  
    private IEnumerator DelaySetIsThrowingFalse()
    {
        yield return null;
        isThrowing = false;
    }

}
