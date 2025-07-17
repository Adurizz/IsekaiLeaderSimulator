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
    protected NavMeshAgent navMeshAgent;
    protected Animator animator;
    private LayerMask heroLayer;
    [SerializeField] private float curDetectionCool = 0;
    private const float maxDetectionCool = 1f;
    private const float maxDetectionCoolWhileAttack = 0.1f;
    [SerializeField] protected EEnemyState curState;
    [SerializeField] protected float curAttackCool;
    [SerializeField] private bool attackInit;

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
                
                if (attackInit)
                {
                    Debug.Log("Init Attack");
                    Attack();
                    curAttackCool = 0;
                    attackInit = false;
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
                    CurState = EEnemyState.Chasing;
                    return;
                }

                // 공격
                curAttackCool += Time.deltaTime;
                if (curAttackCool >= attackSpeed)
                {
                    Attack();
                    curAttackCool = 0;
                }
                break;
            case EEnemyState.Hit:
                
                break;
            case EEnemyState.Dead:
                
                break;
        }
    }

    protected virtual void Move()
    {
        if (Target == null || curState != EEnemyState.Chasing)
        {
            return;
        }
        /*
        else
        {
            if (CurMoveSpeed == 0)
            {
                CurMoveSpeed = moveSpeed;
            }
        }

        if (distanceFromTarget < attackDistance)
        {
            CurMoveSpeed = 0;
        }
        */

        navMeshAgent.SetDestination(target.position);
    }

    protected virtual void Attack()
    {
        animator.SetTrigger("Attack");
    }

    public virtual void MeleeAttackHit()
    {
        Debug.Log("Hit!");
    }

    public override void OnDead()
    {
        spawnManager.EnqueueEnemyOnDead(gameObject);
        gameObject.SetActive(false);
    }

    protected virtual void DropItem()
    {

    }
}
