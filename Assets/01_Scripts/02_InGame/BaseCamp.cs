using UnityEngine;

public class BaseCamp : MonoBehaviour
{
    private ExpeditionTimeChecker timeChecker;
    private ExpeditionManager expeditionManager;

    private void Awake()
    {
        timeChecker = FindAnyObjectByType<ExpeditionTimeChecker>();
        expeditionManager = FindAnyObjectByType<ExpeditionManager>();
    }

    public void OnReachBaseCamp()
    {
        expeditionManager.OnReachedBaseCamp();
        expeditionManager.StopExpedition();
    }
}
