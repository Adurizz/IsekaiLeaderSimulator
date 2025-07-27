using System.Collections.Generic;
using UnityEngine;

public class Overheat : Skill
{
    private Mechanic mechanicScript;

    protected override void Awake()
    {
        base.Awake();
        mechanicScript = companionScript as Mechanic;
    }

    protected override void SetSkillID()
    {
        skillID = (int)EMechanicSkill.SelfFix;
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
            mechanicScript.MakeSelfFix();
        }
    }
}
