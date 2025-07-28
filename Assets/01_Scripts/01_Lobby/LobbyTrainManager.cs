using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum ETrainType
{
    Health, Attack, AttackSpeed, AttackDistance, MoveSpeed
}

public class LobbyTrainManager : MonoBehaviour
{
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private TextMeshProUGUI healthLevel;
    [SerializeField] private TextMeshProUGUI attackLevel;
    [SerializeField] private TextMeshProUGUI attackSpeedLevel;
    [SerializeField] private TextMeshProUGUI attackDistanceLevel;
    [SerializeField] private TextMeshProUGUI moveSpeedLevel;
    [SerializeField] private GameObject notEnoughGoldPanel;
    [SerializeField] private Jun_TweenRuntime notEnoughGoldTween1;
    [SerializeField] private Jun_TweenRuntime notEnoughGoldTween2;

    private void Start()
    {
        RefreshUpgradeLevels();
    }

    public void TrainPlayer(int trainType)
    {
        if (!playerStat.UpgradeStat(trainType))
        {
            notEnoughGoldPanel.SetActive(true);
            notEnoughGoldTween1.Play();
            notEnoughGoldTween2.Play();
            Invoke(nameof(DisableNotEnoughGoldPanel), 1.5f);
        }
        else
        {
            RefreshUpgradeLevels();
        }
    }

    private void DisableNotEnoughGoldPanel()
    {
        notEnoughGoldPanel.SetActive(false);
    }

    private void RefreshUpgradeLevels()
    {
        List<int> temp = playerStat.GetUpgradeInfo();
        healthLevel.text = "Lv." + temp[0];
        attackLevel.text = "Lv." + temp[1];
        attackSpeedLevel.text = "Lv." + temp[2];
        attackDistanceLevel.text = "Lv." + temp[3];
        moveSpeedLevel.text = "Lv." + temp[5];
    }
}
