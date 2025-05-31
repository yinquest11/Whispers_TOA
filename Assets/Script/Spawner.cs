using UnityEngine;
using static Spawner;

public class Spawner : MonoBehaviour
{
    public bool startSpawnFlyEnemy = false;
    public bool startSpawnCatchers = false;

    private Vector3 _cameraLeftBottom;
    private Vector3 _cameraRightTop;
    private Camera _mainCamera;

    public float spawnInterval;
    private float _spawnTimer;

    
    public Vector3 _randomSidePosition;

    public GameObject[] enemyArray;

    public GameObject[] catchers;
    public int catchersAmount;

    public int whichIndexCatchers = 0;

    private int _currentChance;

    public EnumPosition _randomSide;
    public enum EnumPosition // Use enum to represent the _randomSide
    {
        Top, // 0
        Bottom, // 1
        Left, // 2
        Right // 3
    }

    void Start()
    {
        _mainCamera = Camera.main; // Get main camera
        _spawnTimer = spawnInterval; // Initialize _spawnTimer
        

        if (_mainCamera == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }

    }

    
    void Update()
    {
        
        if (startSpawnCatchers == true)
        {
            
            SpawnCatchers();

            startSpawnCatchers = false; // Only got 1 batch of catcher is spawning when startSpawnCatchers is set to true for some reason

        }

        if (startSpawnFlyEnemy == true)
        {
            // Countdown
            if (_spawnTimer > 0)
            {
                _spawnTimer -= Time.deltaTime;
                return;
            }

            RandomSpawnOneEnemy();
            _spawnTimer = spawnInterval; // Reset the coutdown when spawn a FlyEnemy
        }

        
    }

    public void RandomSpawnOneEnemy()
    {

        RandomChooseOneScreenEdge();
        RandomChooseOnePosition();

        _currentChance = Random.Range(0, enemyArray.Length);

        Instantiate(enemyArray[_currentChance], _randomSidePosition, transform.rotation);
    }

    public void RandomChooseOneScreenEdge()
    {
        _cameraLeftBottom = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, _mainCamera.nearClipPlane));
        _cameraRightTop = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, _mainCamera.nearClipPlane));

        _randomSide = (EnumPosition)Random.Range(0, 4); // 0 top, 1 bottom, 2 left, 3 right

        
    }

    public void RandomChooseOnePosition()
    {
        switch (_randomSide)
        {
            case EnumPosition.Top:

                _randomSidePosition = new Vector3(Random.Range(_cameraLeftBottom.x, _cameraRightTop.x), _cameraRightTop.y + 1, _mainCamera.nearClipPlane);
                break;

            case EnumPosition.Bottom:
                _randomSidePosition = new Vector3(Random.Range(_cameraLeftBottom.x, _cameraRightTop.x), _cameraLeftBottom.y - 1, _mainCamera.nearClipPlane);
                break;

            case EnumPosition.Left:
                _randomSidePosition = new Vector3(_cameraLeftBottom.x - 1, Random.Range(_cameraLeftBottom.y, _cameraRightTop.y), _mainCamera.nearClipPlane);
                break;

            case EnumPosition.Right:
                _randomSidePosition = new Vector3(_cameraRightTop.x + 1, Random.Range(_cameraLeftBottom.y, _cameraRightTop.y), _mainCamera.nearClipPlane);
                break;
        }
    }

    public void SpawnCatchers()
    {
        

        for (int i = 0; i <= catchersAmount-1; ++i)
        {
            RandomChooseOneScreenEdge();
            RandomChooseOnePosition();

            if (_randomSide == EnumPosition.Top || _randomSide == EnumPosition.Bottom)
            {
                
                Instantiate(catchers[whichIndexCatchers], _randomSidePosition, transform.rotation);
            }
            else if (_randomSide == EnumPosition.Left || _randomSide == EnumPosition.Right)
            {
                
                Instantiate(catchers[whichIndexCatchers], _randomSidePosition, transform.rotation);

            }

        }
        
    }

}
