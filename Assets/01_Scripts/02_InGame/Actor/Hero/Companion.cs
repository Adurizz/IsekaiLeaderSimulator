using UnityEngine;
using UnityEngine.AI;

public abstract class Companion : Hero
{
    [SerializeField] protected int level;
    protected NavMeshAgent navMeshAgent;
    protected Animator animator;
    protected LayerMask enemyLayer;

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    public void LevelUp()
    {
        if (level < GlobalValueHolder.maxCompanionLevel)
        {
            level++;
            UpdateStat();
        }
    }

    protected void UpdateStat()
    {
        // TODO: 레벨업에 따른 스탯 업데이트
    }

    public string GetName()
    {
        return actorName;
    }

    protected abstract void Move();
    protected abstract void Attack();
}
