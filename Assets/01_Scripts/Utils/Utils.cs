using UnityEngine;

public static class Utils
{
    public static Vector3 GetCenter(Transform objectTransform)
    {
        return new Vector3(objectTransform.localPosition.x, objectTransform.localPosition.y + objectTransform.localScale.y, objectTransform.localPosition.z);
    }
}
