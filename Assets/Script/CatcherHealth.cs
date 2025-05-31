using UnityEngine;

public class CatcherHealth : Health
{

    protected Spawner _spawner;

    private Transform _playerTransform;

    private FairyFollow _fairyFollow;

    private CatchFairy _catchFairy;

    public CatcherHealth[] _catcherHealths;

    public bool isDead = false;

    private bool _allisDead = false;

    private CatchersEnemyMovement _catchersEnemyMovement;

    public bool HasCatchFiary
    {

        set
        {
            if (_fairyFollow != null)
            {
                _fairyFollow.isCatchByEnemy = value;

            }

        }
    }

    private float _withPlayerDistance()
    {
        return Vector2.Distance(transform.position, _playerTransform.position);
    }

    protected override void Start()
    {
        _playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        _catchersEnemyMovement = GetComponent<CatchersEnemyMovement>();
        _catchFairy = GetComponent<CatchFairy>();
        _fairyFollow = GameObject.FindWithTag("Fairy").GetComponent<FairyFollow>();
        base.Start();

        if (_playerTransform == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }
        if (_fairyFollow == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }
    }

    protected override void Update()
    {
        
        base.Update();
        if(_allisDead == true)
        {
            
            HasCatchFiary = false;
        }

        if (_withPlayerDistance() > 20 && _catchersEnemyMovement.stopChasingBefore == true) 
        {
             base.HaveToDie();
        }
    }

    public override void TakeDamage(float _damageAmountTake)
    {
        
    }

    public override void HaveToDie()
    {
        // catcher HaveToDie() will set the _isDead to true first, CatchersEnemyMovement will use the Dead series funcion, after 
        // _withExitPointDistance < 2 and Catcher catch before(if not might immidiately die), use original HaveToDie() in parrent class

        transform.SetParent(null);
        

        if (_catchFairy == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }

        isDead = true;

        _catcherHealths = Object.FindObjectsByType<CatcherHealth>(FindObjectsSortMode.None);

        foreach(CatcherHealth _catcherHealth in _catcherHealths)
        {
            _allisDead = true;

            if (_catcherHealth.isDead == false)
            {
                _allisDead = false;
                break;
            }
        }



    }






}
