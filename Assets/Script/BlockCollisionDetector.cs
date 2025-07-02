using UnityEngine;

public class BlockCollisionDetector : MonoBehaviour
{

    private RopeController ropeController;

    
    private bool _isCollision;

    void Start()
    {
        ropeController = GameObject.FindWithTag("RopeController").GetComponent<RopeController>();
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (ropeController.isThrowing == true)
        {
            ropeController.isThrowing = false;
            ropeController.targetRigidbody.linearVelocity = Vector2.zero;
        }

        //foreach (ContactPoint2D contact in collision.contacts)
        //{ 
        //    if(Vector2.Angle(contact.normal, Vector2.up) < 25f)
        //    {
        //        Debug.DrawRay(contact.point, contact.normal, Color.yellow, 5f);
        //    }
        //}

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
        
    }

    
}
