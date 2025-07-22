using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public abstract class Companion : Hero
{
    [SerializeField] protected int level;
    [SerializeField] protected List<int> skillLevels = new() { 0, 0, 0 };
    [HideInInspector] public UnityEvent<List<int>> levelUpEvent = new();
    protected NavMeshAgent navMeshAgent;
    protected float stoppingDistance;
    [SerializeField] protected bool isStopped;
    public bool IsStopped
    {
        get { return isStopped; }
        set
        {
            isStopped = value;
            navMeshAgent.isStopped = isStopped;
            animator.SetBool("isStopped", isStopped);
        }
    }
    protected CompanionStatManager companionStatManager;
    protected PartyManager partyManager;
    protected Animator animator;
    protected LayerMask enemyLayer;
    protected const int skillNum = 3;

    protected virtual void Awake()
    {
        companionStatManager = FindAnyObjectByType<CompanionStatManager>();
        partyManager = FindAnyObjectByType<PartyManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    public override void InitStat()
    {
        actorStat = companionStatManager.GetCompanionStats(heroClass)[level];
        base.InitStat();
        navMeshAgent.speed = moveSpeed;
        stoppingDistance = navMeshAgent.stoppingDistance;
    }

    public int GetLevel()
    {
        return level;
    }

    public void LevelUp(int skillIndex)
    {
        if (level < GlobalValueHolder.maxCompanionLevel)
        {
            if (skillLevels[skillIndex] < 3)
                skillLevels[skillIndex]++;

            level++;
            UpdateStat();

            levelUpEvent.Invoke(skillLevels);
        }
    }

    protected void UpdateStat()
    {
        // 레벨업에 따른 스탯 업데이트
        InitStat();
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

    public override void OnDead()
    {
        isDead = true;
        IsStopped = true;
        partyManager.UnregisterPartyMember(this);
        Invoke(nameof(VanishBody), 3f);
    }

    private void VanishBody()
    {
        gameObject.SetActive(false);
    }

    public override void UpgradeMoveSpeed(float amount)
    {
        base.UpgradeMoveSpeed(amount);
        navMeshAgent.speed = moveSpeed;
    }
}
