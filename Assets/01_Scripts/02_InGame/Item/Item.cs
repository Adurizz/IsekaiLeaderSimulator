using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] protected SpawnObjPlacementManager objectPlacementManager;
    [SerializeField] protected int size;

    protected virtual void Awake()
    {
        objectPlacementManager = FindAnyObjectByType<SpawnObjPlacementManager>();
    }

    public virtual void OnAcquired()
    {
        objectPlacementManager.UnregisterItem((int)gameObject.transform.position.x, (int)gameObject.transform.position.z, size);
    }

    public int GetSize()
    {
        return size;
    }
}
