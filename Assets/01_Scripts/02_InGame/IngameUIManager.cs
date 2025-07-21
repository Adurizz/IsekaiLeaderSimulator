using UnityEngine;

public class IngameUIManager : Singleton<IngameUIManager>
{
    [SerializeField] private GameObject companionHirePanel;
    private ExpeditionManager expeditionManager;

    protected override void Awake()
    {
        base.Awake();
        expeditionManager = FindAnyObjectByType<ExpeditionManager>();
    }

    public void ActivateCompanionHireUI()
    {
        companionHirePanel.SetActive(true);
        expeditionManager.StopExpedition();
    }
}
