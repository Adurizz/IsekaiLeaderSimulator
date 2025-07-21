using UnityEngine;

public class Gold : Item
{
    [SerializeField] private int acquireGoldAmount;
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        player = FindAnyObjectByType<Player>();
    }

    public void SetGoldAmount(int amount)
    {
        acquireGoldAmount = amount;
    }

    public override void OnAcquired()
    {
        player.EarnGold(acquireGoldAmount);
    }
}
