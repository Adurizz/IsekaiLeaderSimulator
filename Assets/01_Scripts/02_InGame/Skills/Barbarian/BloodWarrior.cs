using System.Collections.Generic;
using UnityEngine;

public class BloodWarrior : Skill
{


    protected override void SetSkillID()
    {
        skillID = (int)EBarbarianSkill.BloodWarrior;
    }

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
        }
    }
}
