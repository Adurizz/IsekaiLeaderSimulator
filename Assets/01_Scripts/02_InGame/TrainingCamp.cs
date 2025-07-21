using System;
using System.Collections.Generic;
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

        // 레벨 5 이하인 파티 멤버중 하나 랜덤 픽
        int randomMemberIndex;
        do
        {
            randomMemberIndex = UnityEngine.Random.Range(0, partyMemberList.Count);
        } while (partyMemberList[randomMemberIndex].GetLevel() >= GlobalValueHolder.maxCompanionLevel);
        Companion trainingTarget = partyMemberList[randomMemberIndex];
        Debug.Log("강화 대상: " + trainingTarget.name);

        int randomSkillIndex;
        do
        {
            randomSkillIndex = UnityEngine.Random.Range(0, 3);

        } while (CheckSkillInfoOccupied(trainingTarget.GetName(), randomSkillIndex) || trainingTarget.GetSkillLevels()[randomSkillIndex] >= 3);

        SkillInfo trainingTargetSkillInfo = GetSkillInfo(trainingTarget.GetName())[randomSkillIndex][trainingTarget.GetSkillLevels()[randomSkillIndex]];
        IngameUIManager.Instance.SetTrainingOptionPanel(index, trainingTarget.GetName(), trainingTargetSkillInfo);
    }
}
