using UnityEngine;

public class Attack : MonoBehaviour
{

    private Animator _animator;
    public bool isMeleeAttack;
    public float attackSpeed = 1; // 3 is maximum
    
    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        if (_animator == null){ Debug.Log(gameObject.name + " has activate defensive programming"); return; }
    }

    private void Update()
    {
        SetAnimation();
        DoAttack();
        
    }

    // Change to attack punya animation
    private void DoAttack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            
            _animator.SetTrigger("meleeAttack");
            isMeleeAttack = true;
        }
    }

    

    // Set the attack aniamtion clip in child punya animator
    private void SetAnimation()
    {
        _animator.SetBool("isMeleeAttack", isMeleeAttack);
        _animator.SetFloat("attackSpeed", attackSpeed);
        
    }

    
}
