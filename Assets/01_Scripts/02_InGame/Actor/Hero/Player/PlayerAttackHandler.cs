using System.Collections;
using UnityEngine;

public class PlayerAttackHandler : MonoBehaviour
{
    private Player playerScript;

    [SerializeField] private GameObject arrowPrefab;

    [SerializeField] private bool isStatInitiated = false;
    [SerializeField] private float curFireCool;
    [SerializeField] private float fireCool;

    private void Awake()
    {
        playerScript = GetComponent<Player>();

        playerScript.onStatInitiated.RemoveListener(CheckPlayerStatInitiated);
        playerScript.onStatInitiated.AddListener(CheckPlayerStatInitiated);
    }

    private void CheckPlayerStatInitiated()
    {
        isStatInitiated = true;
    }

    private void Update()
    {
        if (!isStatInitiated)
            return;

        AttackWithCoolTime();
    }

    private void AttackWithCoolTime()
    {
        if (curFireCool < fireCool)
        {
            curFireCool += Time.deltaTime;
            if (curFireCool >= fireCool)
            {
                curFireCool = 0;
                FireArrow();
            }
        }
    }

    private void FireArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab);
        Debug.Log(transform.position);
        arrow.GetComponent<ProjectileController>().SetOwnerTransform(transform);
    }
}
