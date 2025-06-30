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
        if (myDistanceJoint.enabled == false)
            return;

        if (myDistanceJoint.connectedBody == null)
        {
            myDistanceJoint.enabled = false;
            myRope.gameObject.SetActive(false);
            return;
        }
            
        

        // Get all hit collider
        bool flowControl = GetSmartTagCollider();

        

        // if raycant does not hit any thing, return
        if (!flowControl)
        {
            return;
        }

        UpdateAnchorBaseOnRopeMode();
    }
    // 。。。。。。
    private void UpdateAnchorBaseOnRopeMode()
    {
        
        if (myRope.changeToMultiRope == true && _lastSmartAnchorObject_Tag != null)
        {

            // multi rope

            _boxWorldToLocal = transform.InverseTransformPoint(myDistanceJoint.connectedBody.transform.position);

            if (_nextIsFirstEnterMulti == true)
            {
                _myInitialDistanceToFirstPoint = Vector2.Distance(myRope.firstCollisionPoint, transform.position);
                _baseJointDistance = Vector2.Distance(myRope.lastCollisionPoint, myDistanceJoint.connectedBody.transform.position);
                myDistanceJoint.distance = Vector2.Distance(transform.InverseTransformPoint(myRope.lastCollisionPoint), _boxWorldToLocal);

                _nextIsFirstEnterMulti = false;
            }

            float currentPlayerToFirstAnchorDist = Vector2.Distance(myRope.firstCollisionPoint, transform.position);

            float distanceDelta = _myInitialDistanceToFirstPoint - currentPlayerToFirstAnchorDist;

            myDistanceJoint.distance = _baseJointDistance + distanceDelta;


            myDistanceJoint.anchor = transform.InverseTransformPoint(_lastSmartAnchorObject_Tag.ReturnClosestQPoint(myRope.lastCollisionPoint));

            _myWalkDistance = _myInitialDistanceToFirstPoint - Vector2.Distance(myRope.firstCollisionPoint, transform.position);

            myDistanceJoint.distance += _myWalkDistance * 1.2f;

            //myDistanceJoint.maxDistanceOnly = false;
            myDistanceJoint.autoConfigureConnectedAnchor = false;


        }
        else
        {
            // normal
            myDistanceJoint.anchor = Vector2.zero;
            myDistanceJoint.distance = jointDistance;
            //myDistanceJoint.maxDistanceOnly = true;
            myDistanceJoint.autoConfigureConnectedAnchor = false;

            _nextIsFirstEnterMulti = true;
            // other smart feature, eg: dynamic enemy punya rigidbody
        }
    }

    private bool GetSmartTagCollider()
    {
        
            

        RaycastHit2D[] raycastHit2Ds = Physics2D.RaycastAll
                                               (
                                               transform.position, // origin
                                               (myDistanceJoint.connectedBody.transform.position - transform.position).normalized, // direction
                                               Vector2.Distance(transform.position, myDistanceJoint.connectedBody.transform.position) // Length
                                               );


        if (raycastHit2Ds.Length == 0)
        {
            Debug.Log("raycastHit2Ds does not hit any thing");
            return false;
        }

        // _lastSmartAnchorObject_Tag equal the longest distance collider to player that got SmartAnchorObject_Tag
        _lastSmartAnchorObject_Tag = raycastHit2Ds.Where(hit => hit.collider.GetComponent<SmartAnchorObject_Tag>() != null).OrderByDescending(hit => hit.distance).FirstOrDefault() is RaycastHit2D furthestHit && furthestHit.collider != null ?
                                     furthestHit.collider.GetComponent<SmartAnchorObject_Tag>() :
                                     null;

        // if i got a collider thats got smart tag, its smart component
        if (_lastSmartAnchorObject_Tag != null)
        {
            //Debug.Log("HI");
            
        }

        return true;
    }

}
