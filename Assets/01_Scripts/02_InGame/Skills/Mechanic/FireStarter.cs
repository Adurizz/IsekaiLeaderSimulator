using System.Collections.Generic;
using UnityEngine;

public class FireStarter : Skill
{
    private Mechanic mechanicScript;

    protected override void Awake()
    {
        base.Awake();
        mechanicScript = companionScript as Mechanic;
    }

    protected override void SetSkillID()
    {
        skillID = (int)EMechanicSkill.FireStarter;
    }

    protected override void AdjustSkillLevel(List<int> skillLevels)
    {
        base.AdjustSkillLevel(skillLevels);

        if (level >= 1)
        {
            companionScript.UpgradeAttack(skillAdjustAmount[0]);
            mechanicScript.MakeAttackAvailable();
        }
        if (level >= 2)
        {
            companionScript.UpgradeAttack(skillAdjustAmount[1]);
        }
        if (level >= 3)
        {
            companionScript.UpgradeAttack(skillAdjustAmount[2]);
            mechanicScript.MakeFireStarter();
        }
    }
}
