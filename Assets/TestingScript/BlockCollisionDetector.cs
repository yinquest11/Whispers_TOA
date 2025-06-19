using UnityEngine;

public class BlockCollisionDetector : MonoBehaviour
{

    private TestAddForceToBlock _playerAddForceScript;

    
    void Start()
    {
        _playerAddForceScript = GameObject.FindWithTag("Player").GetComponent<TestAddForceToBlock>();
        
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{

    //    _playerAddForceScript.blockIsGround = true;


    //}

    //void OnCollisionStay2D()
    //{
    //    _playerAddForceScript.blockIsGround = true;
    //}


}
