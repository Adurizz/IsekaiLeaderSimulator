using UnityEngine;

public class SkeletonSoldier : Enemy
{
    private float curAttackCool = 0;

    public void OnEnable()
    {
        InitStat();
    }

    protected override void Update()
    {
        base.Update();
        if (distanceFromTarget < attackDistance)
        {
            Attack();
            AttackWithCool();
        }
        else
        {
            if (curAttackCool != 0)
                curAttackCool = 0;
        }
    }

    private void AttackWithCool()
    {
        curAttackCool += Time.deltaTime;
        if (curAttackCool >= attackSpeed)
        {
            Attack();
            curAttackCool = 0;
        }
    }

    protected override void Attack()
    {
        animator.SetTrigger("Attack");
    }
}
