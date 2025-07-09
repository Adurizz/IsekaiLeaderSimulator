using System;
using UnityEngine;

public class CampSpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject baseCampPrefab;
    [SerializeField] private GameObject trainingCampPrefab;

    public void SpawnTrainingCamp()
    {

    }

    Tuple<int, int> GetRandomSpawnPoint()
    {
        int x = UnityEngine.Random.Range(GlobalValueHolder.minMapIdx, GlobalValueHolder.maxMapIdx);
        int z = UnityEngine.Random.Range(GlobalValueHolder.minMapIdx, GlobalValueHolder.maxMapIdx);

        return new Tuple<int, int>(x, z);
    }
}
