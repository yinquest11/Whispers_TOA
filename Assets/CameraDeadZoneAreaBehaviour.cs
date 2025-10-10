using Unity.VisualScripting;
using UnityEngine;

public class CameraDeadZoneAreaBehaviour : MonoBehaviour
{
    private Collider2D myCollider;
    private CameraHelper cameraHelper;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myCollider = gameObject.GetComponent<Collider2D>();
        cameraHelper = GameObject.FindWithTag("CameraHelper").GetComponent<CameraHelper>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
