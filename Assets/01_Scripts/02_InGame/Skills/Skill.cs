using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [SerializeField] protected int level;
    [SerializeField] protected int skillID;
    [SerializeField] protected Companion companionScript;

    protected virtual void Awake()
    {
        companionScript = GetComponent<Companion>();
        SetSkillID();
    }

    protected abstract void SetSkillID();

    protected virtual void Start()
    {
        companionScript.levelUpEvent.RemoveListener(AdjsustSkillLevel);
        companionScript.levelUpEvent.AddListener(AdjsustSkillLevel);
    }

    /// <summary>
    /// levelEvent를 받아 
    /// </summary>
    /// <param name="skillLevels"></param>
    protected virtual void AdjsustSkillLevel(List<int> skillLevels)
    {
        if (level != skillLevels[skillID])
        {
            level = skillLevels[skillID];
            Debug.Log(name + " 업그레이드: " + level);
        }
    }
}
