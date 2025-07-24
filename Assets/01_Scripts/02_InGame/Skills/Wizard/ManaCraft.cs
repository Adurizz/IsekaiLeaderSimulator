using System.Collections.Generic;
using UnityEngine;

public class ManaCraft : Skill
{
    private Wizard wizardScript;

    protected override void Awake()
    {
        base.Awake();
        wizardScript = companionScript as Wizard;
    }

    protected override void SetSkillID()
    {
        skillID = (int)EWizardSkill.ManaCraft;
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
            wizardScript.MasterManaCraft();
        }
    }
}
