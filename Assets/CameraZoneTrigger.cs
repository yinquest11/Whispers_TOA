using UnityEngine;

public class CameraZoneTrigger : MonoBehaviour
{
    public CameraHelper cameraHelper;
    
    [Header("Camera Behavior")]
    public CameraZoneType zoneType;
    
    [Header("Zoom Settings")]
    [Tooltip("Target camera size (only for Zoom In/Out)")]
    public float targetCameraSize = 5f;
    public float zoomDuration = 1f;
    
    [Header("Lock Camera Settings")]
    [Tooltip("Lock camera at this position (only for Lock Camera)")]
    public Transform lockPosition;
    public bool lockX = true;
    public bool lockY = true;
    
    [Header("Follow Settings")]
    [Tooltip("Dead zone size for follow behavior")]
    public Vector2 followDeadZoneSize = new Vector2(0.3f, 0.3f);
    
    [Header("Hard Limits (Optional)")]
    public bool useHardLimits = false;
    public Vector2 hardLimitsSize = new Vector2(1f, 1f);
    public Vector2 hardLimitsOffset = Vector2.zero;
    
    // Store original values
    private Vector2 originalDeadZoneSize;
    private float originalCameraSize;
    private Transform originalFollowTarget;
    private bool hasTriggered = false;
    
    void Start()
    {
        // Save original settings
        if (cameraHelper != null)
        {
            //originalDeadZoneSize = cameraHelper.cameraPositionComposer.Composition.DeadZone.Size;
            //originalCameraSize = cameraHelper.cinemachineCamera.Lens.OrthographicSize;
            //originalFollowTarget = cameraHelper.cinemachineCamera.Follow;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && cameraHelper != null && !hasTriggered)
        {
            hasTriggered = true;
            ApplyCameraEffect();
        }
    }

    void ApplyCameraEffect()
    {
        switch (zoneType)
        {
            case CameraZoneType.ZoomIn:
                ZoomIn();
                break;
                
            case CameraZoneType.ZoomOut:
                ZoomOut();
                break;
                
            case CameraZoneType.LockCamera:
                LockCamera();
                break;
                
            case CameraZoneType.FollowPlayer:
                FollowPlayer();
                break;
                
            case CameraZoneType.StopFollowing:
                StopFollowing();
                break;
        }
        
        // Apply hard limits if enabled
        if (useHardLimits)
        {
            cameraHelper.SetHardLimitsSize(hardLimitsSize);
            cameraHelper.SetHardLimitsOffset(hardLimitsOffset);
        }
    }

    void ZoomIn()
    {
        float currentSize = cameraHelper.cinemachineCamera.Lens.OrthographicSize;
        cameraHelper.SetCameraSize(currentSize, targetCameraSize, zoomDuration);
        Debug.Log($"Camera zooming in to size: {targetCameraSize}");
    }

    void ZoomOut()
    {
        float currentSize = cameraHelper.cinemachineCamera.Lens.OrthographicSize;
        cameraHelper.SetCameraSize(currentSize, targetCameraSize, zoomDuration);
        Debug.Log($"Camera zooming out to size: {targetCameraSize}");
    }

    void LockCamera()
    {
        if (lockPosition != null)
        {
            // Make camera follow the lock position instead of player
            cameraHelper.cinemachineCamera.Follow = lockPosition;
            // Set dead zone to zero so camera stays exactly at lock position
            cameraHelper.SetDeadZoneSize(Vector2.zero);
            Debug.Log($"Camera locked at position: {lockPosition.position}");
        }
        else
        {
            Debug.LogWarning("Lock Position not set! Please assign a Transform.");
        }
    }

    void FollowPlayer()
    {
        // Make sure camera follows player again (in case it was locked)
        if (originalFollowTarget != null)
        {
            cameraHelper.cinemachineCamera.Follow = originalFollowTarget;
        }
        
        // Set custom dead zone size
        cameraHelper.SetDeadZoneSize(followDeadZoneSize);
        Debug.Log($"Camera following player with dead zone: {followDeadZoneSize}");
    }

    void StopFollowing()
    {
        // Set dead zone to full screen so camera doesn't move
        cameraHelper.SetDeadZoneSize(new Vector2(10f, 10f));
        Debug.Log("Camera stopped following player");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && cameraHelper != null && hasTriggered)
        {
            hasTriggered = false;
            // Optional: Reset to original settings when player leaves
            // Uncomment if you want this behavior:
            // ResetCamera();
        }
    }

    void ResetCamera()
    {
        // Restore original settings
        cameraHelper.SetDeadZoneSize(originalDeadZoneSize);
        cameraHelper.SetCameraSize(cameraHelper.cinemachineCamera.Lens.OrthographicSize, 
                                   originalCameraSize, zoomDuration);
        
        if (originalFollowTarget != null)
        {
            cameraHelper.cinemachineCamera.Follow = originalFollowTarget;
        }
    }

    // Draw gizmos to visualize trigger area
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Gizmos.DrawCube(transform.position + (Vector3)box.offset, box.size);
        }
    }
}

public enum CameraZoneType
{
    ZoomIn,
    ZoomOut,
    LockCamera,
    FollowPlayer,
    StopFollowing
}
