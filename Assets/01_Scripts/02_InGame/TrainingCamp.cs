using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct SkillInfoHolder
{
    public EHeroClass ownerClass;
    public List<SkillInfo> firstSkillInfoList;
    public List<SkillInfo> secondSkillInfoList;
    public List<SkillInfo> thirdSkillInfoList;
}

public class TrainingCamp : MonoBehaviour
{
    private Player player;
    [SerializeField] private List<SkillInfoHolder> skillInfoHolders;
    private PartyManager partyManager;
    private Dictionary<EHeroClass, List<List<SkillInfo>>> skillInfoDictData;

    private Dictionary<string, List<List<SkillInfo>>> skillInfoDict;
    private Dictionary<string, List<bool>> skillInfoOccupiedDict;
    private ExpeditionManager expeditionManager;

    [SerializeField] private Companion[] curSelectedCompanions = new Companion[3];
    [SerializeField] private int[] curSelectedSkillIndexes = new int[3];

    private void Awake()
    {
        partyManager = FindAnyObjectByType<PartyManager>();
        expeditionManager = FindAnyObjectByType<ExpeditionManager>();
        InitSkillInfoDictData();
    }

    private void InitSkillInfoDictData()
    {
        skillInfoDictData = new();

        skillInfoDict = new();
        skillInfoOccupiedDict = new();

        foreach (var skillInfoHolder in skillInfoHolders)
        {
            List<List<SkillInfo>> temp = new List<List<SkillInfo>>()
            {
                skillInfoHolder.firstSkillInfoList,
                skillInfoHolder.secondSkillInfoList,
                skillInfoHolder.thirdSkillInfoList
            };

            skillInfoDictData[skillInfoHolder.ownerClass] = temp;
        }
    }

    public void InitWholeSkillInfoOccupiedState()
    {
        foreach (var partyMember in partyManager.GetWholePartyMember())
        {
            for (int i = 0; i < 3; ++i)
            {
                skillInfoOccupiedDict[partyMember.ActorName][i] = false;

            }
        }
    }

    private List<List<SkillInfo>> GetSkillInfoData(EHeroClass ownerClass)
    {
        return skillInfoDictData[ownerClass];
    }

    private List<List<SkillInfo>> GetSkillInfo(string ownerName)
    {
        return skillInfoDict[ownerName];
    }

    public void RegisterCompanionToTrainingCamp(Companion newCompanion)
    {
        // 개인 SkillInfoDict
        List<List<SkillInfo>> skillInfos = skillInfoDictData[newCompanion.HeroClass];
        skillInfoDict[newCompanion.ActorName] = skillInfos;

        List<bool> skillInfoOccupiedList = new List<bool>() { false, false, false };
        skillInfoOccupiedDict[newCompanion.ActorName] = skillInfoOccupiedList;

        Debug.Log("새 skillinfoDict 크기: " + skillInfoDict.Count);
        Debug.Log("새 skillInfooccupiedDict 크기: " + skillInfoOccupiedDict.Count);
    }

    public void UnregisterCompanionFromTrainingCamp(Companion deadCompanion)
    {
        skillInfoDict.Remove(deadCompanion.ActorName);
        skillInfoOccupiedDict.Remove(deadCompanion.ActorName);
    }

    private bool CheckSkillInfoOccupied(string owner, int skillIdx)
    {
        return skillInfoOccupiedDict[owner][skillIdx];
    }

    public void OnReachTrainingCamp()
    {
        SetTrainingMenu();
        IngameUIManager.Instance.ActivateTrainingCampUI();
    }

    void SetTrainingMenu()
    {
        for (int i = 0; i < 3; ++i)
        {
            GetRandomTrainingMenu(i);
        }
    }

    public void GetRandomTrainingMenu(int index)
    {
        List<Companion> partyMemberList = partyManager.GetWholePartyMember();

        if (partyMemberList.Count <= 0)
        {
            SkillInfo defaultSkilInfo = GetRandomDefaultSkill();
            switch (defaultSkilInfo.skillID)
            {
                case 0:
                    // HP 회복
                    break;
                case 1:
                    // HP 회복 2
                    break;
                case 2:
                    // HP 회복 3
                    break;
            }
            IngameUIManager.Instance.SetTrainingOptionPanel(index, "플레이어", defaultSkilInfo);
            curSelectedCompanions[index] = null;
            return;
        }

        int count1 = 0;
        // 레벨 5 이하인 파티 멤버중 하나 랜덤 픽
        int randomMemberIndex;
        do
        {
            randomMemberIndex = UnityEngine.Random.Range(0, partyMemberList.Count);
            #region 10번동안 유효한 강화 대상 못찾으면 그냥 랜덤 기본스킬 노출
            ++count1;
            if (count1 >= 10)
            {
                SkillInfo defaultSkilInfo = GetRandomDefaultSkill();
                switch (defaultSkilInfo.skillID)
                {
                    case 0:
                        // HP 회복
                        break;
                    case 1:
                        // HP 회복 2
                        break;
                    case 2:
                        // HP 회복 3
                        break;
                }
                IngameUIManager.Instance.SetTrainingOptionPanel(index, "플레이어", defaultSkilInfo);
                curSelectedCompanions[index] = null;
                return;
            }
            #endregion
        } while (partyMemberList[randomMemberIndex].Level >= GlobalValueHolder.maxCompanionLevel);
        Companion trainingTarget = partyMemberList[randomMemberIndex];
        Debug.Log("강화 대상: " + trainingTarget.name);

        // 해당 대상의 랜덤 스킬 픽
        int count2 = 0;
        int randomSkillIndex;
        do
        {
            randomSkillIndex = UnityEngine.Random.Range(0, 3);
            #region 10번동안 유효한 강화 대상 못찾으면 그냥 랜덤 기본스킬 노출
            ++count2;
            if (count2 >= 10)
            {
                SkillInfo defaultSkilInfo = GetRandomDefaultSkill();
                switch (defaultSkilInfo.skillID)
                {
                    case 0:
                        // HP 회복
                        break;
                    case 1:
                        // HP 회복 2
                        break;
                    case 2:
                        // HP 회복 3
                        break;
                }
                IngameUIManager.Instance.SetTrainingOptionPanel(index, "플레이어", defaultSkilInfo);
                curSelectedCompanions[index] = null;
                return;
            }
            #endregion
        } while (CheckSkillInfoOccupied(trainingTarget.ActorName, randomSkillIndex) || trainingTarget.SkillLevels[randomSkillIndex] >= 3);

        // 해당 스킬 점유 처리
        skillInfoOccupiedDict[trainingTarget.ActorName][randomSkillIndex] = true;

        SkillInfo trainingTargetSkillInfo = GetSkillInfo(trainingTarget.ActorName)[randomSkillIndex][trainingTarget.SkillLevels[randomSkillIndex]];
        IngameUIManager.Instance.SetTrainingOptionPanel(index, trainingTarget.ActorName, trainingTargetSkillInfo);

        curSelectedCompanions[index] = trainingTarget;
        curSelectedSkillIndexes[index] = randomSkillIndex;
    }

    private SkillInfo GetRandomDefaultSkill()
    {
        int randomDefaultSkillIdx = UnityEngine.Random.Range(0, 3);
        SkillInfo defaultSkillInfo = GetSkillInfoData(EHeroClass.Player)[randomDefaultSkillIdx][0];

        return defaultSkillInfo;
    }

    /// <summary>
    /// 선택된 대상의 선택된 스킬 업그레이드
    /// </summary>
    /// <param name="index"></param>
    public void UpgradeTarget(int index)
    {
        if (curSelectedCompanions[index] == null)
        {
            Debug.Log(index + "기본 스킬 적용");
            return;
        }

        if (player == null)
            player = FindAnyObjectByType<Player>();

        if (player.ConsumeUpgradeStone(GlobalValueHolder.companyTrainCost))
        {
            curSelectedCompanions[index].LevelUp(curSelectedSkillIndexes[index]);
        }
        else
        {
            IngameUIManager.Instance.RevealNotEnoughStoneUI();
        }
    }
}
