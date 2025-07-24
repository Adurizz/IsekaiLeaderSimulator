using System.Collections.Generic;
using UnityEngine;

public class VoidCraft : Skill
{
    private Wizard wizardScript;

    protected override void Awake()
    {
        base.Awake();
        wizardScript = companionScript as Wizard;
    }

    protected override void SetSkillID()
    {
        skillID = (int)EWizardSkill.VoidCraft;
    }

    protected override void AdjustSkillLevel(List<int> skillLevels)
    {
        base.AdjustSkillLevel(skillLevels);

        if (level >= 1)
        {
            companionScript.UpgradeMoveSpeed(skillAdjustAmount[0]);
        }
        if (level >= 2)
        {
            companionScript.UpgradeMoveSpeed(skillAdjustAmount[1]);
        }
        if (level >= 3)
        {
            companionScript.UpgradeMoveSpeed(skillAdjustAmount[2]);
            wizardScript.MasterVoidCraft();
        }
    }
}
