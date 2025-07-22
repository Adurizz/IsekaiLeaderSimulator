using System.Collections.Generic;

public class StrikerSkill : Skill
{
    private Soldier soldierScript;

    protected override void Awake()
    {
        base.Awake();
        soldierScript = companionScript as Soldier;
    }

    protected override void SetSkillID()
    {
        skillID = (int)ESoldierSkill.Striker;
    }

    protected override void AdjustSkillLevel(List<int> skillLevels)
    {
        base.AdjustSkillLevel(skillLevels);

        if (level >= 1)
        {
            companionScript.UpgradeAttack(10);
        }
        if (level >= 2)
        {
            companionScript.UpgradeAttack(20);
        }
        if (level >= 3)
        {
            companionScript.UpgradeAttack(30);
            soldierScript.MakeAttackKnockBack();
        }
    }
}
