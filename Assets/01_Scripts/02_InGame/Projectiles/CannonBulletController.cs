using UnityEngine;

public class CannonBulletController : ProjectileController
{
    private Cannon ownerCannon;
    [SerializeField] private GameObject boomEffect;

    public override void SetOwnerTransform(Transform newOwnerTransform)
    {
        base.SetOwnerTransform(newOwnerTransform);
        ownerCannon = ownerTransform.GetComponent<Cannon>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Instantiate(boomEffect, transform.position, boomEffect.transform.rotation);
            other.gameObject.GetComponent<Actor>().GetDamage(damage);
            ownerCannon.EnqueueCannonBulletOnDisable(gameObject);
            gameObject.SetActive(false);
        }
    }
}
