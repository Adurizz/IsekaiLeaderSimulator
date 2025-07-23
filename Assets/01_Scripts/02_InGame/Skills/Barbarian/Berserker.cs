using System.Collections.Generic;
using UnityEngine;

public class Berserker : Skill
{
    private Barbarian barbarianScript;
    [SerializeField] private float attackReinforceAmount = 1f;
    public float AttackReinforceAmount { get { return attackReinforceAmount; } }

    protected override void Awake()
    {
        base.Awake();
        barbarianScript = companionScript as Barbarian;
    }

    protected override void SetSkillID()
    {
        skillID = (int)EBarbarianSkill.Berserker;
    }

    protected override void AdjustSkillLevel(List<int> skillLevels)
    {
        base.AdjustSkillLevel(skillLevels);

        if (level >= 1)
        {
            companionScript.UpgradeAttack(skillAdjustAmount[0]);
        }
        if (level >= 2)
        {
            companionScript.UpgradeAttack(skillAdjustAmount[1]);
        }
        if (level >= 3)
        {
            companionScript.UpgradeAttack(skillAdjustAmount[2]);
            barbarianScript.MakeBerserker();
        }
    }
}
