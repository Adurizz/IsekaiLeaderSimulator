using System.Collections.Generic;
using UnityEngine;

public class Runner : Skill
{
    private Archor archorScript;
    [SerializeField] private Vector3 evadeOffset;
    public Vector3 EvadeOffset { get { return evadeOffset; } }

    protected override void Awake()
    {
        base.Awake();
        archorScript = companionScript as Archor;
    }

    protected override void SetSkillID()
    {
        skillID = (int)EArchorSkill.Runner;
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
            archorScript.MakeEvasionPossible();
        }
    }
}
