using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileController : MonoBehaviour
{
    [SerializeField] protected Transform ownerTransform;
    protected Rigidbody rb;
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected Vector3 fireOffset;
    [SerializeField] protected float damage;
    [SerializeField] protected bool isTargeted;
    [SerializeField] protected Transform targetTransform;
    private Vector3 moveVec;


    protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected IEnumerator Start()
    {
        yield return new WaitUntil(() => ownerTransform != null);

        SetInitPosition();
        SetDamage();

        if (!isTargeted)
            SetMoveDirection();
    }

    private void FixedUpdate()
    {
        if (isTargeted)
            MakeTargetedMove();
    }

    protected void SetInitPosition()
    {
        transform.localRotation = ownerTransform.rotation;

        transform.localPosition = ownerTransform.position;
    }

    protected void SetMoveDirection()
    {
        rb.linearVelocity = ownerTransform.forward * projectileSpeed;
    }

    protected void MakeTargetedMove()
    {
        if (targetTransform == null)
            return;

        moveVec = Vector3.Normalize(targetTransform.position - transform.position) * projectileSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveVec);

        Quaternion dirQuat = Quaternion.LookRotation(moveVec);
        Quaternion moveQuat = Quaternion.Slerp(rb.rotation, dirQuat, 0.3f);
        rb.MoveRotation(moveQuat);
    }

    protected void SetDamage()
    {
        damage = ownerTransform.GetComponent<Actor>().GetAttackStat();
    }

    public void SetOwnerTransform(Transform newOwnerTransform)
    {
        ownerTransform = newOwnerTransform;
    }

    public void SetTargetTransform(Transform newTargetTransform)
    {
        targetTransform = newTargetTransform;
    }
}
