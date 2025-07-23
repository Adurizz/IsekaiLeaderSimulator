using UnityEngine;

public class ArchorArrowController : ProjectileController
{
    private bool isKnockBack;
    private bool isThirdAttack;
    private Archor ownerArchor;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void SetOwnerTransform(Transform newOwnerTransform)
    {
        base.SetOwnerTransform(newOwnerTransform);
        ownerArchor = ownerTransform.GetComponent<Archor>();
    }

    public void MakeKnockBackable()
    {
        if (!isKnockBack) isKnockBack = true;
    }

    public void InformThirdAttack()
    {
        if (!isThirdAttack) isThirdAttack = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            float newDamage = damage;
            if (isThirdAttack) newDamage *= 2;

            if (isKnockBack)
            {
                other.gameObject.GetComponent<Actor>().GetDamage(newDamage, true, (other.transform.position - transform.position).normalized);
            }
            else
            {
                other.gameObject.GetComponent<Actor>().GetDamage(newDamage);
            }
            ownerArchor.EnqueueArrowOnDisable(gameObject);
            gameObject.SetActive(false);
        }
    }
}
