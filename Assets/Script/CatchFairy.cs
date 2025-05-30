using Unity.VisualScripting;
using UnityEngine;

public class CatchFairy : MonoBehaviour
{
    
    private FairyFollow _fairyFollow;

    private Transform _emptyFollowFairyTransform;
    

    public bool HasCatchFiary
    {
        get
        {
            if (_fairyFollow == null)
            {
                return false;
            }
            else
            {
                return _fairyFollow.isCatchByEnemy;
            }
            
        }
        set
        {
            if (_fairyFollow != null)
            {
                _fairyFollow.isCatchByEnemy = value;

            }

        }
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _fairyFollow = GameObject.FindWithTag("Fairy").GetComponent<FairyFollow>();
        if (_fairyFollow == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }
        _emptyFollowFairyTransform = _fairyFollow.gameObject.transform;

    }

    // Update is called once per frame
    void Update()
    {
        

        if (HasCatchFiary == true) // All of catchers
        {
            
        }
    }

    private void OnTriggerEnter2D(Collider2D _fairyCollider)
    {
        if(_fairyCollider.gameObject.CompareTag("Fairy") == false)
        {
            return;
        }

        transform.SetParent(_emptyFollowFairyTransform);





        HasCatchFiary = true;
    }

    private void OnDestroy()
    {


        HasCatchFiary = GameObject.FindWithTag("Catcher") == null ? false : true;


    }
}
