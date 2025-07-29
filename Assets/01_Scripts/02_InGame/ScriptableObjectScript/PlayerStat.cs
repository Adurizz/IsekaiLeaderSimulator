using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "PlayerStat", menuName = "ActorStat/PlayerStat")]
public class PlayerStat : ActorStat
{
    [Header("플레이어 고유 스탯")]
    [SerializeField] private int maxHealthUpgradeLevel;
    [SerializeField] private int attackUpgradeLevel;
    [SerializeField] private int attackSpeedUpgradeLevel;
    [SerializeField] private int attackDistanceUpgradeLevel;
    [SerializeField] private int attackRangeUpgradeLevel;
    [SerializeField] private int moveSpeedUpgradeLevel;
    [SerializeField] private int rerollUpgradeLevel;
    [Header("재화")]
    [SerializeField] private int gold;
    [SerializeField] private int upgradeStone;
    public int UpgradeStone
    {
        get { return upgradeStone; }
        set
        {
            upgradeStone = value;
            IngameUIManager.Instance.SetStoneText(upgradeStone);
        }
    }

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

    public bool UpgradeStat(int trainType)
    {
        if (!ConsumeGold(GlobalValueHolder.upgradeCost))
            return false;

        switch (trainType)
        {
            case (int)ETrainType.Health:
                ++maxHealthUpgradeLevel;
                break;
            case (int)ETrainType.Attack:
                ++attackUpgradeLevel;
                break;
            case (int)ETrainType.AttackSpeed:
                ++attackSpeedUpgradeLevel;
                break;
            case (int)ETrainType.AttackDistance:
                ++attackDistanceUpgradeLevel;
                break;
            case (int)ETrainType.MoveSpeed:
                ++moveSpeedUpgradeLevel;
                break;
        }
        return true;
    }

    #region 골드 관련
    public int GetCurHaveGoldAmount()
    {
        return gold;
    }

    private void AdjustGoldAmount(int amount)
    {
        gold += amount;
        if (SceneManager.GetActiveScene().buildIndex == GlobalValueHolder.lobbySceneIndex)
        {

        }
        else if (SceneManager.GetActiveScene().buildIndex == GlobalValueHolder.ingameSceneIndex)
        {
            IngameUIManager.Instance.SetCurHaveGoldText(gold);
        }
    }

    public bool ConsumeGold(int amount)
    {
        if (gold >= amount)
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
    #endregion

    #region 스톤 관련
    private void AdjustStoneAmount(int amount)
    {
        UpgradeStone += amount;
    }

    public bool ConsumeStone(int amount)
    {
        if (UpgradeStone >= amount)
        {
            AdjustStoneAmount(-amount);
            return true;
        }
        else
            return false;
    }

    public void EarnStone(int amount)
    {
        AdjustStoneAmount(amount);
    }

    public void ResetStone()
    {
        UpgradeStone = 0;
    }
    #endregion
}
