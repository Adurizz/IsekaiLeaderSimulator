using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SkillInfo", menuName = "SkillInfo")]
public class SkillInfo : ScriptableObject
{
    public EHeroClass ownerClass;
    public Sprite skillImg;
    public int skillID;
    public int skillLevel;
    public string skillName;
    public string skillDescription;
}
