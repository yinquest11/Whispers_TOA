using UnityEngine;

public class BlockCollisionDetector : MonoBehaviour
{

    private RopeController ropeController;

    public bool GetCollisionState
    {
        get
        {
            return _isCollision;
        }
    }

    private bool _isCollision;

    void Start()
    {
        ropeController = GameObject.FindWithTag("RopeController").GetComponent<RopeController>();
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _isCollision = true;
        ropeController.isThrowing = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _isCollision = false;
        
    }

    
}
