using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{

    public float maxHealth = 10f;
    public float currentHealth = 10f;
    
    
    public GameObject spawnWhenDead;
    protected SpriteRenderer childSprite;
    protected Coroutine _delayDieCoroutine;
    protected Coroutine _changeColorCoroutine;
    protected Color _originalColor;

    [Header("For Light Enemy erhh")] // Only for light enemy might required this
    public GameObject erhhGameObjectPrefab;
    protected GameObject _erhhGameObject;
    public float erhhForce = 18.02f;



    public enum gameObjectType
    {
        LightEnemy,
        HeavyEnemy,
        Player
    }

    public gameObjectType type;

    protected virtual void Start()
    {
        childSprite = GetComponentInChildren<SpriteRenderer>();
        _originalColor = childSprite.color;

        Initialization();

    }
    protected virtual void Update() // some child object might inhereit this update 
    {
       

    }

    protected void Initialization() // initialize the current health
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float _damageAmountTake) // First take damage function, will instantly die if health < 0
    {
        if (this.enabled == false)
        {
            return;
        }

        currentHealth -= _damageAmountTake;

        TakeDamageRespond();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            HaveToDie();
        }

    }

    public virtual void TakeDamage(float _damageAmountTake, float delayDie) // Second take damage function, will delay {delayDie} second only die if health < 0
    {
        if (this.enabled == false)
        {
            Debug.Log($"{gameObject.name} not found");
            return;
        }


        currentHealth -= _damageAmountTake;

        TakeDamageRespond();

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            _delayDieCoroutine = StartCoroutine(DelayDieCoroutine(delayDie));

        }

    }

    protected virtual void TakeDamageRespond() // the respond if take damage, normaly just change color
    {
        switch (type)
        {
            case gameObjectType.LightEnemy:
                _changeColorCoroutine = StartCoroutine(ChangeColorCoroutine());
                break;

            case gameObjectType.HeavyEnemy:
                _changeColorCoroutine = StartCoroutine(ChangeColorCoroutine());
                break;

            case gameObjectType.Player:
                Debug.Log("PLayerHealthMinus");
                break;
        }
    }



    public virtual void HaveToDie() // Destroy them self with different way, can play particular animation
    {

        switch (type)
        {
            case gameObjectType.LightEnemy:
                StopCoroutine(_changeColorCoroutine);

                Destroy(this.gameObject);
                break;

            case gameObjectType.HeavyEnemy:
                StopCoroutine(_changeColorCoroutine);

                Destroy(this.gameObject);
                break;

            case gameObjectType.Player:
                Debug.Log("PLayerShouldDie");
                break;
        }
        

        if (spawnWhenDead != null)
        {
            Instantiate(spawnWhenDead, transform.position, transform.rotation);
        }
        
        
    }

    public virtual void LightEnemyErhhhh(Vector2 erhhDirection) // instantiae the "coin" (if for light enemy)
    {
        if (type != gameObjectType.LightEnemy)
            return;

        if(erhhGameObjectPrefab != null)
        {
            
            _erhhGameObject = Instantiate(erhhGameObjectPrefab, (Vector2)transform.position, transform.rotation);
            
            _erhhGameObject.GetComponent<Rigidbody2D>().AddForce(erhhDirection * erhhForce, ForceMode2D.Impulse);

            Destroy(_erhhGameObject, 2f);

        }

    }

    public virtual IEnumerator DelayDieCoroutine(float delay) // Delay Die Coroutine for second take damage
    {

        yield return new WaitForSeconds(delay);

        HaveToDie();

    }

    protected virtual IEnumerator ChangeColorCoroutine() // Change the color of sprite when is an Enemy are taking damage
    {
        if (childSprite == null) { Debug.Log(gameObject.name + " has activate defensive programming"); }

        childSprite.color = Color.white; //childSprite.color = new Color(0.9433962f, 0.5844309f, 0.5844309f);

        yield return new WaitForSeconds(0.1f);

        childSprite.color = _originalColor;

    }
}
