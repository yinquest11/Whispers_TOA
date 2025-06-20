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
    private bool isReverse = false; // change direction
    private Coroutine _coroutine;

    Vector2 _playerDirectionToBlock;
    Vector2 _blockPrependicularDirectionToPlayer;



    private void Start()
    {
        _distanceJoint = GetComponentInChildren<DistanceJoint2D>();
        
        block.collisionDetectionMode = CollisionDetectionMode2D.Continuous;


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
        CheckBlockIsCollisioned();

        UpdateThrowingState();
        

        if (isThrowing)
            WantAccelerateBlockWhenDownMa();
    
        


        SwingToThrow();

        

        

        

    }

    private void SwingToThrow()
    {
        if (_viewportRuler.HasDirectionReversed == true)
        {
            

            if (_viewportRuler.GetMouseMoveDirection == 1 && blockX <= myX ) // 向右甩，且方块在左边
            {
                Throw();
            }
            else if (_viewportRuler.GetMouseMoveDirection == -1 && blockX >= myX ) // 向左甩，且方块在右边
            {
                Throw();
            }

        }
    }

    void UpdateThrowingState()
    {
        if (blockIsCollisioned)
        {
            isThrowing = false;
        }
        else if (!blockIsCollisioned && block.linearVelocity.magnitude > 0.1f)
        {
            isThrowing = true;
        }
    }

    private void TryToCatch()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        { 
            _distanceJoint.enabled = true;
            _rope.SetActive(true);
        }

    }

    private void ReleaseBlock()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            _distanceJoint.enabled = !_distanceJoint.enabled;
            block.gravityScale = 0f;
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
        if (block.linearVelocityY < 0 && isThrowing == true && _distanceJoint.enabled == true)
        {
            
            block.linearVelocity *= 1.1f;
        }
    }

   

    private void CheckBlockIsCollisioned()
    {
        if (blockCollider.IsTouchingLayers() == true)
        {
            blockIsCollisioned = true;
        }
        else
        {
            blockIsCollisioned = false;
        }
    }

   

    public void Throw()
    {
        canThrow = false;
        

        isReverse = !isReverse;
        forceDirection = isReverse ? -_blockPrependicularDirectionToPlayer : _blockPrependicularDirectionToPlayer;
        block.linearVelocity = Vector2.zero;
        block.AddForce(forceDirection * forceAmount, ForceMode2D.Impulse);

        _coroutine = StartCoroutine(ThrowCooldown());

    }


    private IEnumerator ThrowCooldown()
    {
        yield return new WaitForSeconds(throwCooldown);
        canThrow = true;
    }

}
