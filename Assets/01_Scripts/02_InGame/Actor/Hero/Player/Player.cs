using NUnit.Framework.Interfaces;
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
    private Animator animator;

    [HideInInspector] public UnityEvent onStatInitiated = new();

    private void Awake()
    {
        moveHandler = GetComponent<PlayerMoveHandler>();
        attackHandler = GetComponent<PlayerAttackHandler>();
        animator = GetComponentInChildren<Animator>();
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

    public void EarnUpgradeStone(int amount)
    {
        playerStat.EarnStone(amount);
    }

    public bool ConsumeUpgradeStone(int amount)
    {
        return playerStat.ConsumeStone(amount);
    }

    public override void GetDamage(float damage, bool isKnockBack, Vector3 knockoutDir)
    {
        Debug.Log("Player Attacked");
        curHealth = Mathf.Clamp(curHealth - damage, 0, maxHealth);
        if (curHealth <= 0f)
        {
            // TODO: 사망 관련 처리
            OnDead();
            animator.SetTrigger("Dead");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            collision.gameObject.GetComponent<Item>().OnAcquired();
            Destroy(collision.gameObject);
        }
    }

    public override void OnDead()
    {
        isDead = true;
    }
}
