using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class TestAddForceToBlock : MonoBehaviour
{
    public Rigidbody2D block;
    public Collider2D blockCollider;
    private DistanceJoint2D _distanceJoint;
    private ViewportRuler _viewportRuler;

    public GameObject _rope;

    public float forceAmount = 200f;
    [HideInInspector] public Vector2 forceDirection;

    Vector2 _playerDirectionToBlock;
    Vector2 _blockPrependicularDirectionToPlayer;

    private bool isReverse = false;
    [HideInInspector] public bool blockIsCollisioned = false;

    [HideInInspector] public float blockX;
    [HideInInspector] public float myX;

    [HideInInspector] public bool throwOut = false;

    [HideInInspector] public bool canThrow = true;
    public float throwCooldown = 0.03f;

    private Coroutine _coroutine;

    [HideInInspector] public bool GotCatch = false;

    


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

        CheckGorCatch();



        if (GotCatch == false)
            return;

        

        UpdateX();

        CalculatePrependicular();

        CheckBlockIsCollisioned();

        SwingToThrow();

        ReleaseBlock();

        AccelerateBlockWhenDown();



    }

    private void SwingToThrow()
    {
        if (_viewportRuler.HasDirectionReversed == true)
        {
            //if (canThrow)
            //{
            //    Throw();
            //}

            if (_viewportRuler.GetMouseMoveDirection == 1 && blockX < myX ) // 向右甩，且方块在左边
            {
                Throw();
            }
            else if (_viewportRuler.GetMouseMoveDirection == -1 && blockX > myX ) // 向左甩，且方块在右边
            {
                Throw();
            }

        }
    }

    private void CheckGorCatch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            GotCatch = true;
            throwOut = false;
            _distanceJoint.enabled = true;
            _rope.SetActive(true);
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

    private void AccelerateBlockWhenDown()
    {
        if (block.linearVelocityY < 0 && blockIsCollisioned == false && throwOut == false)
        {
            block.linearVelocity *= 1.1f;
        }
    }

    private void ReleaseBlock()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            _distanceJoint.enabled = !_distanceJoint.enabled;
            block.gravityScale = 0f;
            _rope.SetActive(false);
            throwOut = true;
            GotCatch = false;


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
