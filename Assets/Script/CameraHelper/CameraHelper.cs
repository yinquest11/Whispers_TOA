using Unity.Cinemachine;
using System.Collections;
using UnityEngine;
using System;

public class CameraHelper : MonoBehaviour
{
    
    public  CinemachinePositionComposer cameraPositionComposer;
    public CinemachineCamera cinemachineCamera;
    
    
    public bool isFinished { get; private set; } = true;
    public float currentValue { get; private set; }

    private Coroutine _interpolationCoroutine;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraPositionComposer = GameObject.FindWithTag("CineCamp").GetComponent<CinemachinePositionComposer>();
        cinemachineCamera  = GameObject.FindWithTag("CineCamp").GetComponent<CinemachineCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // if player reach the red area (screen space), the camera will be force to follow target until target back to the area inside
    public void SetHardLimitsSize(Vector2 customSize)
    {
        cameraPositionComposer.Composition.HardLimits.Size = customSize;
    }
    
    // Set hard limits offset
    public void SetHardLimitsOffset(Vector2 customSize)
    {
        cameraPositionComposer.Composition.HardLimits.Offset = customSize;
    }
    
    // Set dead zone size
    public void SetDeadZoneSize(Vector2 customSize)
    {

        cameraPositionComposer.Composition.DeadZone.Size = customSize;
        
    }
    
    // Change camera otho size
    public void SetCameraSize(float startSize, float endSize,float duration, AnimationCurve curve = null)
    {
        Interpolate(startSize, endSize, 2, curve, onUpdate: (newValue) => {cinemachineCamera.Lens.OrthographicSize = newValue;} );
    }
    
    // Action series is for non return value type of function
    public void Interpolate(float startValue, float endValue, float duration, AnimationCurve curve = null, Action<float> onUpdate = null)
    {
        
        if (_interpolationCoroutine != null)
        {
            StopCoroutine(_interpolationCoroutine);
        }

        // default animation curve
        if (curve == null || curve.keys.Length < 2)
        {
            
            curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        // start coroutine
        _interpolationCoroutine = StartCoroutine(InterpolationCoroutine(startValue, endValue, duration, curve, onUpdate));
    }
   
    
    private IEnumerator InterpolationCoroutine(float startValue, float endValue, float duration, AnimationCurve curve, Action<float> onUpdate)
    {
        isFinished = false;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            
            float curveProgress = curve.Evaluate(Mathf.Clamp01(elapsedTime / duration));
            
            currentValue = Mathf.LerpUnclamped(startValue, endValue, curveProgress);

            onUpdate?.Invoke(currentValue);

            // wait 1 frame
            yield return null;
        }

        
        currentValue = endValue;
        
        onUpdate?.Invoke(endValue);
        
        isFinished = true;
        
        // clean
        _interpolationCoroutine = null; 
    }

  
    public void StopInterpolation()
    {
        if (_interpolationCoroutine != null)
        {
            StopCoroutine(_interpolationCoroutine);
            isFinished = true;
            _interpolationCoroutine = null;
        }
    }
}
