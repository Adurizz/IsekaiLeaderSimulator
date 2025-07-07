using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileController : MonoBehaviour
{
    [SerializeField] private Transform ownerTransform;
    private Rigidbody rb;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private Vector3 fireOffset;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => ownerTransform != null);

        SetInitPosition();
        SetMoveDirection();
    }

    private void SetInitPosition()
    {
        transform.localRotation = ownerTransform.rotation;

        transform.localPosition = ownerTransform.position;
    }

    private void SetMoveDirection()
    {
        rb.linearVelocity = ownerTransform.forward * projectileSpeed;
    }

    public void SetOwnerTransform(Transform newOwnerTransform)
    {
        ownerTransform = newOwnerTransform;
    }
}
