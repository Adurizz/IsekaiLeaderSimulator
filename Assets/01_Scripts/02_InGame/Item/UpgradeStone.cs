using UnityEngine;

public class UpgradeStone : Item
{
    [SerializeField] private int acquireStoneAmount;
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        player = FindAnyObjectByType<Player>();
    }

    public void SetStoneAmount(int amount)
    {
        acquireStoneAmount = amount;
    }

    public override void OnAcquired()
    {
        player.EarnUpgradeStone(acquireStoneAmount);
    }
}
