using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct HeroPrefabHolder
{
    public EHeroClass companionClass;
    public GameObject companionPrefab;
}

public class CompanionSpawnManager : MonoBehaviour
{
    [SerializeField] private List<HeroPrefabHolder> companionPrefabList = new();
    private Dictionary<EHeroClass, GameObject> companionPrefabDict = null;
    private Vector3 spawnOffset = new(0, 0, -8);
    [SerializeField] private Player player;
    [SerializeField] private EHeroClass curSpawnTarget;

    private void InitHeroPrefabDict()
    {
        companionPrefabDict = new();

        foreach (var item in companionPrefabList)
        {
            companionPrefabDict[item.companionClass] = item.companionPrefab;
        }
    }

    private GameObject GetHeroPrefab(EHeroClass companionClass)
    {
        if (companionPrefabDict == null)
            InitHeroPrefabDict();

        return companionPrefabDict[companionClass];
    }

    public void SetSpawnTarget(EHeroClass spawnTargetClass)
    {
        curSpawnTarget = spawnTargetClass;
    }

    public void SpawnCompanion()
    {
        GameObject spawnedCompanion = Instantiate(GetHeroPrefab(curSpawnTarget));
        spawnedCompanion.transform.localPosition = player.transform.localPosition + spawnOffset;
    }

    public void SpawnCompanion(EHeroClass companionClass)
    {
        GameObject spawnedCompanion = Instantiate(GetHeroPrefab(companionClass));
        spawnedCompanion.transform.localPosition = player.transform.localPosition + spawnOffset;
    }
    
    /// <summary>
    /// CompanyHirePanel 예 버튼에 바인딩
    /// </summary>
    public void TryToHireCompanion()
    {
        if (player.ConsumeGold(GlobalValueHolder.companionHireCost))
        {
            SpawnCompanion();
        }
        else
        {
            Debug.Log("Not enough gold");
        }
    }
}
