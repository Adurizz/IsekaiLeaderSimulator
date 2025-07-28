using UnityEngine;

public class Gold : Item
{
    [SerializeField] private int acquireGoldAmount;
    private ExpeditionManager expeditionManager;

    protected override void Awake()
    {
        base.Awake();
        expeditionManager = FindAnyObjectByType<ExpeditionManager>();
    }

    public void SetGoldAmount(int amount)
    {
        acquireGoldAmount = amount;
    }

    public override void OnAcquired()
    {
        expeditionManager.EarnGold(acquireGoldAmount);
    }
}
