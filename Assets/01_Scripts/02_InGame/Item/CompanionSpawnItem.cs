using UnityEngine;

public class CompanionSpawnItem : Item
{
    [SerializeField] private EHeroClass companionClass;
    private CompanionSpawnManager companionSpawnManager;

    protected override void Awake()
    {
        base.Awake();
        companionSpawnManager = FindAnyObjectByType<CompanionSpawnManager>();
    }

    public override void OnAcquired()
    {
        companionSpawnManager.SpawnCompanion(companionClass);
        base.OnAcquired();
    }
}
