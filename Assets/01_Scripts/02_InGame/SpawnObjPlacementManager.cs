using System;
using UnityEngine;

public class SpawnObjPlacementManager : MonoBehaviour
{
    private bool[,] occupied = new bool[GlobalValueHolder.mapSize, GlobalValueHolder.mapSize];

    public void SpawnObject(GameObject objectPrefab, int size)
    {
        int xPos;
        int zPos;
        do
        {
            Tuple<int, int> temp = GetRandomSpawnPoint();
            xPos = temp.Item1;
            zPos = temp.Item2;
        }
        while (!CheckCanPlaceObject(xPos, zPos, size));

        GameObject spawnedObj = Instantiate(objectPrefab);
        spawnedObj.transform.SetParent(GameObject.FindWithTag("MapPrefab").transform);
        spawnedObj.transform.localPosition = new Vector3(xPos, spawnedObj.transform.position.y, zPos);
        RegisterItem(xPos, zPos, size);
    }

    public bool SpawnObject(GameObject objectPrefab, int xPos, int zPos, int size)
    {
        if (CheckCanPlaceObject(xPos, zPos, size))
        {
            GameObject spawnedObj = Instantiate(objectPrefab);
            spawnedObj.transform.SetParent(GameObject.FindWithTag("MapPrefab").transform);
            spawnedObj.transform.localPosition = new Vector3(xPos, 0, zPos);
            RegisterItem(xPos, zPos, size);

            return true;
        }
        else
        {
            Debug.Log("Cant Spawn: " + objectPrefab.name);
            return false;
        }
    }

    private Tuple<int, int> GetRandomSpawnPoint()
    {
        int x = UnityEngine.Random.Range(GlobalValueHolder.minMapIdx, GlobalValueHolder.maxMapIdx);
        int z = UnityEngine.Random.Range(GlobalValueHolder.minMapIdx, GlobalValueHolder.maxMapIdx);

        return new Tuple<int, int>(x, z);
    }

    private bool CheckCanPlaceObject(int x, int z, int size)
    {
        int xIndex = x - GlobalValueHolder.minMapIdx;
        int zIndex = z - GlobalValueHolder.minMapIdx;

        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < size; ++j)
            {
                int nx = xIndex + i;
                int nz = zIndex + j;
                // 범위 체크
                if (nx < 0 || nx >= GlobalValueHolder.mapSize || nz < 0 || nz >= GlobalValueHolder.mapSize)
                {
                    Debug.Log("Out Of Range");
                    return false;
                }
                if (occupied[nx, nz])
                {
                    Debug.Log("Occupied");
                    return false;
                }
            }
        }

        return true;
    }

    private void RegisterItem(int x, int z, int size)
    {
        int xIndex = x - GlobalValueHolder.minMapIdx;
        int zIndex = z - GlobalValueHolder.minMapIdx;

        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < size; ++j)
            {
                int nx = xIndex + i;
                int nz = zIndex + j;
                occupied[nx, nz] = true;
            }
        }
    }

    public void UnregisterItem(int x, int z, int size)
    {
        int xIndex = x - GlobalValueHolder.minMapIdx;
        int zIndex = z - GlobalValueHolder.minMapIdx;

        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < size; ++j)
            {
                int nx = xIndex + i;
                int nz = zIndex + j;
                occupied[nx, nz] = false;
            }
        }
    }
}
