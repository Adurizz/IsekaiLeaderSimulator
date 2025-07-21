using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorStat : ScriptableObject
{
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float attack;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float attackDistance;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float moveSpeed;

    public List<float> CopyStat()
    {
        List<float> temp = new List<float>()
        {
            maxHealth, attack, attackSpeed, attackDistance, attackRange, moveSpeed
        };
        return temp;
    }
}
