using UnityEngine;

public class Attack : MonoBehaviour
{

    private Animator _animator;
    public bool isMeleeAttack;

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

    private void DoAttack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            
            _animator.SetTrigger("meleeAttack");
            isMeleeAttack = true;
        }
    }

    


    private void SetAnimation()
    {
        _animator.SetBool("isMeleeAttack", isMeleeAttack);
        
    }

    
}
