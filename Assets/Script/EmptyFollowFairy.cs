using UnityEngine;

public class EmptyFollowFairy : MonoBehaviour
{
    // This is a component let EmptyFollowFairy GameObject transform equal to fairy transform
    private Transform _fairyTransform;

    void Start()
    {
        _fairyTransform = GameObject.FindWithTag("Fairy").GetComponent<Transform>();
        if (_fairyTransform == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }
    }

    
    void Update()
    {
        transform.position = _fairyTransform.position;  
    }
}
