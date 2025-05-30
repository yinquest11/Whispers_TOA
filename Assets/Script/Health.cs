using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{

    public float maxHealth = 10f;
    public float currentHealth = 10f;
    public GameObject takeDamageSound;
    public GameObject spawnWhenDead;
    private SpriteRenderer childSprite;
    private Coroutine _changeColorCoroutine;

    public bool gameObjectIsPlayer = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        childSprite = GetComponentInChildren<SpriteRenderer>();

        Initialization();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Initialization()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float _damageAmountTake)
    {
        

        if (takeDamageSound != null)
        {
            GameObject.Instantiate(takeDamageSound, transform.position, transform.rotation);
        }
        

        currentHealth -= _damageAmountTake;
        Debug.Log("Curretn Health: " + currentHealth);

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

            Debug.Log(gameObject.name + " die");

            HaveToDie();


        }




    }

    IEnumerator _changeColor()
    {
        childSprite.color = new Color(0.9433962f, 0.5844309f, 0.5844309f);

        yield return new WaitForSeconds(0.1f);

        childSprite.color = Color.white;

    }

    public void HaveToDie()
    {
        if(gameObjectIsPlayer == false)
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
