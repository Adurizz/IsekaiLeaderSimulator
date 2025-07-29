using UnityEngine;

public class MechanicAreaFire : MonoBehaviour
{
    private Mechanic ownerMechanic;
    private float passedTime;
    private float maxDamageCool = 0.1f;
    [SerializeField] private float curLife;
    [SerializeField] private float maxLifeTime = 3f;
    [SerializeField] private float attackDistance;
    LayerMask enemyLayer;

    public void SetOwner(Mechanic owner)
    {
        ownerMechanic = owner;
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    private void Update()
    {
        ApplyDamageWithInterval();
        CheckLifetime();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
#endif

    private void ApplyDamageWithInterval()
    {
        passedTime += Time.deltaTime;
        if (passedTime > maxDamageCool) 
        {
            ApplyAreaDamage(transform.position, attackDistance, 0.1f);
            passedTime = 0;
        }
    }

    private void CheckLifetime()
    {
        curLife += Time.deltaTime;
        if (curLife > maxLifeTime)
        {
            ownerMechanic.EnqueueAreaFireOnDisable(gameObject);
            gameObject.SetActive(false);
            curLife = 0;
        }
    }

    public void ApplyAreaDamage(Vector3 position, float range, float damageMultiplier = 1f)
    {
        Collider[] colliders = Physics.OverlapSphere(position, range, enemyLayer);

        foreach (Collider col in colliders)
        {
            if (col.gameObject.GetComponent<Enemy>().IsDead)
                continue;

            col.gameObject.GetComponent<Actor>().GetDamage(ownerMechanic.Attack * damageMultiplier);
        }
    }
}
