using System.Collections.Generic;
using UnityEngine;

public enum EArchorState
{
    Idle, Attack, Hit, Dead
}

public enum EArchorSkill
{
    Deadeye, Ranger, Runner
}

public class Archor : Companion
{
    private Player player;
    [SerializeField] private EArchorState curState;
    private float distanceFromAttackTarget;

    public EArchorState CurState
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
    [SerializeField] private bool isAttackKnockBack;
    [SerializeField] private bool isThirdAttackEnhanced;
    [SerializeField] private bool canEvade;
    private float distanceFromPlayer = 6f;
    [SerializeField] private GameObject normalArrow;
    Queue<GameObject> normalArrowPool= new();
    private const int arrowPoolNum = 20;
    [SerializeField] private int fireCount;
    private Runner runnerScript;

    protected override void Awake()
    {
        base.Awake();
        player = FindAnyObjectByType<Player>();
        runnerScript = GetComponent<Runner>();
    }

    private void OnEnable()
    {
        InitStat();
        CreateArrowPool();
    }

    public override void InitStat()
    {
        base.InitStat();
        navMeshAgent.stoppingDistance = distanceFromPlayer;
        stoppingDistance = navMeshAgent.stoppingDistance;
        fireCount = 0;
    }

    protected override void Update()
    {
        base.Update();
        FindNearestEnemyWithCoolTime();
        FSM();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void FSM()
    {
        if (attackTarget == null && CurState != EArchorState.Idle)
            CurState = EArchorState.Idle;

        switch (CurState)
        {
            case EArchorState.Idle:
                animator.SetLayerWeight(1, 0);
                if (attackTarget != null)
                {
                    curDetectionCool = 0;
                    curAttackCool = 0;
                    CurState = EArchorState.Attack;
                    attackInit = true;
                }
                break;
            case EArchorState.Attack:
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
            case EArchorState.Hit:
                break;
            case EArchorState.Dead:
                animator.SetLayerWeight(1, 0);
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
        if (CurState == EArchorState.Dead)
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
            if (canEvade)
            {
                if (attackTarget != null && distanceFromAttackTarget < stoppingDistance)
                {
                    IsStopped = false;
                    transform.localRotation = transform.rotation;
                    Vector3 forwardOffset = transform.rotation * runnerScript.EvadeOffset;

                    navMeshAgent.SetDestination(Utils.GetCenter(transform) + forwardOffset);
                    return;
                }
                else
                    IsStopped = true;
            }
            else
                IsStopped = true;
        }
    }

    private void RotateTowardsTarget()
    {
        if (CurState == EArchorState.Dead)
            return;
        if (attackTarget == null)
            return;

        Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
        direction.y = 0; // 바닥에서만 회전
        if (direction == Vector3.zero)
            return;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = lookRotation;
    }

    protected override void PerformAttack()
    {
        FireArrow();
        animator.SetTrigger("Attack");
    }

    public void CreateArrowPool()
    {
        Debug.Log("궁수 풀 생성");
        GameObject arrowPoolGO = new GameObject("ArchorArrowPool");
        arrowPoolGO.transform.SetParent(GameObject.Find(GlobalValueHolder.objectPoolName).transform);

        for (int i = 0; i < arrowPoolNum; ++i)
        {
            GameObject temp = Instantiate(normalArrow, arrowPoolGO.transform);
            temp.SetActive(false);
            normalArrowPool.Enqueue(temp);
        }
    }

    private void FireArrow()
    {
        if (normalArrowPool.Count <= 0)
            return;

        GameObject arrow = normalArrowPool.Dequeue();
        ArchorArrowController arrowController = arrow.GetComponent<ArchorArrowController>();
        arrowController.SetOwnerTransform(transform);
        arrowController.SetTargetTransform(attackTarget.transform);
        if (isAttackKnockBack)
            arrowController.IsKnockBack = true;
        else
            arrowController.IsKnockBack= false;

        if (isThirdAttackEnhanced)
        {
            ++fireCount;
            if (fireCount >= 3)
            {
                arrowController.IsThirdAttack = true;
                fireCount = 0;
            }
        }
        else
            arrowController.IsThirdAttack= false;
        arrow.SetActive(true);
    }

    public void EnqueueArrowOnDisable(GameObject arrow)
    {
        normalArrowPool.Enqueue(arrow);
    }

    public override bool GetDamage(float damage, bool isKnockBack, Vector3 knockoutDir)
    {
        curHealth = Mathf.Clamp(curHealth - damage, 0, maxHealth);
        getDamageEffect.Play();
        UpdateHPBar();
        if (curHealth <= 0f)
        {
            // TODO: 사망 관련 처리
            CurState = EArchorState.Dead;
            OnDead();
            return true;
        }

        return false;
    }

    public void MakeAttackKnockBackable()
    {
        if (!isAttackKnockBack) isAttackKnockBack = true;
    }

    public void MakeThirdAttackEnhanced()
    {
        if (!isThirdAttackEnhanced) isThirdAttackEnhanced = true;
    }

    public void MakeEvasionPossible()
    {
        if (!canEvade)
            canEvade = true;
    }
}
