using System.Collections.Generic;
using UnityEngine;

public class BloodWarrior : Skill
{
    private Barbarian barbarianScript;
    [SerializeField] private float bloodDrainRate = 0.1f;
    public float BloodDrainRate { get { return bloodDrainRate; } }

    protected override void SetSkillID()
    {
        skillID = (int)EBarbarianSkill.BloodWarrior;
        barbarianScript = companionScript as Barbarian;
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
            barbarianScript.MakeBloodWarrior();
        }
    }
}
