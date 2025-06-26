using UnityEngine;

public class BlockCollisionDetector : MonoBehaviour
{

    private TestAddForceToBlock _playerAddForceScript;

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
        _playerAddForceScript = GameObject.FindWithTag("Player").GetComponent<TestAddForceToBlock>();
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        _isCollision = true;
        _playerAddForceScript.isThrowing = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _isCollision = false;
        
    }

    
}
