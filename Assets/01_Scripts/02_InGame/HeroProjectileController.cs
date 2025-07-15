using UnityEngine;

public class HeroProjectileController : ProjectileController
{
    private PlayerAttackHandler attackHandler;

    protected override void Awake()
    {
        base.Awake();
        attackHandler = FindAnyObjectByType<PlayerAttackHandler>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Actor>().GetDamage(damage);
            attackHandler.EnqueueArrowOnDisable(gameObject);
            gameObject.SetActive(false);
        }
    }
}
