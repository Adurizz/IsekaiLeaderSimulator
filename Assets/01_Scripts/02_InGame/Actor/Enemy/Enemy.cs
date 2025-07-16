using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : Actor
{
    [SerializeField] protected Transform target;
    protected EnemySpawnManager spawnManager;
    protected NavMeshAgent navMeshAgent;
    protected Animator animator;
    protected float distanceFromTarget;
    private LayerMask heroLayer;
    private float curDetectionCool = 0;
    private const float maxDetectionCool = 1f;

    public float MoveSpeed
    {
        get
        {
            return moveSpeed;
        }
        set
        {
            moveSpeed = value;
            if (moveSpeed != 0)
                animator.SetBool("isMoving", true);
            else
                animator.SetBool("isMoving", false);
        }
    }

    private void Awake()
    {
        spawnManager = FindAnyObjectByType<EnemySpawnManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        heroLayer = LayerMask.GetMask("Hero");
        SetTarget();
    }

    public override void InitStat()
    {
        base.InitStat();
        MoveSpeed = moveSpeed;
        navMeshAgent.speed = MoveSpeed;
    }

    protected virtual void Update()
    {
        SetTargetWithCoolTime();
    }

    protected virtual void SetTargetWithCoolTime()
    {
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
            target = null;
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
            target = nearest;
            distanceFromTarget = Vector3.Distance(transform.position, target.position);
        }
    }

    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected virtual void Move()
    {
        if (target == null)
            return;

        navMeshAgent.SetDestination(target.position);
    }

    protected abstract void Attack();

    public override void OnDead()
    {
        spawnManager.EnqueueEnemyOnDead(gameObject);
        gameObject.SetActive(false);
    }

    protected virtual void DropItem()
    {

    }
}
