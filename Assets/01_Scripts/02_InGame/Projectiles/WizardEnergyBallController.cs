using UnityEngine;

public class WizardEnergyBallController : ProjectileController
{
    private Wizard ownerWizard;
    [SerializeField] private bool isMasteredManaCraft;

    public bool IsMasteredManaCraft
    {
        get { return isMasteredManaCraft; }
        set { isMasteredManaCraft = value; }
    }
    [SerializeField] private ParticleSystem areaAttackParticle;

    public override void SetOwnerTransform(Transform newOwnerTransform)
    {
        base.SetOwnerTransform(newOwnerTransform);
        ownerWizard = ownerTransform.GetComponent<Wizard>();
    }
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (ownerWizard != null)
            Gizmos.DrawWireSphere(transform.position, ownerWizard.RangedNormalAttackRange);
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (!IsMasteredManaCraft)
            {
                other.gameObject.GetComponent<Actor>().GetDamage(damage);
                ownerWizard.EnqueueEnergyBallOnDisable(gameObject);
                gameObject.SetActive(false);
                OnDisableEnergyBall();
            }
            else
            {
                areaAttackParticle.Play();
                ownerWizard.ApplyAreaDamage(transform.position, ownerWizard.RangedNormalAttackRange);
                Invoke(nameof(OnDisableEnergyBall), 0.6f);
            }
        }
    }

    private void OnDisableEnergyBall()
    {
        ownerWizard.EnqueueEnergyBallOnDisable(gameObject);
        gameObject.SetActive(false);
    }
}
