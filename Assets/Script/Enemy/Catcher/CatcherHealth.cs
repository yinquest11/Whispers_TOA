using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

public class CatcherHealth : Health // Inhereit from Health, can make some modify
{

    protected Spawner _spawner;

    private Transform _playerTransform;

    private FairyFollow _fairyFollow;

    private CatchFairy _catchFairy;

    public CatcherHealth[] _catcherHealths;

    public bool isDead = false;

    private bool _allisDead = false;

    private CatchersEnemyMovement _catchersEnemyMovement;

    public bool wantToTakeDamegeByPlayer = false; // Can control want to get hit by Player?

    public bool HasCatchFiary // Set the bolean "isCatchByEnemy" in FairyFollow, when i set HasCatchFiary, directly effect isCatchByEnemy
    {

        set
        {
            if (_fairyFollow != null)
            {
                _fairyFollow.isCatchByEnemy = value;

            }
            else
            {
                if (_fairyFollow == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }
            }

        }
    }

    private float _withPlayerDistance()// Calculate the distance self(Cactcher) with the Player
    {
        if (_playerTransform == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return 0; }
        return Vector2.Distance(transform.position, _playerTransform.position);
    }

    protected override void Start()
    {
        _playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        _catchersEnemyMovement = GetComponent<CatchersEnemyMovement>();
        _catchFairy = GetComponent<CatchFairy>();
        _fairyFollow = GameObject.FindWithTag("Fairy").GetComponent<FairyFollow>();

        base.Start();

        
        if (_catchersEnemyMovement == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }
        
        
    }

    protected override void Update()
    {
        
        base.Update();

        if(_allisDead == true)
        {
            
            HasCatchFiary = false; // Let the fairy know she is not been catching 
        }

        if (_withPlayerDistance() > 20 && _catchersEnemyMovement.stopChasingBefore == true) 
        {
             base.HaveToDie();
        }
    }

    public override void TakeDamage(float _damageAmountTake)
    {
        if (wantToTakeDamegeByPlayer == true)
        {
            base.TakeDamage(_damageAmountTake);
        }
        else if (wantToTakeDamegeByPlayer == false)
        {

        }
        
    }

    public override void HaveToDie()// Modify HaveToDie function in Health
    {
        

        transform.SetParent(null); // Set the parent null from EmptyFollowFairy when should die
        

        if (_catchFairy == null) { Debug.Log(gameObject.name + " has activate defensive programming"); return; }

        isDead = true;

        // Get all CatcherHealth component in the game
        _catcherHealths = Object.FindObjectsByType<CatcherHealth>(FindObjectsSortMode.None);

        // if got one does not dead, _allIsDead is false, else true
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
