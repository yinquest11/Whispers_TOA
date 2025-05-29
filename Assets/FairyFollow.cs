using UnityEngine;

public class FairyFollow : MonoBehaviour
{
    public Transform player;             // Reference to the main character
    public Vector2 offset = new Vector2(-1.5f, 1.5f); // Relative position
    public float followSpeed = 3f;       // Speed of movement
    public float floatAmplitude = 0.25f; // How high it floats
    public float floatFrequency = 2f;    // Speed of floating

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        // Base position near the player with offset
        Vector3 targetPosition = player.position + (Vector3)offset;

        // Add a floating effect (sine wave)
        targetPosition.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        // Smooth follow
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / followSpeed);
    }
}

