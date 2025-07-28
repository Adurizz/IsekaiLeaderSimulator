using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PropHolder
{
    public GameObject propPrefab;
    public int propQuantity;
}
public class EnvironmentPropManager : MonoBehaviour
{
    [SerializeField] private List<PropHolder> propHolderList = new();
    
    public void LoadPropObjects()
    {
        foreach (var propHolder in propHolderList)
        {
            GameObject tempHolder = new GameObject(propHolder.propPrefab.name + "Pool");
            tempHolder.transform.SetParent(GameObject.FindWithTag("MapPrefab").transform);
            for (int i = 0; i < propHolder.propQuantity; ++i)
            {
                GameObject prop = Instantiate(propHolder.propPrefab);
                prop.transform.SetParent(tempHolder.transform);
                float x = UnityEngine.Random.Range(-240f, 240f);
                float z = UnityEngine.Random.Range(-240f, 240f);
                prop.transform.position = new Vector3(x, prop.transform.position.y, z);
            }
        }
    }
}
