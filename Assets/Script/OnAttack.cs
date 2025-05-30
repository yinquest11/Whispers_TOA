using Unity.VisualScripting;
using UnityEngine;

public class OnAttack : MonoBehaviour
{
    public float attackDamage;
    public Vector2 attackSize = new Vector2(1f, 1f);
    private Vector2 _attackAreaPosition;
    public float offSetX = 1f;
    public float offSetY = 1f;
    public Transform parrentTransform;
    public LayerMask enemyLayer;

    public void Awake()
    {
        parrentTransform = transform.parent;
    }

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnAttacking(float _attackNum)
    {

        _attackAreaPosition = transform.position;

        offSetX = parrentTransform.localScale.x < 0 ? -Mathf.Abs(offSetX) : Mathf.Abs(offSetX);

        _attackAreaPosition.x += offSetX;
        _attackAreaPosition.y += offSetY;




        Collider2D[] _hitColliders = Physics2D.OverlapBoxAll(_attackAreaPosition, attackSize, 0f, enemyLayer);

        foreach(Collider2D _hitCollider in _hitColliders)
        {
            
            _hitCollider.gameObject.GetComponent<Health>().TakeDamage(_attackNum);
            
            // find the collider that collide with us then use Enemy punya TakeDamage function
            // hitCollider.GetComponent<Enemy>().TakeDamage(attackDamage * isAttack);
            // need to assign enemy to enemyLayer
        }

        
    }

     

    private void OnDrawGizmosSelected()
    {
        _attackAreaPosition = transform.position;
        _attackAreaPosition.x += offSetX;
        _attackAreaPosition.y += offSetY;
        

        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_attackAreaPosition, attackSize);
    }

    
}
