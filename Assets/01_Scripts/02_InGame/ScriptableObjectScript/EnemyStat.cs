using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStat", menuName = "ActorStat/EnemyStat")]
public class EnemyStat : ActorStat
{
    [Header("드랍 재화")]
    [SerializeField] private int dropGold;
    [SerializeField] private int dropUpgradeStone;

    /// <summary>
    /// dropGold, dropUpgradeStone
    /// </summary>
    /// <returns></returns>
    public int[] GetRewardInfo()
    {
        return new int[] { dropGold, dropUpgradeStone };
    }
}
