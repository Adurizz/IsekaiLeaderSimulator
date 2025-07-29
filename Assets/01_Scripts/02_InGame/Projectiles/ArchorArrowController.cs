using UnityEngine;

public class ArchorArrowController : ProjectileController
{
    [SerializeField] private bool isKnockBack;
    public bool IsKnockBack
    {
        get { return isKnockBack; }
        set { isKnockBack = value; }
    }
    [SerializeField] private bool isThirdAttack;
    public bool IsThirdAttack
    {
        get { return isThirdAttack; }
        set { isThirdAttack = value; }
    }
    private Archor ownerArchor;
    [SerializeField] private GameObject thirdArrowHitEffect;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void SetOwnerTransform(Transform newOwnerTransform)
    {
        base.SetOwnerTransform(newOwnerTransform);
        ownerArchor = ownerTransform.GetComponent<Archor>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            float newDamage = damage;
            if (isThirdAttack)
            {
                newDamage *= 3;
                GameObject thirdHitEffect = Instantiate(thirdArrowHitEffect, other.transform.position, thirdArrowHitEffect.transform.rotation);
            }

            if (isKnockBack)
            {
                other.gameObject.GetComponent<Actor>().GetDamage(newDamage, true, (other.transform.position - ownerTransform.position).normalized);
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
