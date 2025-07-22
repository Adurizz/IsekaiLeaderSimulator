using System.Collections.Generic;
using UnityEngine;

public class Berserker : Skill
{
    private Barbarian barbarianScript;
    public static float attackUpAmount = 1f;

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
            companionScript.UpgradeAttack(10);
        }
        if (level >= 2)
        {
            companionScript.UpgradeAttack(20);
        }
        if (level >= 3)
        {
            companionScript.UpgradeAttack(30);
            barbarianScript.MakeBerserker();
        }
    }
}
