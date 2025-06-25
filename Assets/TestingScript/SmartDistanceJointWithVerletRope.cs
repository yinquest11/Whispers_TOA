using System.Linq;
using UnityEngine;

public class SmartDistanceJointWithVerletRope : MonoBehaviour
{
    public RopeVerlet myRope;

    public DistanceJoint2D myDistanceJoint;

    private SmartAnchorObject_Tag _lastSmartAnchorObject_Tag;


    Vector2 _boxWorldToLocal;

    float _myWalkDistance;
    float _myInitialDistanceToFirstPoint;
    float _baseJointDistance;
    bool _nextIsFirstEnterMulti;

    [Range(1,10)] public float jointDistance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

   

    
    void Update()
    {
        RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll(transform.position, myDistanceJoint.connectedBody.transform.position - transform.position.normalized);

        if (raycastHit2Ds.Length == 0)
            return;
        

        RaycastHit2D furthestHit = raycastHit2Ds.Where(hit => hit.collider.GetComponent<SmartAnchorObject_Tag>() != null).OrderByDescending(hit => hit.distance).FirstOrDefault();
        _lastSmartAnchorObject_Tag = furthestHit.collider.GetComponent<SmartAnchorObject_Tag>();

        if (furthestHit.collider != null)
        {
            Debug.Log(furthestHit.collider.name);

        }

        if (myRope.changeToMultiRope == false)
        {
            // normal
            myDistanceJoint.anchor = Vector2.zero;
            myDistanceJoint.distance = jointDistance;
            myDistanceJoint.maxDistanceOnly = true;
            myDistanceJoint.autoConfigureConnectedAnchor = false;

            _nextIsFirstEnterMulti = true;
            // other smart feature, eg: dynamic enemy punya rigidbody

        }
        else 
        {
            
            // multi rope
            
            _boxWorldToLocal = transform.InverseTransformPoint(myDistanceJoint.connectedBody.transform.position);

            if(_nextIsFirstEnterMulti == true)
            {
                _myInitialDistanceToFirstPoint = Vector2.Distance(myRope.firstCollisionPoint, transform.position);
                _baseJointDistance = Vector2.Distance(myRope.lastCollisionPoint, myDistanceJoint.connectedBody.transform.position);
                myDistanceJoint.distance = Vector2.Distance(transform.InverseTransformPoint(myRope.lastCollisionPoint), _boxWorldToLocal);

                _nextIsFirstEnterMulti = false;
            }

            float currentPlayerToFirstAnchorDist = Vector2.Distance(myRope.firstCollisionPoint, transform.position);

            float distanceDelta = _myInitialDistanceToFirstPoint - currentPlayerToFirstAnchorDist;

            myDistanceJoint.distance = _baseJointDistance + distanceDelta;

            
            myDistanceJoint.anchor = transform.InverseTransformPoint((2 * myRope.lastCollisionPoint - (Vector2)myDistanceJoint.connectedBody.transform.position)  +myRope.lastPointEscapeDirection);

            _myWalkDistance = _myInitialDistanceToFirstPoint - Vector2.Distance(myRope.firstCollisionPoint, transform.position);

            myDistanceJoint.distance += _myWalkDistance * 1.2f;

            myDistanceJoint.maxDistanceOnly = true;
            myDistanceJoint.autoConfigureConnectedAnchor = false;


        }
    }

    
}
