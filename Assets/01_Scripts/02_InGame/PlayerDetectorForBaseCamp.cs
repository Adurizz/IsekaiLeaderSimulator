using UnityEngine;

public class PlayerDetectorForBaseCamp : MonoBehaviour
{
    private BaseCamp baseCampScript;

    private void Awake()
    {
        baseCampScript = GetComponentInParent<BaseCamp>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 6f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어 인");
            baseCampScript.OnReachBaseCamp();
        }
    }
}
