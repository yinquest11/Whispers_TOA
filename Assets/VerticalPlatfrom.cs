using UnityEngine;

public class VerticalPlatfrom : MonoBehaviour
{
    PlatformEffector2D _effector;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _effector = GetComponent<PlatformEffector2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
