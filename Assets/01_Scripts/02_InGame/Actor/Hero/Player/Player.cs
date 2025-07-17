using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerMoveHandler), typeof(PlayerAttackHandler))]
public class Player : Hero
{
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private int trainingStoneAmount;
    [SerializeField] private int rerollChance;
    private PlayerMoveHandler moveHandler;
    private PlayerAttackHandler attackHandler;

    [HideInInspector] public UnityEvent onStatInitiated = new();

    private void Awake()
    {
        moveHandler = GetComponent<PlayerMoveHandler>();
        attackHandler = GetComponent<PlayerAttackHandler>();
    }

    public override void InitStat()
    {
        base.InitStat();
        List<int> upgradeInfo = playerStat.GetUpgradeInfo();
        maxHealth += upgradeInfo[0];
        curHealth = maxHealth;
        attack += upgradeInfo[1];
        attackSpeed += upgradeInfo[2];
        attackDistance += upgradeInfo[3];
        attackRange += upgradeInfo[4];
        moveSpeed += upgradeInfo[5];
        rerollChance += upgradeInfo[6];

        trainingStoneAmount = 0;

        moveHandler.SetMoveSpeed(moveSpeed);
        attackHandler.SetAttackDistance(attackDistance);
        attackHandler.SetFireCool(attackSpeed);

        onStatInitiated.Invoke();
    }

    public void EarnGold(int amount)
    {
        playerStat.EarnGold(amount);
    }

    public bool ConsumeGold(int amount)
    {
        return playerStat.ConsumeGold(amount);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            collision.gameObject.GetComponent<Item>().OnAcquired();
            collision.gameObject.SetActive(false);
        }
    }

    public override void OnDead()
    {
        
    }
}
