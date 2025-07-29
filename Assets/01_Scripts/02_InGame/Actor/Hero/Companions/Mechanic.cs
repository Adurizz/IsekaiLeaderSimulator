using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMechanicState
{
    Idle, Attack, Hit, Dead, InstallCannon
}

public enum EMechanicSkill
{
    FireStarter, SelfFix, Overheat
}

public class Mechanic : Companion
{
    private Player player;
    private float distanceFromAttackTarget;

    [SerializeField] private EMechanicState curState;
    public EMechanicState CurState
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
    [SerializeField] private float curAttackCool;
    [SerializeField] private float curAttackTime = 0f;
    private float maxAttackTime = 2.5f;
    private bool attackInit;
    private float distanceFromPlayer = 4.5f;
    [Header("Instantiate할 Object 관련")]
    [SerializeField] private GameObject areaFirePrefab;
    Queue<GameObject> areaFirePool = new();
    private const int areaFirePoolNum = 20;
    [Header("공격 관련")]
    [SerializeField] private bool isFiring;
    [SerializeField] private ParticleSystem fireEffect;
    [SerializeField] private GameObject lightEffect;
    [SerializeField] private Vector3 attackPosOffset;
    [SerializeField] private Vector3 attackPos;
    private float curDamageCool;
    private float maxDamageCool = 0.1f;
    [Header("무기들")]
    [SerializeField] private GameObject rench;
    [SerializeField] private GameObject shotGun;
    private float coolForInstall;
    private float maxInstallCool = 20f;
    // private float maxInstallCool = 5f;
    private float installedTime;
    private float maxInstallTime = 3f;
    [SerializeField] private GameObject cannonPrefab;
    private int cannonPoolNum = 50;
    [SerializeField] private List<ActorStat> cannonStats = new List<ActorStat>();
    Queue<GameObject> cannonPool = new();
    [SerializeField] private bool canAttack;
    [SerializeField] private bool isFireStarter;
    [SerializeField] private bool isSelfFix;
    private SelfFix selfFixScript;
    [SerializeField] private bool isOverheat;

    public bool IsFiring
    {
        get { return isFiring; }
        set
        {
            isFiring = value;
            // animator.SetBool("isFiring", isFiring);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        player = FindAnyObjectByType<Player>();
        attackPos = transform.rotation * attackPosOffset;
        selfFixScript = GetComponent<SelfFix>();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        transform.localRotation = transform.rotation;
        // Gizmos.DrawWireSphere(Utils.GetCenter(transform) + attackPos, attackRange);
    }
#endif

    private void OnEnable()
    {
        InitStat();
        CreateAreaFirePool();
        CreateEnergyCannonPool();
        lightEffect.SetActive(false);
    }

    public override void InitStat()
    {
        base.InitStat();
        navMeshAgent.stoppingDistance = distanceFromPlayer;
        stoppingDistance = navMeshAgent.stoppingDistance;
    }

    protected override void Update()
    {
        base.Update();
        FindNearestEnemyWithCoolTime();
        FSM();
        PerpareForInstallCannon();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void CreateAreaFirePool()
    {
        Debug.Log("메카닉 풀 생성");
        GameObject areaFirePoolGO = new GameObject("MechanicPool");
        areaFirePoolGO.transform.SetParent(GameObject.Find(GlobalValueHolder.objectPoolName).transform);

        for (int i = 0; i < areaFirePoolNum; ++i)
        {
            GameObject temp = Instantiate(areaFirePrefab, areaFirePoolGO.transform);
            temp.GetComponent<MechanicAreaFire>().SetOwner(this);
            temp.SetActive(false);
            areaFirePool.Enqueue(temp);
        }
    }

    public void GenerateAreaFire()
    {
        GameObject areaFire = areaFirePool.Dequeue();
        areaFire.transform.position = Utils.GetCenter(transform) + attackPos;
        areaFire.SetActive(true);
        areaFire.GetComponent<ParticleSystem>().Play();
    }

    public void EnqueueAreaFireOnDisable(GameObject areaFire)
    {
        areaFirePool.Enqueue(areaFire);
    }

    public void CreateEnergyCannonPool()
    {
        GameObject cannonPoolGO = new GameObject("CannonPool");
        cannonPoolGO.transform.SetParent(GameObject.Find(GlobalValueHolder.objectPoolName).transform);

        for (int i = 0; i < cannonPoolNum; ++i)
        {
            GameObject temp = Instantiate(cannonPrefab, cannonPoolGO.transform);
            temp.GetComponent<Cannon>().InformOwner(this);
            temp.SetActive(false);
            cannonPool.Enqueue(temp);
        }
    }

    private void PerpareForInstallCannon()
    {
        if (CurState == EMechanicState.InstallCannon || CurState == EMechanicState.Dead)
            return;

        coolForInstall += Time.deltaTime;
        if (coolForInstall > maxInstallCool)
        {
            if (IsFiring)
                return;
            else
            {
                RaiseWrench();
                CurState = EMechanicState.InstallCannon;
                animator.SetTrigger("InstallCannon");
                coolForInstall = 0;
            }
        }
    }

    public void InstallCannon()
    {
        if (cannonPool.Count <= 0)
        {
            GameObject temp = Instantiate(cannonPrefab, GameObject.Find(GlobalValueHolder.objectPoolName).transform);
            temp.SetActive(false);
            cannonPool.Enqueue(temp);
        }

        GameObject cannon = cannonPool.Dequeue();
        cannon.GetComponent<Cannon>().SetStat(cannonStats[skillLevels[(int)EMechanicSkill.Overheat]]);
        cannon.GetComponent<Cannon>().InitStat();
        cannon.SetActive(true);
        cannon.transform.position = transform.position;
    }

    public void EnqueueCannonOnDisable(GameObject cannon)
    {
        cannonPool.Enqueue(cannon);
    }

    protected override void Move()
    {
        if (CurState == EMechanicState.Dead)
            return;
        if (CurState == EMechanicState.InstallCannon)
        {
            if (!IsStopped)
                IsStopped = true;
            return;
        }

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
        attackPos = transform.rotation * attackPosOffset;
    }

    private void RotateTowardsTarget()
    {
        if (CurState == EMechanicState.Dead)
            return;
        if (attackTarget == null)
            return;
        /*
        if (isFiring)
            return;
        */
        Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
        direction.y = 0; // 바닥에서만 회전
        if (direction == Vector3.zero)
            return;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        transform.rotation = lookRotation;
        attackPos = transform.rotation * attackPosOffset;
    }

    private void FSM()
    {
        if (CurState != EMechanicState.InstallCannon && attackTarget == null && CurState != EMechanicState.Dead && CurState != EMechanicState.Idle)
            CurState = EMechanicState.Idle;

        switch (CurState)
        {
            case EMechanicState.Idle:
                if (canAttack)
                    RaiseShotgun();
                animator.SetLayerWeight(1, 0);
                IsFiring = false;
                if (attackTarget != null)
                {
                    curDetectionCool = 0;
                    curAttackCool = 0;
                    CurState = EMechanicState.Attack;
                    attackInit = true;
                }
                break;
            case EMechanicState.Attack:
                RotateTowardsTarget();
                animator.SetLayerWeight(1, 1);
                if (attackInit)
                {
                    Debug.Log("Init Attack");
                    IsFiring = true;
                    fireEffect.Play();
                    lightEffect.SetActive(true);
                    curAttackCool = 0;
                    attackInit = false;
                    return;
                }

                // 공격
                if (!isFiring)
                {
                    curAttackCool += Time.deltaTime;
                    if (curAttackCool >= attackSpeed)
                    {
                        IsFiring = true;
                        // IsStopped = true;
                        fireEffect.Play();
                        lightEffect.SetActive(true);
                        curAttackCool = 0;
                    }
                }
                else
                {
                    PerformAttack();
                }
                
                break;
            case EMechanicState.Hit:
                break;
            case EMechanicState.Dead:
                animator.SetLayerWeight(1, 0);
                break;
            case EMechanicState.InstallCannon:
                animator.SetLayerWeight(1, 0);
                installedTime += Time.deltaTime;
                if (installedTime >= maxInstallTime)
                {
                    InstallCannon();
                    installedTime = 0;
                    CurState = EMechanicState.Idle;
                }
                break;
        }
    }



    private void FindNearestEnemyWithCoolTime()
    {
        if (!canAttack)
            return;

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

    protected override void PerformAttack()
    {
        if (!canAttack)
            return;

        curDamageCool += Time.deltaTime;
        if (curDamageCool >= maxDamageCool)
        {
            ApplyAreaDamage(Utils.GetCenter(transform) + attackPos, attackRange, 0.1f);
            curDamageCool = 0;
        }

        curAttackTime += Time.deltaTime;
        if (curAttackTime >= maxAttackTime)
        {
            IsFiring = false;
            IsStopped = false;
            lightEffect.SetActive(false);
            curAttackTime = 0;
            if (isFireStarter)
                GenerateAreaFire();
        }
    }

    public void ApplyAreaDamage(Vector3 position, float range, float damageMultiplier = 1f)
    {
        Collider[] colliders = Physics.OverlapSphere(position, range, enemyLayer);

        foreach (Collider col in colliders)
        {
            if (col.gameObject.GetComponent<Enemy>().IsDead)
                continue;

            col.gameObject.GetComponent<Actor>().GetDamage(attack * damageMultiplier);
        }
    }

    public override bool GetDamage(float damage, bool isKnockBack, Vector3 knockoutDir)
    {
        curHealth = Mathf.Clamp(curHealth - damage, 0, maxHealth);
        getDamageEffect.Play();
        UpdateHPBar();
        if (curHealth <= 0f)
        {
            // TODO: 사망 관련 처리
            CurState = EMechanicState.Dead;
            selfFixScript.DeactivateSelfFixEffect();
            OnDead();
            return true;
        }

        return false;
    }

    public void MakeAttackAvailable()
    {
        if (!canAttack)
            canAttack = true;
        RaiseShotgun();
        Debug.Log("총들어");
    }

    public void RaiseWrench()
    {
        rench.SetActive(true);
        shotGun.SetActive(false);
    }

    public void RaiseShotgun()
    {
        rench.SetActive(false);
        shotGun.SetActive(true);
    }

    #region 스킬 마스터 처리
    public void MakeFireStarter()
    {
        if (!isFireStarter)
            isFireStarter = true;
    }

    public void MakeSelfFix()
    {
        if (!isSelfFix)
            isSelfFix = true;
    }

    public void MakeOverheat()
    {
        if (!isOverheat)
            isOverheat = true;
    }
    #endregion
}
