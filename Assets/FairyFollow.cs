using UnityEngine;

public class FairyFollow : MonoBehaviour
{
    public Transform player;             
    public Vector2 offset = new Vector2(-1.5f, 1.5f); 
    public float followSpeed = 3f;       
    public float floatAmplitude = 0.25f; 
    public float floatFrequency = 2f;    
    public float orbitRadius = 0.2f;
    public float orbitSpeed = 2f;

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        Vector3 targetPosition = player.position + (Vector3)offset;

        targetPosition.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / followSpeed);

        Vector3 orbitOffset = new Vector3(
        Mathf.Cos(Time.time * orbitSpeed) * orbitRadius,
        Mathf.Sin(Time.time * orbitSpeed) * orbitRadius,
        0
        );

        Vector3 baseTarget = player.position + (Vector3)offset + orbitOffset;
        baseTarget.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = Vector3.SmoothDamp(transform.position, baseTarget, ref velocity, 1f / followSpeed);
    }
}

