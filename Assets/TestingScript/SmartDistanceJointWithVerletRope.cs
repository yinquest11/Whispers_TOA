using UnityEngine;

public class SmartDistanceJointWithVerletRope : MonoBehaviour
{
    public RopeVerlet myRope;

    public DistanceJoint2D myDistanceJoint;

    
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

    // Update is called once per frame
    void FixedUpdate()
    {


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

            myDistanceJoint.anchor = transform.InverseTransformPoint(myRope.lastCollisionPoint);

            
            

            _myWalkDistance = _myInitialDistanceToFirstPoint - Vector2.Distance(myRope.firstCollisionPoint, transform.position);

            myDistanceJoint.distance += _myWalkDistance;


            myDistanceJoint.maxDistanceOnly = true;
            myDistanceJoint.autoConfigureConnectedAnchor = false;


        }
    }

    
}
