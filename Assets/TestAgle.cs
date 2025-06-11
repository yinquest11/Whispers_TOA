using UnityEngine;

public class TestAgle : MonoBehaviour
{
    public Transform target;
    private void Start()
    {
        
    }

    void Update()
    {
        Vector3 relativePos = target.position - transform.position;

        // the second argument, upwards, defaults to Vector3.up
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, relativePos);
        transform.rotation = rotation;
    }

    //  Quaternion.LookRotation
}
