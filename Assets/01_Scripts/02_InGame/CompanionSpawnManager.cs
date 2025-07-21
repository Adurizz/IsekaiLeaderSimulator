using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;
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
    private PartyManager partyManager;
    private List<string> nameList = new();
    private Dictionary<string, bool> nameOccupiedDict = new();

    private void Awake()
    {
        partyManager = FindAnyObjectByType<PartyManager>();
        InitNameDict();
    }

    private void InitHeroPrefabDict()
    {
        companionPrefabDict = new();

        foreach (var item in companionPrefabList)
        {
            companionPrefabDict[item.companionClass] = item.companionPrefab;
        }
    }

    private void InitNameDict()
    {
        // List 초기화
        var list = CSVReader.Read("Companions_Name");
        foreach (var item in list)
        {
            nameList.Add(item["Name"].ToString());
        }

        foreach (string name in nameList)
        {
            nameOccupiedDict[name] = false;
        }
    }

    public string GetUnoccupiedRandomName()
    {
        if (nameOccupiedDict == null)
            InitNameDict();

        int randIdx = UnityEngine.Random.Range(0, nameList.Count);
        string name;
        do
        {
            name = nameList[randIdx];
        }
        while (CheckNameOccupied(name));

        // 점유 처리
        nameOccupiedDict[name] = true;
        return name;
    }

    bool CheckNameOccupied(string name)
    {
        return nameOccupiedDict[name];
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
        spawnedCompanion.GetComponent<Companion>().SetName(GetUnoccupiedRandomName());

        partyManager.RegisterPartyMember(spawnedCompanion.GetComponent<Companion>());
    }

    public void SpawnCompanion(EHeroClass companionClass)
    {
        GameObject spawnedCompanion = Instantiate(GetHeroPrefab(companionClass));
        spawnedCompanion.transform.localPosition = player.transform.localPosition + spawnOffset;

        partyManager.RegisterPartyMember(spawnedCompanion.GetComponent<Companion>());
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
