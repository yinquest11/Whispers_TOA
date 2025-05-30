using UnityEngine;

public class EmptyFollowFairy : MonoBehaviour
{
    private Transform _fairyTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        _fairyTransform = GameObject.FindWithTag("Fairy").GetComponent<Transform>();
        if (_fairyTransform == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _fairyTransform.position;  
    }
}
