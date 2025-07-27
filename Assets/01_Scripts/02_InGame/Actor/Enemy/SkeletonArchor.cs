using UnityEngine;

public class SkeletonArchor : Enemy
{
    private void FireProjectile()
    {
        GameObject arrow = spawnManager.GetEnemyArrow();
        EnemyArrowController arrowController = arrow.GetComponent<EnemyArrowController>();
        arrowController.SetOwnerTransform(transform);
        arrowController.SetTargetTransform(Target.transform);
        arrow.SetActive(true);
    }
}
