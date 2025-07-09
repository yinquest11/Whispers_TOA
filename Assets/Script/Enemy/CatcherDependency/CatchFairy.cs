using Unity.VisualScripting;
using UnityEngine;

public class CatchFairy : MonoBehaviour
{
    private CatcherHealth _catcherHealth;

    private FairyFollow _fairyFollow;

    private Transform _emptyFollowFairyTransform;

    private GameObject[] _allCatchers;

    public bool HasCatchFiary // Set the bolean "isCatchByEnemy" in FairyFollow, when i set HasCatchFiary, directly effect isCatchByEnemy
    {
       
        set
        {
            if (_fairyFollow != null)
            {
                _fairyFollow.isCatchByEnemy = value;

            }
            else
            {
                if (_fairyFollow == null) { Debug.Log(gameObject.name + " has activate defensive programming");}
            }

        }
    }
    
    void Start()
    {
        _fairyFollow = GameObject.FindWithTag("Fairy").GetComponent<FairyFollow>();
        _emptyFollowFairyTransform = GameObject.FindWithTag("EmptyFollowFairy").gameObject.transform;
        _catcherHealth = GetComponent<CatcherHealth>();
    }

    private void OnTriggerEnter2D(Collider2D _fairyCollider) // When self OnTriggerEnetr with fairy
    {
        if (_emptyFollowFairyTransform == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }

        if(_fairyCollider.gameObject.CompareTag("Fairy") == false)
        {
            return;
        }

        if (_catcherHealth.isDead == true)
        {
            return;
        }

        if (_emptyFollowFairyTransform == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }

        transform.SetParent(_emptyFollowFairyTransform);// Set self as a child with an empty object that following fairy, so can
                                                        // remain the relative position of each catcher when OnTriggerEnetr with fairy


        HasCatchFiary = true; // Let the fairy know she is been catching

    }

}
