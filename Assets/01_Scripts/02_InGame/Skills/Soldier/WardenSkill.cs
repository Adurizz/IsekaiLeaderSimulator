using UnityEngine;

public class WardenSkill : Skill
{
    protected override void SetSkillID()
    {
        skillID = (int)ESoldierSkill.Warden;
    }
}
