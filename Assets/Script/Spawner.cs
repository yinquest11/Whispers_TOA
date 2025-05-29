using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool startSpawn = false;

    private Vector3 _cameraLeftBottom;
    private Vector3 _cameraRightTop;
    private Camera _mainCamera;

    public float spawnInterval;
    private float _spawnTimer;

    private int _randomSideNumber;
    private Vector3 _spawnPosition;

    public GameObject[] enemyArray;
    private int _currentChance;




    void Start()
    {
        _mainCamera = Camera.main;

        spawnInterval = 0.68f;
        _spawnTimer = spawnInterval;

    }

    
    void Update()
    {
        if(startSpawn == true)
        {
            _cameraLeftBottom = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, _mainCamera.nearClipPlane));
            _cameraRightTop = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, _mainCamera.nearClipPlane));

            if (_spawnTimer > 0)
            {
                _spawnTimer -= Time.deltaTime;
                return;
            }

            _randomSideNumber = Random.Range(0, 4);

            switch (_randomSideNumber)
            {
                case 0: // Spawn on top

                    _spawnPosition = new Vector3(Random.Range(_cameraLeftBottom.x, _cameraRightTop.x), _cameraRightTop.y + 1, _mainCamera.nearClipPlane);
                    break;

                case 1: // Spawn on bottom
                    _spawnPosition = new Vector3(Random.Range(_cameraLeftBottom.x, _cameraRightTop.x), _cameraLeftBottom.y - 1, _mainCamera.nearClipPlane);
                    break;

                case 2: // Spawn on left
                    _spawnPosition = new Vector3(_cameraLeftBottom.x - 1, Random.Range(_cameraLeftBottom.y, _cameraRightTop.y), _mainCamera.nearClipPlane);
                    break;

                case 3:// Spawn on right
                    _spawnPosition = new Vector3(_cameraRightTop.x + 1, Random.Range(_cameraLeftBottom.y, _cameraRightTop.y), _mainCamera.nearClipPlane);
                    break;
            }

            _currentChance = Random.Range(0, enemyArray.Length);

            Instantiate(enemyArray[_currentChance], _spawnPosition, transform.rotation);

            _spawnTimer = spawnInterval;
        }
    }
}
