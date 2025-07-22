using UnityEngine;

public abstract class Hero : Actor
{
    [SerializeField] protected EHeroClass heroClass;
    public EHeroClass HeroClass { get { return heroClass; } }

    [SerializeField] protected ParticleSystem gotHealedEffect;
    public ParticleSystem GotHealedEffect { get { return gotHealedEffect; } }
}
