using UnityEngine;

public class SpawnObjPlacementManager : MonoBehaviour
{
    private bool[,] occupied = new bool[GlobalValueHolder.mapSize, GlobalValueHolder.mapSize];

    public bool CheckCanPlaceObject(int x, int z, int size)
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
                    return false;
                if (occupied[nx, nz])
                    return false;
            }
        }
        return true;
    }

    void RegisterItem(int x, int z, int size)
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

}
