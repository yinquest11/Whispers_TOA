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

    //private int _randomSideNumber;
    private Vector3 _spawnPosition;


    public GameObject[] enemyArray;

    public GameObject[] catchers;
    public int catchersAmount;

    public GameObject[] exitsCatchers;

    public int whichIndexCatchers = 0;

    private int _currentChance;

    public EnumPosition _randomSide;
    public enum EnumPosition
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

        exitsCatchers = new GameObject[catchersAmount];

    }

    
    void Update()
    {

        if (startSpawnCatchers == true)
        {
            
            SpawnCatchers();

            startSpawnCatchers = false;

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
            _spawnTimer = spawnInterval;
        }

        
    }

    public void RandomSpawnOneEnemy()
    {
        RandomChooseOneScreenEdge();
        RandomChooseOnePosition();

        _currentChance = Random.Range(0, enemyArray.Length);

        Instantiate(enemyArray[_currentChance], _spawnPosition, transform.rotation);
    }

    public void RandomChooseOneScreenEdge()
    {
        _cameraLeftBottom = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, _mainCamera.nearClipPlane));
        _cameraRightTop = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, _mainCamera.nearClipPlane));

        _randomSide = (EnumPosition)Random.Range(0, 4); // 0 top, 1 bottom, 2 left, 3 right

        
    }

    private void RandomChooseOnePosition()
    {
        switch (_randomSide)
        {
            case EnumPosition.Top:

                _spawnPosition = new Vector3(Random.Range(_cameraLeftBottom.x, _cameraRightTop.x), _cameraRightTop.y + 1, _mainCamera.nearClipPlane);
                break;

            case EnumPosition.Bottom:
                _spawnPosition = new Vector3(Random.Range(_cameraLeftBottom.x, _cameraRightTop.x), _cameraLeftBottom.y - 1, _mainCamera.nearClipPlane);
                break;

            case EnumPosition.Left:
                _spawnPosition = new Vector3(_cameraLeftBottom.x - 1, Random.Range(_cameraLeftBottom.y, _cameraRightTop.y), _mainCamera.nearClipPlane);
                break;

            case EnumPosition.Right:
                _spawnPosition = new Vector3(_cameraRightTop.x + 1, Random.Range(_cameraLeftBottom.y, _cameraRightTop.y), _mainCamera.nearClipPlane);
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
                exitsCatchers[i] = Instantiate(catchers[whichIndexCatchers], _spawnPosition, transform.rotation);
            }
            else if (_randomSide == EnumPosition.Left || _randomSide == EnumPosition.Right)
            {
                exitsCatchers[i] = Instantiate(catchers[whichIndexCatchers], _spawnPosition, transform.rotation);

            }

        }
        
    }

}
