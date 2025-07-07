using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    [SerializeField] protected ActorStat actorStat;
    [SerializeField] protected string actorName = "actor";
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float curHealth;
    [SerializeField] protected float attack;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float attackDistance;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected bool isDead;

    protected virtual void InitStat()
    {
        List<float> originStatList = actorStat.CopyStat();
        maxHealth = originStatList[0];
        curHealth = maxHealth;
        attack = originStatList[1];
        attackSpeed = originStatList[2];
        attackDistance = originStatList[3];
        attackRange = originStatList[4];
        moveSpeed = originStatList[5];
        isDead = false;
    }

    protected virtual void GetDamage(float damage)
    {
        curHealth = Mathf.Clamp(curHealth - damage, 0, maxHealth);
        if (curHealth <= 0f)
        {
            isDead = true;
            // TODO: 사망 관련 처리
            OnDead();
        }
    }

    protected virtual void GetHealed(float healAmount)
    {
        if (isDead)
            return;

        curHealth = Mathf.Clamp(curHealth + healAmount, 0, maxHealth);
    }

    protected virtual void OnDead()
    {

    }
}
