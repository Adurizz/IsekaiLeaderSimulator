using System.Collections.Generic;
using UnityEngine;

public class Deadeye : Skill
{
    private Archor archorScript;

    protected override void Awake()
    {
        base.Awake();
        archorScript = companionScript as Archor;
    }

    protected override void SetSkillID()
    {
        skillID = (int)EArchorSkill.Deadeye;
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
            archorScript.MakeThirdAttackEnhanced();
        }
    }

    
}
