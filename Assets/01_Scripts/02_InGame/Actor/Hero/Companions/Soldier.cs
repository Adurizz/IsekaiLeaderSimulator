using UnityEngine;

public enum ESoldierState
{
    Idle, Chasing, Attack, Hit, Dead
}

public enum ESoldierSkill
{
    Guardian, Striker, Warden
}

public class Soldier : Companion
{
    private Player player;
    private PlayerAttackHandler playerAttackHandler;
    [SerializeField] private GameObject protectTarget;
    [SerializeField] private ESoldierState curState;
    public ESoldierState CurState
    {
        get { return curState; }
        set
        {
            curState = value;
            animator.SetInteger("State", (int)curState);
        }
    }
    private const float protectDistance = 15f;
    [SerializeField] private GameObject attackTarget;
    private float distanceFromAttackTarget;
    private float curDetectionCool;
    private const float detectionCool = 0.5f;
    private float curAttackCool;
    private bool attackInit;
    private GuardianSkill guardianSkill;
    [SerializeField] private bool isAttackKnockBack;
    [SerializeField] private bool canDodge;

    protected override void Awake()
    {
        base.Awake();
        player = FindAnyObjectByType<Player>();
        playerAttackHandler = player.gameObject.GetComponent<PlayerAttackHandler>();
        guardianSkill = GetComponent<GuardianSkill>();
    }

    private void OnEnable()
    {
        InitStat();
    }

    private void Start()
    {
        protectTarget = player.gameObject;
    }

    public void SetProtectTarget(GameObject newTarget)
    {
        protectTarget = newTarget;
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
        if (attackTarget == null && CurState != ESoldierState.Idle)
            CurState = ESoldierState.Idle;

        switch (CurState)
        {
            case ESoldierState.Idle:
                if (attackTarget != null)
                {
                    CurState = ESoldierState.Chasing;
                }
                break;
            case ESoldierState.Chasing:
                if (distanceFromAttackTarget < attackDistance)
                {
                    curDetectionCool = 0;
                    curAttackCool = 0;
                    CurState = ESoldierState.Attack;
                    attackInit = true;
                }
                break;
            case ESoldierState.Attack:
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
                    CurState = ESoldierState.Chasing;
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
            case ESoldierState.Hit:
                break;
            case ESoldierState.Dead:
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
        Collider[] colliders = Physics.OverlapSphere(protectTarget.transform.position, protectDistance, enemyLayer);
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
        if (CurState == ESoldierState.Idle)
        {
            if (Vector3.Distance(protectTarget.transform.position, transform.position) > stoppingDistance)
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
        else if (CurState == ESoldierState.Chasing)
        {
            IsStopped = false;
            navMeshAgent.SetDestination(attackTarget.transform.position);
        }
    }

    protected override void PerformAttack()
    {
        animator.SetTrigger("Attack");
    }

    /// <summary>
    /// 공격 적중시: animation에 바인딩
    /// </summary>
    public void OnAttackHit()
    {
        if (attackTarget == null)
            return;

        if (isAttackKnockBack)
        {
            Debug.Log("넉백 공격");
            attackTarget.GetComponent<Actor>().GetDamage(attack, true, (attackTarget.transform.position - transform.position).normalized);
        }
        else
        {
            attackTarget.GetComponent<Actor>().GetDamage(attack);
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
        
        transform.rotation = lookRotation;
    }

    public override bool GetDamage(float damage, bool isKnockBack, Vector3 knockoutDir)
    {
        if (canDodge)
        {
            int rand = Random.Range(0, 10);
            if (rand < 3)
            {
                Debug.Log("회피 성공!");
                return false;
            }
        }

        curHealth = Mathf.Clamp(curHealth - damage, 0, maxHealth);
        getDamageEffect.Play();
        if (curHealth <= 0f)
        {
            // TODO: 사망 관련 처리
            CurState = ESoldierState.Dead;
            OnDead();
            return true;
        }

        return false;
    }

    public void MakeAttackKnockBack()
    {
        if (!isAttackKnockBack)
            isAttackKnockBack = true;
    }

    public void MakeDodgePossible()
    {
        if (!canDodge)
            canDodge = true;
    }

    public override void OnDead()
    {
        guardianSkill.DeactivateAreaHealEffect();
        base.OnDead();
    }
}
