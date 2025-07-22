using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : Actor
{
    [SerializeField] protected float distanceFromTarget;
    [SerializeField] protected Transform target;
    public Transform Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
            if (target == null)
                distanceFromTarget = Mathf.Infinity;
        }
    }
    protected EnemySpawnManager spawnManager;
    protected ItemSpawnManager itemSpawnManager;
    protected NavMeshAgent navMeshAgent;
    protected Animator animator;
    // 적 탐지 처리
    private LayerMask heroLayer;
    [SerializeField] private float curDetectionCool = 0;
    private const float maxDetectionCool = 1f;
    private const float maxDetectionCoolWhileAttack = 0.1f;
    [SerializeField] protected EEnemyState curState;
    protected float curAttackCool;
    protected bool attackInit;

    // Hit 처리
    protected float curDelay;
    protected const float hitDelay = 1f;

    public EEnemyState CurState
    {
        get
        {
            return curState;
        }
        set
        {
            curState = value;
            animator.SetInteger("State", (int)curState);
        }
    }
    /// <summary>
    /// moveSpeed를 원본값으로 하는 curMoveSpeed -> 자유롭게 변형해서 사용, 원본 보존해서 복구하기 위함
    /// </summary>
    [SerializeField] protected float curMoveSpeed;

    public float CurMoveSpeed
    {
        get
        {
            return curMoveSpeed;
        }
        set
        {
            curMoveSpeed = value;
            navMeshAgent.speed = curMoveSpeed;
            /*
            if (curMoveSpeed != 0)
                animator.SetBool("isMoving", true);
            else
                animator.SetBool("isMoving", false);
            */
        }
    }

    private void Awake()
    {
        spawnManager = FindAnyObjectByType<EnemySpawnManager>();
        itemSpawnManager = FindAnyObjectByType<ItemSpawnManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        heroLayer = LayerMask.GetMask("Hero");
    }

    protected virtual void OnEnable()
    {
        InitStat();
    }

    public override void InitStat()
    {
        base.InitStat();
        distanceFromTarget = Mathf.Infinity;
        curState = EEnemyState.Idle;
        CurMoveSpeed = moveSpeed;
        navMeshAgent.stoppingDistance = attackDistance;
    }

    protected virtual void Update()
    {
        SetTargetWithCoolTime();
        FSM();
    }

    protected virtual void SetTargetWithCoolTime()
    {
        if (curState == EEnemyState.Attack)
            return;

        curDetectionCool += Time.deltaTime;
        if (curDetectionCool > maxDetectionCool)
        {
            SetTarget();
            curDetectionCool = 0;
        }
    }

    /// <summary>
    /// 가장 가까운 Hero(Player 포함) 탐색
    /// </summary>
    protected virtual void SetTarget()
    {
        // target = FindAnyObjectByType<Player>().transform;
        // Debug.Log(navMeshAgent.ta)
        Collider[] colliders = Physics.OverlapSphere(transform.position, 400, heroLayer);
        if (colliders.Length == 0)
        {
            Target = null;
            return;
        }

        Transform nearest = null;
        float minSqrDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            if (col.gameObject.GetComponent<Actor>().IsDead)
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
            Target = nearest;
            distanceFromTarget = Vector3.Distance(transform.position, Target.position);
        }
        else
        {
            Target = null;
        }
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected virtual void FSM()
    {
        if (Target == null)
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
                if (distanceFromTarget <= attackDistance)
                {
                    curDetectionCool = 0;
                    curAttackCool = 0;
                    CurState = EEnemyState.Attack;
                    attackInit = true;
                }
                break;
            case EEnemyState.Attack:
                RotateTowardsTarget();
                if (attackInit)
                {
                    Debug.Log("Init Attack");
                    PerformAttack();
                    curAttackCool = 0;
                    attackInit = false;
                    navMeshAgent.isStopped = true;
                    return;
                }
                
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

                // 공격
                curAttackCool += Time.deltaTime;
                if (curAttackCool >= attackSpeed)
                {
                    PerformAttack();
                    curAttackCool = 0;
                }
                break;
            case EEnemyState.Hit:
                Debug.Log("Hit State");
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

    protected virtual void Move()
    {
        if (Target == null || curState != EEnemyState.Chasing)
        {
            return;
        }
        navMeshAgent.SetDestination(target.position);
    }

    protected virtual void PerformAttack()
    {
        animator.SetTrigger("Attack");
    }

    public virtual void MeleeAttackHit()
    {
        Debug.Log("Hit: " + Target.name);
        target.GetComponent<Actor>().GetDamage(attack);
    }

    private void RotateTowardsTarget()
    {
        if (Target == null) return;
        Vector3 direction = (Target.transform.position - transform.position).normalized;
        direction.y = 0; // 바닥에서만 회전
        if (direction == Vector3.zero) return;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        // 서서히 회전하고 싶으면 Lerp/RotateTowards, 즉시 회전이면 그냥 대입
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

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
        else
        {
            navMeshAgent.isStopped = true;
            animator.SetTrigger("Hit");
            curDelay = 0;
            if (CurState != EEnemyState.Hit)
            {
                CurState = EEnemyState.Hit;
            }
        }
    }

    public override void OnDead()
    {
        isDead = true;
        navMeshAgent.isStopped = true;
        DropItem();
        Invoke(nameof(VanishBody), 3f);
    }

    private void VanishBody()
    {
        spawnManager.EnqueueEnemyOnDead(gameObject);
        gameObject.SetActive(false);
    }

    protected virtual void DropItem()
    {
        EnemyStat enemyStat = actorStat as EnemyStat;
        int[] rewardInfo = enemyStat.GetRewardInfo();
        itemSpawnManager.SpawnGoldItem(rewardInfo[0], transform);
        itemSpawnManager.SpawnUpgradeStoneItem(rewardInfo[1], transform);
    }
}
