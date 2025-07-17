using TMPro;
using UnityEngine;

public class CompanionSpawnItem : Item
{
    [SerializeField] private EHeroClass companionClass;
    [SerializeField] private GameObject companionHirePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI classText;
    [SerializeField] private TextMeshProUGUI costText;
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
