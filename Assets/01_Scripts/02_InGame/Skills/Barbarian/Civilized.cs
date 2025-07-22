using System.Collections.Generic;
using UnityEngine;

public class Civilized : Skill
{
    Barbarian barbarianScript;

    protected override void Awake()
    {
        base.Awake();
        barbarianScript = companionScript as Barbarian;
    }

    protected override void SetSkillID()
    {
        skillID = (int)EBarbarianSkill.Civilized;
    }

    protected override void AdjustSkillLevel(List<int> skillLevels)
    {
        base.AdjustSkillLevel(skillLevels);

        if (level >= 1)
        {
            companionScript.UpgradeAttackSpeed(0.05f);
            companionScript.UpgradeMoveSpeed(0.5f);
        }
        if (level >= 2)
        {
            companionScript.UpgradeAttackSpeed(0.1f);
            companionScript.UpgradeMoveSpeed(1f);
        }
        if (level >= 3)
        {
            companionScript.UpgradeAttackSpeed(0.25f);
            companionScript.UpgradeMoveSpeed(2.5f);
            barbarianScript.MakeCivilized();
        }
    }
}
