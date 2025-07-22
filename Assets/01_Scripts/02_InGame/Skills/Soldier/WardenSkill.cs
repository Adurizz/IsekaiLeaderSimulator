using System.Collections.Generic;
using UnityEngine;

public class WardenSkill : Skill
{
    private Soldier soldierScript;

    protected override void Awake()
    {
        base.Awake();
        soldierScript = companionScript as Soldier;
    }

    protected override void SetSkillID()
    {
        skillID = (int)ESoldierSkill.Warden;
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
            soldierScript.MakeDodgePossible();
        }
    }
}
