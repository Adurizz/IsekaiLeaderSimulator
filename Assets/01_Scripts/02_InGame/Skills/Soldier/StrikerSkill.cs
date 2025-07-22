using UnityEngine;

public class StrikerSkill : Skill
{
    protected override void SetSkillID()
    {
        skillID = (int)ESoldierSkill.Striker;
    }
}
