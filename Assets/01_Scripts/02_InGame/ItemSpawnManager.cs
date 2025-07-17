using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    public bool Test;
    [SerializeField] private GameObject treasureBoxPrefab;
    [SerializeField] private GameObject healPotionPrefab;
    [SerializeField] private List<GameObject> companionSpawnItemPrefabs = new();
    [SerializeField] private SpawnObjPlacementManager objectPlacementManager;

    private float curCompanionSpawnCool = 0f;
    private float maxCompanionSpawnCool = 100f;

    private void Start()
    {
        if (Test)
        {
            GameObject temp = companionSpawnItemPrefabs[Random.Range(0, companionSpawnItemPrefabs.Count)];
            objectPlacementManager.SpawnObject(temp, temp.GetComponent<Item>().GetSize());
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
}
