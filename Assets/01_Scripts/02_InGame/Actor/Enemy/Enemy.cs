using UnityEngine;

public abstract class Enemy : Actor
{
    [SerializeField] private Transform target;
    private Rigidbody rb;
    private Vector3 moveVec;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SetTarget();
    }

    protected virtual void SetTarget()
    {
        target = FindAnyObjectByType<Player>().transform;
    }

    private void FixedUpdate()
    {
        Move();
    }

    protected virtual void Move()
    {
        if (target == null)
            return;

        moveVec = Vector3.Normalize(target.transform.position - transform.position) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveVec);

        Quaternion dirQuat = Quaternion.LookRotation(moveVec);
        Quaternion moveQuat = Quaternion.Slerp(rb.rotation, dirQuat, 0.3f);
        rb.MoveRotation(moveQuat);
    }

    protected abstract void Attack();

    protected virtual void DropItem()
    {

    }
}
