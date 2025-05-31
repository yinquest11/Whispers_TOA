using Unity.VisualScripting;
using UnityEngine;

public class CatchFairy : MonoBehaviour
{
    private CatcherHealth _catcherHealth;

    private FairyFollow _fairyFollow;

    private Transform _emptyFollowFairyTransform;

    private GameObject[] _allCatchers;

    //private bool _allisDead = false;

    

    public bool HasCatchFiary
    {
       
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

        _emptyFollowFairyTransform = GameObject.FindWithTag("EmptyFollowFairy").gameObject.transform;


        _catcherHealth = GetComponent<CatcherHealth>();
        if (_catcherHealth == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }

    }

    // Update is called once per frame
    void Update()
    {
         

        

    }

    

    private void OnTriggerEnter2D(Collider2D _fairyCollider)
    {
        if(_fairyCollider.gameObject.CompareTag("Fairy") == false)
        {
            return;
        }

        if (_catcherHealth.isDead == true)
        {
            return;
        }

        transform.SetParent(_emptyFollowFairyTransform);


        HasCatchFiary = true;




    }

    private void OnDestroy()
    {


        

    }
}
