using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Actor : MonoBehaviour
{
    [SerializeField] protected ActorStat actorStat;
    [SerializeField] protected string actorName = "actor";
    public string ActorName { get { return actorName; } }
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float curHealth;
    [SerializeField] protected float attack;
    public float Attack { get { return attack; } }
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float attackDistance;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected bool isDead;
    [SerializeField] protected ParticleSystem getDamageEffect;
    public bool IsDead { get { return isDead; } }

    public virtual void InitStat()
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
    }

    public virtual bool GetDamage(float damage, bool isKnockBack = false, Vector3 knockoutDir = new Vector3())
    {
        curHealth = Mathf.Clamp(curHealth - damage, 0, maxHealth);
        if (curHealth <= 0f)
        {
            isDead = true;
            // TODO: 사망 관련 처리
            OnDead();
            return true;
        }
        else
        {
            if (isKnockBack)
            {
                GetKnockBack(knockoutDir, 10f, 3f);
            }
            return false;
        }
    }

    public virtual void GetKnockBack(Vector3 knockbackDir, float knockbackForce, float duration)
    {
        NavMeshAgent navMeshAgent = null;
        bool navMeshPresents = TryGetComponent<NavMeshAgent>(out navMeshAgent);
        if (navMeshPresents)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.updatePosition = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode.Impulse);

        if (navMeshPresents)
            StartCoroutine(KnockBackResetCor(duration, rb, navMeshAgent));
    }

    protected IEnumerator KnockBackResetCor(float duration, Rigidbody rb, NavMeshAgent navMeshAgent)
    {
        yield return new WaitForSeconds(duration);

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        navMeshAgent.Warp(transform.position);
        navMeshAgent.isStopped = false;
        navMeshAgent.updatePosition = true;
    }

    public virtual void GetHeal(float healAmount)
    {
        if (isDead)
            return;

        curHealth = Mathf.Clamp(curHealth + healAmount, 0, maxHealth);
    }

    public abstract void OnDead();

    #region StatUpgrade -> Companion 스킬에서 사용
    public void UpgradeMaxHP(float amount)
    {
        maxHealth += amount;
    }

    public void UpgradeAttack(float amount)
    {
        attack += amount;
    }

    public void UpgradeAttackSpeed(float amount)
    {
        attackSpeed -= amount;
    }

    public void UpgradeAttackDistance(float amount)
    {
        attackDistance += amount;
    }

    public void UpgradeAttackRange(float amount)
    {
        attackRange += amount;
    }

    public virtual void UpgradeMoveSpeed(float amount)
    {
        moveSpeed += amount;
    }
    #endregion
}
