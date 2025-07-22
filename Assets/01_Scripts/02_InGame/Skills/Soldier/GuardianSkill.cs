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

        switch (level)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }
}
