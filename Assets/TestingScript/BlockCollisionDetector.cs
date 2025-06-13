using UnityEngine;

public class BlockCollisionDetector : MonoBehaviour
{

    private TestAddForceToBlock _playerAddForceScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerAddForceScript = GameObject.FindWithTag("Player").GetComponent<TestAddForceToBlock>();
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (_playerAddForceScript != null)
        {
            
            _playerAddForceScript.isThrowing = false;
        }
    }

    
    
}
