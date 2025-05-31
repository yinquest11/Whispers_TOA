using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{

    public float maxHealth = 10f;
    public float currentHealth = 10f;
    public GameObject takeDamageSound;
    public GameObject spawnWhenDead;
    protected SpriteRenderer childSprite;
    protected Coroutine _changeColorCoroutine;

    public bool gameObjectIsPlayer = false;

    





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        childSprite = GetComponentInChildren<SpriteRenderer>();

        Initialization();
    }

    // Update is called once per frame
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

        if (takeDamageSound != null)
        {
            GameObject.Instantiate(takeDamageSound, transform.position, transform.rotation);
        }
        

        currentHealth -= _damageAmountTake;
        

        // Different Action if the gameObject is Enemy or Player
        if (gameObjectIsPlayer == false)
        {
            _changeColorCoroutine = StartCoroutine(_changeColor());
        }
        else if (gameObjectIsPlayer == true)
        {
            // Change player health bar
            Debug.Log("PLayerHealthMinus");
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;

            

            HaveToDie();


        }




    }

    protected  virtual IEnumerator _changeColor()
    {
        childSprite.color = new Color(0.9433962f, 0.5844309f, 0.5844309f);

        yield return new WaitForSeconds(0.1f);

        childSprite.color = Color.white;

    }

    public virtual void HaveToDie()
    {


        if (gameObjectIsPlayer == false && _changeColorCoroutine != null) 
        {
            StopCoroutine(_changeColorCoroutine);
        }

        if (spawnWhenDead != null)
        {
            Instantiate(spawnWhenDead, transform.position, transform.rotation);
        }
        
        Destroy(this.gameObject);
    }
}
