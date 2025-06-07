using UnityEngine;

public class Fortesting : MonoBehaviour
{
    // test header
    [Header("Setting:")]
    [SerializeField] private float _speed;
    [Range(0, 20)] [SerializeField] private float _high;

    // test animation curve
    [Range(0,10)] [SerializeField] private float _timeValueX;
    [SerializeField] private AnimationCurve _testCurve;

    [Header("Should hide:")]
    // test hide in inspector
    [HideInInspector] public float speed;
    [HideInInspector] private float high;
    [HideInInspector][SerializeField] private float timeValue;
    [SerializeField][HideInInspector] private float timeValuee;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        Debug.Log("Enable"+Time.time);
    }

    private void OnDisable()
    {
        Debug.Log("Disable" + Time.time);
    }
}
