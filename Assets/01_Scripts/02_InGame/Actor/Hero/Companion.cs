using UnityEngine;

public abstract class Companion : Hero
{
    [SerializeField] protected int level;

    public void LevelUp()
    {
        if (level < GlobalValueHolder.maxCompanionLevel)
        {
            level++;
            UpdateStat();
        }
    }

    protected void UpdateStat()
    {
        // TODO: 레벨업에 따른 스탯 업데이트
    }

    protected abstract void Move();
    protected abstract void Attack();
}
