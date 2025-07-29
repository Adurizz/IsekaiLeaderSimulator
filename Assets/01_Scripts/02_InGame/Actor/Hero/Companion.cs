using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public abstract class Companion : Hero
{
    [SerializeField] protected int level;
    public int Level { get { return level; } }

    [SerializeField] protected List<int> skillLevels = new() { 0, 0, 0 };
    public List<int> SkillLevels { get { return skillLevels; } }

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

    protected override void Awake()
    {
        base.Awake();
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
        navMeshAgent.stoppingDistance = attackDistance;
        stoppingDistance = navMeshAgent.stoppingDistance;
    }

    public void LevelUp(int skillIndex)
    {
        if (level < GlobalValueHolder.maxCompanionLevel)
        {
            if (skillLevels[skillIndex] < 3)
                skillLevels[skillIndex]++;

            level++;
            InitStat();

            levelUpEvent.Invoke(skillLevels);
        }
    }

    public void SetName(string name)
    {
        actorName = name;
        this.name = name;
    }

    protected abstract void Move();
    protected abstract void PerformAttack();

    public override bool GetDamage(float damage, bool isKnockBack, Vector3 knockoutDir)
    {
        curHealth = Mathf.Clamp(curHealth - damage, 0, maxHealth);
        if (curHealth <= 0f)
        {
            OnDead();
            return true;
        }
        return false;
    }

    public override void OnDead()
    {
        isDead = true;
        IsStopped = true;
        partyManager.UnregisterPartyMember(this);
        DestroyHPBar();
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
