using System.Collections.Generic;
using UnityEngine;

public class Civilized : Skill
{
    [Header("위: 공속, 아래: 이동속도 증가량")]
    [SerializeField] protected float[] skillAdjustAmount2 = new float[3];
    Barbarian barbarianScript;

    protected override void Awake()
    {
        base.Awake();
        barbarianScript = companionScript as Barbarian;
    }

    protected override void SetSkillID()
    {
        skillID = (int)EBarbarianSkill.Civilized;
    }

    protected override void AdjustSkillLevel(List<int> skillLevels)
    {
        base.AdjustSkillLevel(skillLevels);

        if (level >= 1)
        {
            companionScript.UpgradeAttackSpeed(skillAdjustAmount[0]);
            companionScript.UpgradeMoveSpeed(skillAdjustAmount2[0]);
        }
        if (level >= 2)
        {
            companionScript.UpgradeAttackSpeed(skillAdjustAmount[1]);
            companionScript.UpgradeMoveSpeed(skillAdjustAmount2[1]);
        }
        if (level >= 3)
        {
            companionScript.UpgradeAttackSpeed(skillAdjustAmount[2]);
            companionScript.UpgradeMoveSpeed(skillAdjustAmount2[2]);
            barbarianScript.MakeCivilized();
        }
    }
}
