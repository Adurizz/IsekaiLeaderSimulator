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


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (ownerTransform != null)
            SetInitPosition();
    }

    protected virtual void FixedUpdate()
    {
        if (isTargeted)
            MakeTargetedMove();
    }

    protected void SetInitPosition()
    {
        transform.localRotation = ownerTransform.rotation;
        Vector3 forwardOffset = ownerTransform.rotation * fireOffset;
        transform.localPosition = Utils.GetCenter(ownerTransform) + forwardOffset;
    }

    protected void SetMoveDirection()
    {
        rb.linearVelocity = ownerTransform.forward * projectileSpeed;
    }

    protected void MakeTargetedMove()
    {
        if (targetTransform == null)
            return;

        Vector3 dirVec = Utils.GetCenter(targetTransform) - transform.localPosition;

        moveVec = Vector3.Normalize(dirVec) * projectileSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveVec);

        Quaternion dirQuat = Quaternion.LookRotation(moveVec);
        Quaternion moveQuat = Quaternion.Slerp(rb.rotation, dirQuat, 0.3f);
        rb.MoveRotation(moveQuat);
    }

    protected void SetDamage()
    {
        damage = ownerTransform.GetComponent<Actor>().Attack;
    }

    public virtual void SetOwnerTransform(Transform newOwnerTransform)
    {
        ownerTransform = newOwnerTransform;

        SetInitPosition();
        SetDamage();

        if (!isTargeted)
            SetMoveDirection();
    }

    public void SetTargetTransform(Transform newTargetTransform)
    {
        targetTransform = newTargetTransform;
    }
}
