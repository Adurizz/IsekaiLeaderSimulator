using UnityEngine;

public class EnemySoldier : Enemy
{
    public void OnEnable()
    {
        InitStat();
    }

    protected override void Attack()
    {
        throw new System.NotImplementedException();
    }
}
