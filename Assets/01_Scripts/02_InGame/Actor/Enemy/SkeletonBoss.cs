using UnityEngine;

public class SkeletonBoss : Enemy
{
    [SerializeField] private ParticleSystem attackRangeEffect;
    [SerializeField] bool isAttacking;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif

    protected override void PerformAttack()
    {
        
    }

    protected override void Move()
    {
        if (Target == null || curState != EEnemyState.Chasing || isAttacking)
        {
            return;
        }
        navMeshAgent.SetDestination(target.position);
    }

    public void ActivateAreaEffect()
    {
        attackRangeEffect.Play();
        animator.SetTrigger("Attack");
        /*
        isAttacking = true;
        navMeshAgent.isStopped = true;
        */
    }

    public void EndAttack()
    {
        /*
        isAttacking = false;
        navMeshAgent.isStopped = false;
        */
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
