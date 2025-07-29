using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMoveHandler), typeof(PlayerAttackHandler))]
public class Player : Hero
{
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private int trainingStoneAmount;
    [SerializeField] private int rerollChance;
    private PlayerMoveHandler moveHandler;
    private PlayerAttackHandler attackHandler;
    private Animator animator;
    [SerializeField] private Image playerGotDamagedImg;
    [HideInInspector] public UnityEvent onStatInitiated = new();
    private bool damagedEffectCorStarted;
    private int damagedEffectCorUpdateTimes = 100;
    private float damagedEffectCorUpdatePeriod = 2f;

    protected override void Awake()
    {
        base.Awake();
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

    public int GetCurHaveGoldAmount()
    {
        return playerStat.GetCurHaveGoldAmount();
    }

    public void EarnUpgradeStone(int amount)
    {
        playerStat.EarnStone(amount);
    }

    public void ResetStone()
    {
        playerStat.ResetStone();
    }

    public bool ConsumeUpgradeStone(int amount)
    {
        return playerStat.ConsumeStone(amount);
    }

    public override bool GetDamage(float damage, bool isKnockBack, Vector3 knockoutDir)
    {
        Debug.Log("Player Attacked");
        curHealth = Mathf.Clamp(curHealth - damage, 0, maxHealth);
        getDamageEffect.Play();
        UpdateHPBar();
        if (damagedEffectCorStarted)
            playerGotDamagedImg.color = Color.white;
        else
            StartCoroutine(GotDamagedImageEffectCor());
        if (curHealth <= 0f)
        {
            // TODO: 사망 관련 처리
            OnDead();
            animator.SetTrigger("Dead");
            return true;
        }
        return false;
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
        DestroyHPBar();
        Invoke(nameof(ProcessDeadWithExpeditionManager), 3f);
    }

    private void ProcessDeadWithExpeditionManager()
    {
        ExpeditionManager expeditionManager = FindAnyObjectByType<ExpeditionManager>();
        expeditionManager.OnPlayerDied();
    }

    private IEnumerator GotDamagedImageEffectCor()
    {
        damagedEffectCorStarted = true;
        playerGotDamagedImg.color = Color.white;

        while (playerGotDamagedImg.color.a > 0)
        {
            float tempAlpha = playerGotDamagedImg.color.a - (float)1 / damagedEffectCorUpdateTimes;
            playerGotDamagedImg.color = new Color(1, 1, 1, tempAlpha);
            yield return new WaitForSeconds(damagedEffectCorUpdatePeriod / damagedEffectCorUpdateTimes);
        }
        damagedEffectCorStarted = false;
    }
}
