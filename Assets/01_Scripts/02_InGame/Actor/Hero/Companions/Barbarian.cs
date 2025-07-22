using NUnit.Framework.Interfaces;
using UnityEngine;

public enum EBarbarianState
{
    Idle, Chasing, Attack, Hit, Dead
}

public enum EBarbarianSkill
{
    Civilized, Berserker, BloodWarrior
}

public class Barbarian : Companion
{
    [SerializeField] private GameObject attackTarget;
    [SerializeField] private EBarbarianState curState;
    public EBarbarianState CurState
    {
        get { return curState; }
        set
        {
            curState = value;
            animator.SetInteger("State", (int)curState);
        }
    }
    private Player player;
    private float distanceFromAttackTarget;
    private const float detectionDistance = 15f;
    private float curDetectionCool;
    private const float detectionCool = 0.5f;
    private float curAttackCool;
    private bool attackInit;
    [SerializeField] private bool isCivilized;
    [SerializeField] private bool isBerserker;

    protected override void Awake()
    {
        base.Awake();
        player = FindAnyObjectByType<Player>();
    }

    private void OnEnable()
    {
        InitStat();
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

    private void FSM()
    {
        if (attackTarget == null && CurState != EBarbarianState.Idle)
            CurState = EBarbarianState.Idle;

        switch (CurState)
        {
            case EBarbarianState.Idle:
                if (attackTarget != null)
                {
                    CurState = EBarbarianState.Chasing;
                }
                break;
            case EBarbarianState.Chasing:
                if (distanceFromAttackTarget < attackDistance)
                {
                    curDetectionCool = 0;
                    curAttackCool = 0;
                    CurState = EBarbarianState.Attack;
                    attackInit = true;
                }
                break;
            case EBarbarianState.Attack:
                RotateTowardsTarget();
                if (attackInit)
                {
                    Debug.Log("Init Attack");
                    PerformAttack();
                    curAttackCool = 0;
                    attackInit = false;
                    IsStopped = true;
                    return;
                }

                if (distanceFromAttackTarget > attackDistance)
                {
                    IsStopped = false;
                    CurState = EBarbarianState.Chasing;
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
            case EBarbarianState.Hit:
                break;
            case EBarbarianState.Dead:
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
        Collider[] colliders;

        if (isCivilized)
            colliders = Physics.OverlapSphere(player.transform.position, detectionDistance, enemyLayer);
        else
            colliders = Physics.OverlapSphere(transform.position, detectionDistance, enemyLayer);

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

    private void RotateTowardsTarget()
    {
        if (attackTarget == null)
            return;
        Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
        direction.y = 0; // 바닥에서만 회전
        if (direction == Vector3.zero)
            return;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    protected override void Move()
    {
        if (CurState == EBarbarianState.Idle)
        {
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
        else if (CurState == EBarbarianState.Chasing)
        {
            IsStopped = false;
            navMeshAgent.SetDestination(attackTarget.transform.position);
        }
    }

    protected override void PerformAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void OnAttackHit()
    {
        if (attackTarget.GetComponent<Actor>().GetDamage(attack))
        {
            if (isBerserker)
                UpgradeAttack(Berserker.attackUpAmount);
        }
    }

    public override bool GetDamage(float damage, bool isKnockBack, Vector3 knockoutDir)
    {
        curHealth = Mathf.Clamp(curHealth - damage, 0, maxHealth);
        if (curHealth <= 0f)
        {
            // TODO: 사망 관련 처리
            CurState = EBarbarianState.Dead;
            OnDead();
            return true;
        }
        return false;
    }

    public void MakeCivilized()
    {
        if (!isCivilized)
            isCivilized = true;
    }

    public void MakeBerserker()
    {
        if (!isBerserker)
            isBerserker = true;
    }
}
