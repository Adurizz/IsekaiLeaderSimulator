using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Companion : Hero
{
    [SerializeField] protected int level;
    [SerializeField] protected List<int> skillLevels = new() { 0, 0, 0 };
    protected NavMeshAgent navMeshAgent;
    protected Animator animator;
    protected LayerMask enemyLayer;
    protected const int skillNum = 3;

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    public int GetLevel()
    {
        return level;
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

    public EHeroClass GetClass()
    {
        return heroClass;
    }

    public List<int> GetSkillLevels()
    {
        return skillLevels;
    }

    public string GetName()
    {
        return actorName;
    }

    public void SetName(string name)
    {
        actorName = name;
        this.name = name;
    }

    protected abstract void Move();
    protected abstract void Attack();
}
