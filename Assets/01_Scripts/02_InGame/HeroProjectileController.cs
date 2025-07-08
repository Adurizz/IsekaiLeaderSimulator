using UnityEngine;

public class HeroProjectileController : ProjectileController
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Actor>().GetDamage(damage);
            gameObject.SetActive(false);
        }
    }
}
