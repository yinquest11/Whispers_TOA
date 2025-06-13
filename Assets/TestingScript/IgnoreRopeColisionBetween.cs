using UnityEngine;

public class IgnoreRopeColisionBetween : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Rope"), LayerMask.NameToLayer("Rope"), true);
        Debug.Log("HI" + gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
