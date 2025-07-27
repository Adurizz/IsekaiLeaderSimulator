using UnityEngine;

public class EnemyEnergyBallController : ProjectileController
{
    private SkeletonWizard ownerWizard;
    [SerializeField] private float attackRange;
    private EnemySpawnManager enemySpawnManager;
    private float curLife;
    private float lifeTime = 5f;
    [SerializeField] private ParticleSystem areaAttackParticle;
    private bool Hit;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif

    public void SetEnemySpawnManager(EnemySpawnManager enemySpawnManager)
    {
        this.enemySpawnManager = enemySpawnManager;
    }

    public override void SetOwnerTransform(Transform newOwnerTransform)
    {
        base.SetOwnerTransform(newOwnerTransform);
        ownerWizard = ownerTransform.GetComponent<SkeletonWizard>();
    }

    public void SetAttackRange(float attackRange)
    {
        this.attackRange = attackRange;
    }

    private void Update()
    {
        curLife += Time.deltaTime;
        if (curLife >= lifeTime)
        {
            curLife = 0;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Company"))
        {
            areaAttackParticle.Play();
            ownerWizard.ApplyAreaDamage(transform.position, attackRange);
            rb.linearVelocity = Vector3.zero;
            Invoke(nameof(OnDisableEnergyBall), 0.6f);
        }
    }

    private void OnDisableEnergyBall()
    {
        gameObject.SetActive(false);
        enemySpawnManager.EnqueueEnergyBall(gameObject);
    }
}
