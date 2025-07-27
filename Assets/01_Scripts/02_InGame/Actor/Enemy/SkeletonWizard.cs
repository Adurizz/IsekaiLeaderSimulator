using UnityEngine;

public class SkeletonWizard : Enemy
{
    private void FireProjectile()
    {
        GameObject energyBall = spawnManager.GetEnergyBall();
        EnemyEnergyBallController energyBallController = energyBall.GetComponent<EnemyEnergyBallController>();
        energyBallController.SetOwnerTransform(transform);
        energyBallController.SetTargetTransform(Target.transform);
        energyBallController.SetAttackRange(attackRange);
        energyBall.SetActive(true);
    }

    public void ApplyAreaDamage(Vector3 position, float range, float damageMultiplier = 1f)
    {
        Collider[] colliders = Physics.OverlapSphere(position, range, heroLayer);

        foreach (Collider col in colliders)
        {
            if (col.gameObject.GetComponent<Actor>().IsDead)
                continue;

            col.gameObject.GetComponent<Actor>().GetDamage(attack * damageMultiplier);
        }
    }
}
