using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    public bool Test;
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private GameObject upgradeStonePrefab;
    [SerializeField] private GameObject treasureBoxPrefab;
    [SerializeField] private GameObject healPotionPrefab;
    [SerializeField] private List<GameObject> companionSpawnItemPrefabs = new();
    [SerializeField] private SpawnObjPlacementManager objectPlacementManager;

    private float curCompanionSpawnCool = 0f;
    private float maxCompanionSpawnCool = 100f;

    [SerializeField] private Vector3 goldSpawnOffset;
    [SerializeField] private Vector3 stoneSpawnOffset;

    private void Start()
    {
        if (Test)
        {
            foreach (var cp in companionSpawnItemPrefabs)
            {
                objectPlacementManager.SpawnObject(cp, cp.GetComponent<Item>().GetSize());
            }
            
        }
    }

    private void Update()
    {
        SpawnRandomCompanionSpawnItem();
    }

    private void SpawnRandomCompanionSpawnItem()
    {
        curCompanionSpawnCool += Time.deltaTime;
        if (curCompanionSpawnCool >= maxCompanionSpawnCool)
        {
            GameObject temp = companionSpawnItemPrefabs[Random.Range(0, companionSpawnItemPrefabs.Count)];
            objectPlacementManager.SpawnObject(temp, temp.GetComponent<Item>().GetSize());
            curCompanionSpawnCool = 0;
        }
    }

    public void SpawnGoldItem(int goldAmount, Transform spawnPos)
    {
        Vector3 pos = spawnPos.position + goldSpawnOffset;
        GameObject gold = Instantiate(goldPrefab, pos, goldPrefab.transform.rotation);
        gold.GetComponent<Gold>().SetGoldAmount(goldAmount);
    }

    public void SpawnUpgradeStoneItem(int stoneAmount, Transform spawnPos)
    {
        Vector3 pos = spawnPos.position + stoneSpawnOffset;
        GameObject stone = Instantiate(upgradeStonePrefab, pos, upgradeStonePrefab.transform.rotation);
        stone.GetComponent<UpgradeStone>().SetStoneAmount(stoneAmount);
    }
}
