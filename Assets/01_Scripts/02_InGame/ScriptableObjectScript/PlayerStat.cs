using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStat", menuName = "ActorStat/PlayerStat")]
public class PlayerStat : ActorStat
{
    [Header("플레이어 고유 스탯")]
    [SerializeField] private int gold;

    [SerializeField] private int maxHealthUpgradeLevel;
    [SerializeField] private int attackUpgradeLevel;
    [SerializeField] private int attackSpeedUpgradeLevel;
    [SerializeField] private int attackDistanceUpgradeLevel;
    [SerializeField] private int attackRangeUpgradeLevel;
    [SerializeField] private int moveSpeedUpgradeLevel;
    [SerializeField] private int rerollUpgradeLevel;

    /// <summary>
    /// 
    /// </summary>
    /// <returns>0: maxHealthUpgradeLevel, 1: attackUpgradeLevel, 2: attackSpeedUpgradeLevel, 3: attackDistanceUpgradeLevel, 4: attackRangeUpgradeLevel, 5: moveSpeedUpgradeLevel, 6: rerollUpgradeLevel</returns>
    public List<int> GetUpgradeInfo()
    {
        List<int> temp = new List<int>()
        {
            maxHealthUpgradeLevel,
            attackUpgradeLevel, attackSpeedUpgradeLevel, attackDistanceUpgradeLevel, attackRangeUpgradeLevel, 
            moveSpeedUpgradeLevel, rerollUpgradeLevel
        };
        return temp;
    }

    private void AdjustGoldAmount(int amount)
    {
        gold += amount;
    }

    public bool ConsumeGold(int amount)
    {
        if (gold > amount)
        {
            AdjustGoldAmount(-amount);
            return true;
        }
        else
            return false;
    }

    public void EarnGold(int amount)
    {
        AdjustGoldAmount(amount);
    }
}
