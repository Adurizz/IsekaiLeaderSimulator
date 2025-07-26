using System.Collections.Generic;
using UnityEngine;

public class SelfFix : Skill
{
    private Mechanic mechanicScript;
    [SerializeField] private bool mastered;
    private float curTime;
    private float healCoolTime = 1f;
    [SerializeField] private float healAmount = 5f;
    [SerializeField] private GameObject selfFixEffect;
    [SerializeField] private float selfFixRange;

    protected override void Awake()
    {
        base.Awake();
        mechanicScript = companionScript as Mechanic;
        mastered = false;
    }

    protected override void SetSkillID()
    {
        skillID = (int)EMechanicSkill.SelfFix;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        transform.localRotation = transform.rotation;
        // Gizmos.DrawWireSphere(Utils.GetCenter(transform) + attackPos, attackRange);
        Gizmos.DrawWireSphere(transform.position, selfFixRange);
    }
#endif

    protected override void AdjustSkillLevel(List<int> skillLevels)
    {
        base.AdjustSkillLevel(skillLevels);

        if (level >= 1)
        {
            companionScript.UpgradeMaxHP(skillAdjustAmount[0]);
        }
        if (level >= 2)
        {
            companionScript.UpgradeMaxHP(skillAdjustAmount[1]);
        }
        if (level >= 3)
        {
            companionScript.UpgradeMaxHP(skillAdjustAmount[2]);
            mechanicScript.MakeSelfFix();
            selfFixEffect.SetActive(true);
            mastered = true;
        }
    }

    private void Update()
    {
        if (!mastered)
            return;
        GiveAreaHealWithInterval();
    }

    public void DeactivateSelfFixEffect()
    {
        selfFixEffect.SetActive(false);
    }

    private void GiveAreaHealWithInterval()
    {
        curTime += Time.deltaTime;
        if (curTime >= healCoolTime)
        {
            AreaHeal();
            curTime = 0;
        }
    }

    private void AreaHeal()
    {
        List<Cannon> nearbyCannonList = FindAllNearbyCannon();

        foreach (Cannon cannon in nearbyCannonList)
        {
            cannon.GetHeal(healAmount);
            cannon.GotHealedEffect.Play();
        }
        companionScript.GetHeal(healAmount);
        companionScript.GotHealedEffect.Play();
    }

    private List<Cannon> FindAllNearbyCannon()
    {
        List<Cannon> temp = new();
        Collider[] colliders = Physics.OverlapSphere(transform.position, selfFixRange, heroLayer);
        if (colliders.Length == 0)
        {
            return null;
        }

        foreach (Collider col in colliders)
        {
            if (col.GetComponent<Cannon>() == null)
                continue;

            Cannon nearbyCannon = col.GetComponent<Cannon>();
            if (nearbyCannon.IsDead)
                continue;

            temp.Add(nearbyCannon);
        }

        return temp;
    }
}
