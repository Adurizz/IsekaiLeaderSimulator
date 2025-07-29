using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Companion
{
    private Mechanic ownerMechanic;
    [SerializeField] private GameObject attackTarget;
    private float curDetectionCool;
    private const float detectionCool = 1f;
    [SerializeField] private float curAttackCool;
    private float distanceFromAttackTarget;

    [SerializeField] private GameObject cannonBulletPrefab;
    private int cannonBulletPoolNum = 20;
    Queue<GameObject> cannonBulletPool = new();
    [SerializeField] private ParticleSystem cannonDestroyEffect;

    protected override void Awake()
    {
        CreateCannonBulletPool();
    }

    /*private void OnEnable()
    {
        InitStat();
    }
    */

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
#endif

    public void SetStat(ActorStat stat)
    {
        actorStat = stat;
    }

    public override void InitStat()
    {
        List<float> originStatList = actorStat.CopyStat();
        maxHealth = originStatList[0];
        curHealth = maxHealth;
        attack = originStatList[1];
        attackSpeed = originStatList[2];
        attackDistance = originStatList[3];
        attackRange = originStatList[4];
        moveSpeed = originStatList[5];
        isDead = false;

        enemyLayer = LayerMask.GetMask("Enemy");
    }

    public void InformOwner(Mechanic owner)
    {
        ownerMechanic = owner;
    }

    protected override void Move()
    {
        return;
    }

    private void Update()
    {
        FindNearestEnemyWithCoolTime();
        RotateTowardsTarget();
        PrepareAttack();
    }

    public void CreateCannonBulletPool()
    {
        GameObject cannonPoolGO = new GameObject("CannonBulletPool");
        cannonPoolGO.transform.SetParent(GameObject.Find(GlobalValueHolder.objectPoolName).transform);

        for (int i = 0; i < cannonBulletPoolNum; ++i)
        {
            GameObject temp = Instantiate(cannonBulletPrefab, cannonPoolGO.transform);
            temp.transform.position = transform.position;
            temp.SetActive(false);
            cannonBulletPool.Enqueue(temp);
        }
    }
    
    public void EnqueueCannonBulletOnDisable(GameObject cannonBullet)
    {
        cannonBullet.transform.position = transform.position;
        cannonBulletPool.Enqueue(cannonBullet);
    }

    private void PrepareAttack()
    {
        if (attackTarget == null || IsDead)
            return;

        curAttackCool += Time.deltaTime;
        if (curAttackCool >= attackSpeed)
        {
            PerformAttack();
            curAttackCool = 0;
        }
    }

    protected override void PerformAttack()
    {
        FireCannonBullet();
    }

    private void FireCannonBullet()
    {
        if (cannonBulletPool.Count <= 0)
            return;

        GameObject cannonBullet = cannonBulletPool.Dequeue();
        CannonBulletController bulletController = cannonBullet.GetComponent<CannonBulletController>();
        bulletController.SetOwnerTransform(transform);
        bulletController.SetTargetTransform(attackTarget.transform);
        cannonBullet.SetActive(true);
    }

    private void RotateTowardsTarget()
    {
        if (IsDead)
            return;

        if (attackTarget == null)
        {
            transform.Rotate(new Vector3(0, 50, 0) * Time.deltaTime);
        }
        else
        {
            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            direction.y = 0; // 바닥에서만 회전
            if (direction == Vector3.zero)
                return;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    private void FindNearestEnemyWithCoolTime()
    {
        if (IsDead)
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

    public override bool GetDamage(float damage, bool isKnockBack, Vector3 knockoutDir)
    {
        curHealth = Mathf.Clamp(curHealth - damage, 0, maxHealth);
        getDamageEffect.Play();
        if (curHealth <= 0f)
        {
            // TODO: 사망 관련 처리
            OnDead();
            return true;
        }

        return false;
    }

    public override void OnDead()
    {
        isDead = true;
        ownerMechanic.EnqueueCannonOnDisable(gameObject);
        cannonDestroyEffect.Play();
        Invoke(nameof(VanishBody), 3f);
    }

    private void VanishBody()
    {
        gameObject.SetActive(false);
    }
}
