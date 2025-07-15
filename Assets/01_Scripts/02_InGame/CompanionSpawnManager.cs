using System;
using System.Collections.Generic;
using UnityEngine;

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

    public void SpawnCompanion(EHeroClass companionClass)
    {
        GameObject spawnedCompanion = Instantiate(GetHeroPrefab(companionClass));
        spawnedCompanion.transform.localPosition = player.transform.localPosition + spawnOffset;
    }

    public void TryToHireCompanion(EHeroClass companionClass)
    {
        if (player.ConsumeGold(GlobalValueHolder.companionCost))
        {
            SpawnCompanion(companionClass);
        }
        else
        {
            Debug.Log("Not enough gold");
        }
    }
}
