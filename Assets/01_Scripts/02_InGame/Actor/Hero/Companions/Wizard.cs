using System.Collections.Generic;
using UnityEngine;

public enum EWizardState
{
    Idle, Attack, Hit, Dead, CastingSpell
}

public enum EWizardSkill
{
    VoidCraft, ManaCraft, ElementCraft
}

public class Wizard : Companion
{
    private Player player;
    [SerializeField] private EWizardState curState;
    private float distanceFromAttackTarget;

    public EWizardState CurState
    {
        get { return curState; }
        set
        {
            curState = value;
            animator.SetInteger("State", (int)curState);
        }
    }
    [SerializeField] private GameObject attackTarget;
    private float curDetectionCool;
    private const float detectionCool = 0.5f;
    private float curAttackCool;
    private bool attackInit;
    private float distanceFromPlayer = 6f;
    [SerializeField] private GameObject energyBall;
    Queue<GameObject> energyBallPool = new();
    private const int energyBallPoolNum = 20;
    [SerializeField] private int shotCount;
    [SerializeField] private int countForCastSpell = 5;
    [SerializeField] private ParticleSystem[] spellParticle = new ParticleSystem[4];
    [SerializeField] private float[] spellRange = new float[4];

    protected override void Awake()
    {
        base.Awake();
        player = FindAnyObjectByType<Player>();
    }

    private void OnEnable()
    {
        InitStat();
        CreateEnergyBallPool();
    }

    public override void InitStat()
    {
        base.InitStat();
        navMeshAgent.stoppingDistance = distanceFromPlayer;
        stoppingDistance = navMeshAgent.stoppingDistance;
    }

    private void Update()
    {
        FindNearestEnemyWithCoolTime();
        FSM();
    }

    private void FixedUpdate()
    {
        Move();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spellRange[skillLevels[(int)EWizardSkill.ManaCraft]]);
    }
#endif

    private void FSM()
    {
        if (CurState != EWizardState.CastingSpell && attackTarget == null && CurState != EWizardState.Idle)
            CurState = EWizardState.Idle;

        switch (CurState)
        {
            case EWizardState.Idle:
                animator.SetLayerWeight(1, 0);
                if (attackTarget != null)
                {
                    curDetectionCool = 0;
                    curAttackCool = 0;
                    CurState = EWizardState.Attack;
                    attackInit = true;
                }
                break;
            case EWizardState.Attack:
                RotateTowardsTarget();
                animator.SetLayerWeight(1, 1);
                if (attackInit)
                {
                    Debug.Log("Init Attack");
                    PerformAttack();
                    curAttackCool = 0;
                    attackInit = false;
                    return;
                }

                // 공격
                curAttackCool += Time.deltaTime;
                if (curAttackCool >= attackSpeed)
                {
                    PerformAttack();
                    curAttackCool = 0;
                }
                break;
            case EWizardState.Hit:
                break;
            case EWizardState.Dead:
                animator.SetLayerWeight(1, 0);
                break;
            case EWizardState.CastingSpell:
                break;
        }
    }

    private void FindNearestEnemyWithCoolTime()
    {
        curDetectionCool += Time.deltaTime;
        if (curDetectionCool >= detectionCool)
        {
            FindNearestEnemy();
            curDetectionCool = 0;
        }
    }

    private void FindNearestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackDistance, enemyLayer);
        if (colliders.Length == 0)
        {
            attackTarget = null;
            return;
        }

        Transform nearest = null;
        float minSqrDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            if (col.gameObject.GetComponent<Enemy>().IsDead)
                continue;

            float sqrDist = (col.transform.position - transform.position).sqrMagnitude;
            if (sqrDist < minSqrDistance)
            {
                minSqrDistance = sqrDist;
                nearest = col.transform;
            }
        }

        if (nearest != null)
        {
            attackTarget = nearest.gameObject;
            distanceFromAttackTarget = Vector3.Distance(attackTarget.transform.position, transform.position);
        }
        else
        {
            attackTarget = null;
        }
    }

    protected override void Move()
    {
        if (CurState == EWizardState.Dead || CurState == EWizardState.CastingSpell)
            return;

        // 언제나 플레이어를 따라다님
        if (Vector3.Distance(player.transform.position, transform.position) > stoppingDistance)
        {
            if (IsStopped)
                IsStopped = false;

            navMeshAgent.SetDestination(player.transform.position);
        }
        else
        {
            IsStopped = true;
        }
    }

    private void RotateTowardsTarget()
    {
        if (CurState == EWizardState.Dead)
            return;
        if (attackTarget == null)
            return;

        Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
        direction.y = 0; // 바닥에서만 회전
        if (direction == Vector3.zero)
            return;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        transform.rotation = lookRotation;
    }

    protected override void PerformAttack()
    {
        ShotEnergyBall();
        animator.SetTrigger("Attack");
        ++shotCount;
        if (shotCount >= countForCastSpell)
        {
            CurState = EWizardState.CastingSpell;
            CastSpell();
            shotCount = 0;
        }
    }

    public void CreateEnergyBallPool()
    {
        Debug.Log("궁수 풀 생성");
        GameObject energyBallPoolGO = new GameObject("WizardEnergyBallPool");
        energyBallPoolGO.transform.SetParent(GameObject.Find(GlobalValueHolder.objectPoolName).transform);

        for (int i = 0; i < energyBallPoolNum; ++i)
        {
            GameObject temp = Instantiate(energyBall, energyBallPoolGO.transform);
            temp.SetActive(false);
            energyBallPool.Enqueue(temp);
        }
    }

    private void ShotEnergyBall()
    {
        if (energyBallPool.Count <= 0)
            return;

        GameObject energyBall = energyBallPool.Dequeue();
        energyBall.SetActive(true);
        WizardEnergyBallController energyBallController = energyBall.GetComponent<WizardEnergyBallController>();
        energyBallController.SetOwnerTransform(transform);
        energyBallController.SetTargetTransform(attackTarget.transform);
    }

    private void CastSpell()
    {
        IsStopped = true;
        animator.SetLayerWeight(1, 0);
        animator.SetTrigger("CastSpell");
        spellParticle[skillLevels[(int)EWizardSkill.ManaCraft]].Play();
    }

    public void OnEndCastSpell()
    {
        Debug.Log("Cast End");
        CurState = EWizardState.Idle;
    }

    public void ApplyDamageToNearEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, spellRange[skillLevels[(int)EWizardSkill.ManaCraft]], enemyLayer);

        foreach (Collider col in colliders)
        {
            if (col.gameObject.GetComponent<Enemy>().IsDead)
                continue;

            col.gameObject.GetComponent<Actor>().GetDamage(attack);
        }
    }

    public void EnqueueEnergyBallOnDisable(GameObject energyBall)
    {
        energyBallPool.Enqueue(energyBall);
    }

    public override bool GetDamage(float damage, bool isKnockBack, Vector3 knockoutDir)
    {
        curHealth = Mathf.Clamp(curHealth - damage, 0, maxHealth);
        if (curHealth <= 0f)
        {
            // TODO: 사망 관련 처리
            CurState = EWizardState.Dead;
            OnDead();
            return true;
        }

        return false;
    }
}
