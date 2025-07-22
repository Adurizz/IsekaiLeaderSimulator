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
                skillInfoOccupiedDict[partyMember.GetName()][i] = false;

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
        List<List<SkillInfo>> skillInfos = skillInfoDictData[newCompanion.GetClass()];
        skillInfoDict[newCompanion.GetName()] = skillInfos;

        List<bool> skillInfoOccupiedDict = new List<bool>() { false, false, false };
        this.skillInfoOccupiedDict[newCompanion.GetName()] = skillInfoOccupiedDict;

        Debug.Log("새 skillinfoDict 크기: " + skillInfoDict.Count);
        Debug.Log("새 skillInfooccupiedDict 크기: " + this.skillInfoOccupiedDict.Count);
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

        int count1 = 0;
        // 레벨 5 이하인 파티 멤버중 하나 랜덤 픽
        int randomMemberIndex;
        do
        {
            randomMemberIndex = UnityEngine.Random.Range(0, partyMemberList.Count);
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
        } while (partyMemberList[randomMemberIndex].GetLevel() >= GlobalValueHolder.maxCompanionLevel);
        Companion trainingTarget = partyMemberList[randomMemberIndex];
        Debug.Log("강화 대상: " + trainingTarget.name);

        int count2 = 0;
        int randomSkillIndex;
        do
        {
            randomSkillIndex = UnityEngine.Random.Range(0, 3);
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
        } while (CheckSkillInfoOccupied(trainingTarget.GetName(), randomSkillIndex) || trainingTarget.GetSkillLevels()[randomSkillIndex] >= 3);

        skillInfoOccupiedDict[trainingTarget.GetName()][randomSkillIndex] = true;

        SkillInfo trainingTargetSkillInfo = GetSkillInfo(trainingTarget.GetName())[randomSkillIndex][trainingTarget.GetSkillLevels()[randomSkillIndex]];
        IngameUIManager.Instance.SetTrainingOptionPanel(index, trainingTarget.GetName(), trainingTargetSkillInfo);

        curSelectedCompanions[index] = trainingTarget;
        curSelectedSkillIndexes[index] = randomSkillIndex;
    }

    private SkillInfo GetRandomDefaultSkill()
    {
        int randomDefaultSkillIdx = UnityEngine.Random.Range(0, 3);
        SkillInfo defaultSkillInfo = GetSkillInfoData(EHeroClass.Player)[randomDefaultSkillIdx][0];

        return defaultSkillInfo;
    }

    public void UpgradeTarget(int index)
    {
        if (curSelectedCompanions[index] == null)
        {
            Debug.Log(index + "기본 스킬 적용");
            return;
        }

        curSelectedCompanions[index].LevelUp(curSelectedSkillIndexes[index]);
    }
}
