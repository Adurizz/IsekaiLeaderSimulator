using UnityEngine;

public class WizardEnergyBallController : ProjectileController
{
    private Wizard ownerWizard;

    public override void SetOwnerTransform(Transform newOwnerTransform)
    {
        base.SetOwnerTransform(newOwnerTransform);
        ownerWizard = ownerTransform.GetComponent<Wizard>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<Actor>().GetDamage(damage);
            ownerWizard.EnqueueEnergyBallOnDisable(gameObject);
            gameObject.SetActive(false);
        }
    }
}
