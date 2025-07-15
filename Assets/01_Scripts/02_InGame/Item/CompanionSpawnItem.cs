using UnityEngine;

public class CompanionSpawnItem : Item
{
    [SerializeField] private EHeroClass companionClass;
    private CompanionSpawnManager companionSpawnManager;

    private void Awake()
    {
        companionSpawnManager = FindAnyObjectByType<CompanionSpawnManager>();
    }

    public override void OnAcquired()
    {
        companionSpawnManager.SpawnCompanion(companionClass);
    }
}
