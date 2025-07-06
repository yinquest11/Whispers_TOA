using UnityEngine;

public class IgnoreColisionBetween : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("erhhObject"), LayerMask.NameToLayer("Enemy"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("erhhObject"), LayerMask.NameToLayer("erhhObject"), true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
