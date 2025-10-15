using UnityEngine;

public class CameraStartBehaviour : MonoBehaviour
{
    public CameraHelper cameraHelper;
    
    [Header("Starting Camera Settings")]
    public bool lockCameraAtStart = true;
    
    [Tooltip("Position where camera should be locked")]
    public Transform lockPosition;
    
    [Header("Or use Dead Zone Lock")]
    public bool useDeadZoneLock = false;
    public Vector2 largeDeadZoneSize = new Vector2(10f, 10f);
    
    void Start()
    {
        if (cameraHelper == null)
        {
            Debug.LogError("CameraHelper not assigned!");
            return;
        }
        
        if (lockCameraAtStart)
        {
            if (lockPosition != null)
            {
                // Method 1: Lock to specific position
                cameraHelper.cinemachineCamera.Follow = lockPosition;
                cameraHelper.SetDeadZoneSize(Vector2.zero);
                Debug.Log("Camera locked at start position");
            }
            else if (useDeadZoneLock)
            {
                // Method 2: Use huge dead zone so camera doesn't move
                cameraHelper.SetDeadZoneSize(largeDeadZoneSize);
                Debug.Log("Camera locked using large dead zone");
            }
            else
            {
                Debug.LogWarning("No lock method configured!");
            }
        }
    }
    
    // Call this function when you want to unlock camera (from trigger or event)
    public void UnlockCamera(Vector2 normalDeadZoneSize)
    {
        // Restore normal follow behavior
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            cameraHelper.cinemachineCamera.Follow = player.transform;
            cameraHelper.SetDeadZoneSize(normalDeadZoneSize);
            Debug.Log("Camera unlocked - now following player");
        }
    }
}
