using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : AttackArea
{
    private Animator animator;
    bool isAttack;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    public override void Attack()
    {
        // if(!isAttack)
        // {
        //     animator.Play("hit");
        //     isAttack = true;
        // }
        
        // attackTimer += time.deltaTime;
        // if(attackTimer >= attackInterval) {
        //     isAttack = false;
        //     attackTimer -= attackInterval;
        // }
        //animator.Play("hit");
    }

    public override void StopAttacking(bool value)
    {
        // �������汾��ʹ�ã�Ϊ����ʱͣʱֹͣ���˻ӽ�����
        //if(value == true)
        //{
        //    animator.speed = 0f;
        //}
        //else
        //{
        //    animator.speed = 1.0f;
        //}
    }
}
