using System;
using UnityEngine;

public class CampSpawnManager : MonoBehaviour
{
    [SerializeField] private SpawnObjPlacementManager objectPlacementManager;
    [SerializeField] private GameObject baseCampPrefab;
    private const int baseCampSize = 10;
    [SerializeField] private GameObject trainingCampPrefab;
    private const int trainingCampSize = 8;


    public void SpawnTrainingCamp()
    {
        objectPlacementManager.SpawnObject(trainingCampPrefab, trainingCampSize);
    }

    public void SpawnBaseCamp()
    {
        objectPlacementManager.SpawnObject(baseCampPrefab, 0, 8, baseCampSize);
    }
}
