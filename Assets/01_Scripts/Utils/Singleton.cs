using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance != null)
                return instance;
            else
                return null;
        }
        private set
        {
            instance = value;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
            instance = this as T;
    }
}

