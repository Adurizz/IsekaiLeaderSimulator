using UnityEngine;

public class CompanionSpawnItem : Item
{
    [SerializeField] private EHeroClass companionClass;
    private CompanionSpawnManager companionSpawnManager;
    private Animator animator;
    private float curTime;
    private const float hiTime = 5f;

    protected override void Awake()
    {
        base.Awake();
        companionSpawnManager = FindAnyObjectByType<CompanionSpawnManager>();
        animator = GetComponent<Animator>();
    }

    public override void OnAcquired()
    {
        companionSpawnManager.SpawnCompanion(companionClass);
        base.OnAcquired();
    }

    private void Update()
    {
        SayHi();
    }

    private void SayHi()
    {
        curTime += Time.deltaTime;
        if (curTime >= hiTime)
        {
            animator.SetTrigger("Hi");
            curTime = 0f;
        }
    }
}
