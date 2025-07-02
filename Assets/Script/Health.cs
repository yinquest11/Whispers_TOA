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

    [Header("For Light Enemy erhh")]
    public GameObject erhhGameObjectPrefab;
    protected GameObject _erhhGameObject;
    public float erhhForce;



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

    protected virtual void Update()
    {
        
    }

    protected void Initialization()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float _damageAmountTake)
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

    public virtual void TakeDamage(float _damageAmountTake, float delayDie)
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

    protected virtual void TakeDamageRespond()
    {
        switch (type)
        {
            case gameObjectType.LightEnemy:
                _changeColorCoroutine = StartCoroutine(_changeColor());
                break;

            case gameObjectType.HeavyEnemy:
                _changeColorCoroutine = StartCoroutine(_changeColor());
                break;

            case gameObjectType.Player:
                Debug.Log("PLayerHealthMinus");
                break;
        }
    }

    protected virtual IEnumerator DelayDieCoroutine(float delay) 
    {

        yield return new WaitForSeconds(0.1f);
        
        HaveToDie();

    }

    protected  virtual IEnumerator _changeColor() // Change the color of sprite when is an Enemy are taking damage
    {
        if (childSprite == null) { Debug.Log(gameObject.name + " has activate defensive programming");  }

        
        childSprite.color = Color.white; //childSprite.color = new Color(0.9433962f, 0.5844309f, 0.5844309f);

        yield return new WaitForSeconds(0.1f);

        childSprite.color = _originalColor;

    }

    public virtual void HaveToDie()
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

    public virtual void LightEnemyErhhhh(Vector2 fromPlayerDirection)
    {
        if (type != gameObjectType.LightEnemy)
            return;

        if(erhhGameObjectPrefab != null)
        {
            //_erhhGameObject = Instantiate(erhhGameObjectPrefab, fromPlayerDirection + (Vector2)transform.position * 1.1f, transform.rotation);
            _erhhGameObject = Instantiate(erhhGameObjectPrefab, (Vector2)transform.position, transform.rotation);
            //_erhhGameObject.GetComponent<Rigidbody2D>().AddForce(fromPlayerDirection * erhhForce, ForceMode2D.Impulse);
            _erhhGameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * erhhForce, ForceMode2D.Impulse);

            Destroy(_erhhGameObject, 2f);

        }

        

    }
}
