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
        if(Input.GetKeyDown(KeyCode.E))
        {
            // change size
            ChangeCameraSize(cinemachineCamera.Lens.OrthographicSize, cinemachineCamera.Lens.OrthographicSize + 2, 2);
        }
        
        if(Input.GetKeyDown(KeyCode.Q))
        {
            // enable full screen dead zone
            DeadZoneEnable(new Vector2(1f, 1f));
        }
        
        if(Input.GetKeyDown(KeyCode.Z))
        {
            // enable empty dead zone area, so no death zone
            DeadZoneEnable(new Vector2(0f, 0f));
        }
        
        
    }

    public void DeadZoneEnable(Vector2 customSize)
    {

        cameraPositionComposer.Composition.DeadZone.Size = customSize;
        
    }
    public void ChangeCameraSize(float startSize, float endSize,float duration, AnimationCurve curve = null)
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
