using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CompanionStatHolder
{
    public EHeroClass ownerClass;
    public List<CompanionStat> statList;
}

public class CompanionStatManager : MonoBehaviour
{
    [SerializeField] private List<CompanionStatHolder> companionStatHolders;
    private Dictionary<EHeroClass, List<CompanionStat>> companionStatDict = null;

    private void Awake()
    {
        InitCompanionStatDict();
    }

    private void InitCompanionStatDict()
    {
        companionStatDict = new();

        foreach (var item in companionStatHolders)
        {
            companionStatDict[item.ownerClass] = item.statList;
        }
    }

    public List<CompanionStat> GetCompanionStats(EHeroClass heroClass)
    {
        if (companionStatDict == null)
            InitCompanionStatDict();

        return companionStatDict[heroClass];
    }
}
