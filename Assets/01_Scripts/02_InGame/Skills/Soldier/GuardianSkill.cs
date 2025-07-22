using System.Collections.Generic;
using UnityEngine;

public class GuardianSkill : Skill
{
    [SerializeField] private List<int> skillAdjustAmount = new();
    private Soldier soldierScript;

    protected override void Awake()
    {
        base.Awake();
        soldierScript = companionScript as Soldier;
    }

    protected override void SetSkillID()
    {
        skillID = (int)ESoldierSkill.Guardian;
    }

    protected override void AdjsustSkillLevel(List<int> skillLevels)
    {
        base.AdjsustSkillLevel(skillLevels);

        if (level >= 1)
        {
            companionScript.UpgradeMaxHP(10);
        }
        if (level >= 2)
        {
            companionScript.UpgradeMaxHP(20);
        }
        if (level >= 3)
        {
            companionScript.UpgradeMaxHP(30);
        }
    }
}
