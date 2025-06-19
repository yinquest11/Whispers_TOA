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
    public Vector2 forceDirection;

    Vector2 _playerDirectionToBlock;
    Vector2 _blockPrependicularDirectionToPlayer;

    private bool isReverse = false;
    public bool blockIsGround = false;

    public float blockX;
    public float myX;

    public bool throwOut = false;

    public bool canThrow = true;
    public float throwCooldown = 0.03f;

    private Coroutine _coroutine;


    private void Start()
    {
        _distanceJoint = GetComponentInChildren<DistanceJoint2D>();
        _viewportRuler = GameObject.FindWithTag("ViewportRuler").GetComponent<ViewportRuler>();
        block.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

       
        

    }

    void Update()
    {

        UpdateX();

        CalculatePrependicular();

        CheckBlockIsGround();

        

        if(_viewportRuler.HasDirectionReversed == true)
        {
            if (canThrow)
            {
                Throw();
            }

            //int direction = _viewportRuler.GetMouseMoveDirection;

            //if (direction == 1 && canThrow) // 向右甩，且方块在左边
            //{
            //    Throw();
            //}
            //if (direction == -1 && canThrow) // 向左甩，且方块在右边
            //{
            //    Throw();
            //}

            

           
        }


        

        ReleaseBlock();

        AccelerateBlockWhenDown();


        


        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    Throw();
        //}



        



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
        if (block.linearVelocityY < 0 && blockIsGround == false && throwOut == false)
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
        }
    }

    private void CheckBlockIsGround()
    {
        if (blockCollider.IsTouchingLayers() == true)
        {
            blockIsGround = true;
        }
        else
        {
            blockIsGround = false;
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
