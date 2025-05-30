using UnityEngine;

public class FairyFollow : MonoBehaviour
{
    private Transform _playerTransform;
    public Vector2 offset = new Vector2(-1.5f, 1.5f);
    public float followSpeed = 3f;
    public float catchFollowToLocationSpeed = 3f;
    

    public float floatAmplitude = 0.25f;
    public float floatFrequency = 2f;
    public float orbitRadius = 0.2f;
    public float orbitSpeed = 2f;

    private Vector3 _velocity = Vector3.zero;

    public bool isCatchByEnemy = false;

    private Transform _catchToLocation;

    private void Start()
    {
        _playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        _catchToLocation = GameObject.FindWithTag("CatchToLocation").GetComponent<Transform>();
    }

    void Update()
    {
        if (isCatchByEnemy == false)
        {
            if (_playerTransform == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }
            FairyFollowPlayer();
        }
        else
        {
            if (_catchToLocation == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }
            CatchByEnemyLiao();
        }
      
    }

    public void FairyFollowPlayer()
    {
        Vector3 targetPosition = _playerTransform.position + (Vector3)offset;

        targetPosition.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, 1f / followSpeed);

        Vector3 orbitOffset = new Vector3
        (
            Mathf.Cos(Time.time * orbitSpeed) * orbitRadius,
            Mathf.Sin(Time.time * orbitSpeed) * orbitRadius,
            0
        );


        Vector3 baseTarget = _playerTransform.position + (Vector3)offset + orbitOffset;
        baseTarget.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        transform.position = Vector3.SmoothDamp(transform.position, baseTarget, ref _velocity, 1f / followSpeed);
    }

    public void CatchByEnemyLiao()
    {
        Vector3 targetPosition = _catchToLocation.position + (Vector3)offset;

        targetPosition.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, catchFollowToLocationSpeed);

        Vector3 orbitOffset = new Vector3
        (
            Mathf.Cos(Time.time * orbitSpeed) * orbitRadius,
            Mathf.Sin(Time.time * orbitSpeed) * orbitRadius,
            0
        );


        Vector3 baseTarget = _catchToLocation.position + (Vector3)offset + orbitOffset;
        baseTarget.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        transform.position = Vector3.SmoothDamp(transform.position, baseTarget, ref _velocity, catchFollowToLocationSpeed);
    }

}