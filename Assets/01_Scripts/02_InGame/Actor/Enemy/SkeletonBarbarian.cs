using UnityEngine;

public class SkeletonBarbarian : Enemy
{
    public override void GetDamage(float damage)
    {
        Debug.Log("Enemy Hit");
        curHealth = Mathf.Clamp(curHealth - damage, 0, maxHealth);
        if (curHealth <= 0f)
        {
            // TODO: 사망 관련 처리
            OnDead();
            CurState = EEnemyState.Dead;
        }
    }
}
