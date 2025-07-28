using System.Collections;
using UnityEngine;

public class SkeletonBoss : Enemy
{
    [SerializeField] private ParticleSystem attackRangeEffect;
    [SerializeField] private bool isAttacking;
    [SerializeField] TrailRenderer weaponTrailRenderer;

    public bool IsAttacking
    {
        get { return isAttacking; }
        set 
        { 
            isAttacking = value;
            weaponTrailRenderer.enabled = IsAttacking;
            animator.SetBool("isAttacking", isAttacking);
            if (isAttacking == true)
                Invoke(nameof(DeactivateAttackState), 2.5f);
        }
    }    

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif

    protected override void FSM()
    {
        if (!IsAttacking && Target == null)
        {
            CurState = EEnemyState.Idle;
        }

        switch (CurState)
        {
            case EEnemyState.Idle:
                if (Target != null)
                {
                    navMeshAgent.isStopped = false;
                    CurState = EEnemyState.Chasing;
                }
                break;
            case EEnemyState.Chasing:
                #region 범위 내에 들어오면 공격으로 전환
                if (distanceFromTarget <= attackDistance)
                {
                    curDetectionCool = 0;
                    curAttackCool = 0;
                    CurState = EEnemyState.Attack;
                    attackInit = true;
                }
                break;
            #endregion
            case EEnemyState.Attack:
                RotateTowardsTarget();
                #region 첫 공격 하고 쿨타임 시작
                if (attackInit)
                {
                    Debug.Log("Init Attack");
                    PerformAttack();
                    curAttackCool = 0;
                    attackInit = false;
                    navMeshAgent.isStopped = true;
                    return;
                }
                #endregion

                #region 공격중에는 더 짧은 빈도로 적 탐색, 범위 벗어나면 공격 캔슬
                // 적 탐지
                curDetectionCool += Time.deltaTime;
                if (curDetectionCool >= maxDetectionCoolWhileAttack)
                {
                    SetTarget();
                    curDetectionCool = 0;
                }

                if (distanceFromTarget > attackDistance)
                {
                    navMeshAgent.isStopped = false;
                    CurState = EEnemyState.Chasing;
                    return;
                }
                #endregion
                // 공격
                curAttackCool += Time.deltaTime;
                if (curAttackCool >= attackSpeed)
                {
                    PerformAttack();
                    curAttackCool = 0;
                }
                break;
            case EEnemyState.Hit:
                curDelay += Time.deltaTime;
                if (curDelay >= hitDelay)
                {
                    if (Target != null)
                    {
                        navMeshAgent.isStopped = false;
                        CurState = EEnemyState.Chasing;
                    }
                    else
                        CurState = EEnemyState.Idle;
                }
                break;
            case EEnemyState.Dead:
                // Debug.Log("Dead State");
                break;
        }
    }

    protected override void PerformAttack()
    {
        
    }

    protected override void Move()
    {
        if (Target == null || curState != EEnemyState.Chasing || IsAttacking)
        {
            return;
        }
        navMeshAgent.SetDestination(target.position);
    }

    public void ActivateAreaEffect()
    {
        attackRangeEffect.Play();
        animator.SetTrigger("Attack");
        IsAttacking = true;
        navMeshAgent.isStopped = true;
    }

    public void EndAttack()
    {
        /*
        IsAttacking = false;
        navMeshAgent.isStopped = false;
        */
    }

    public void DeactivateAttackState()
    {
        IsAttacking = false;
        navMeshAgent.isStopped = false;
    }

    public void MakeAreaAttack()
    {
        ApplyAreaDamage(transform.position, attackRange);
    }

    public void ApplyAreaDamage(Vector3 position, float range, float damageMultiplier = 1f)
    {
        Collider[] colliders = Physics.OverlapSphere(position, range, heroLayer);

        foreach (Collider col in colliders)
        {
            if (col.gameObject.GetComponent<Actor>().IsDead)
                continue;

            col.gameObject.GetComponent<Actor>().GetDamage(attack * damageMultiplier);
        }
    }
}
