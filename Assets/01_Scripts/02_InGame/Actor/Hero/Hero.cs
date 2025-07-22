using UnityEngine;

public abstract class Hero : Actor
{
    [SerializeField] protected EHeroClass heroClass;
    public EHeroClass HeroClass { get { return heroClass; } }
}
