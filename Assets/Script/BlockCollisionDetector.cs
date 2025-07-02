using UnityEngine;

public class BlockCollisionDetector : MonoBehaviour
{

    private RopeController ropeController;
    private Health targetHealth;

    
    

    void Start()
    {
        ropeController = GameObject.FindWithTag("RopeController").GetComponent<RopeController>();
        targetHealth = GetComponent<Health>();
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (ropeController.isThrowing == true)
        {
            ropeController.MaxDistanceOnly(true);
            ropeController.isThrowing = false;
            ropeController.targetRigidbody.linearVelocity = Vector2.zero;

            if (targetHealth != null)
            {
                AudioManager.Instance.PlaySfx("ThrowHit");
                //targetHealth.LightEnemyErhhhh(Vector2.up);
                targetHealth.TakeDamage(0.01f,1f);
            }
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
