using System.Collections;
using UnityEngine;

public class PlayerAttackHandler : MonoBehaviour
{
    private Player playerScript;

    [SerializeField] private GameObject arrowPrefab;

    [SerializeField] private GameObject target;
    public GameObject Target
    {
        get
        {
            return target;
        }
        set
        {
            target = value;
            moveHandler.SetTarget(value);
        }
    }

    [SerializeField] private bool isStatInitiated = false;
    [SerializeField] private float curFireCool;
    [SerializeField] private float fireCool;
    [SerializeField] private float searchRadius = 10f;
    private LayerMask enemyLayer;
    [SerializeField] private float curDetectionCool;
    [SerializeField] private float enemyDetectionCool;
    private PlayerMoveHandler moveHandler;

    private void Awake()
    {
        playerScript = GetComponent<Player>();

        playerScript.onStatInitiated.RemoveListener(CheckPlayerStatInitiated);
        playerScript.onStatInitiated.AddListener(CheckPlayerStatInitiated);

        enemyLayer = LayerMask.GetMask("Enemy");
        moveHandler = GetComponent<PlayerMoveHandler>();
    }

    private void CheckPlayerStatInitiated()
    {
        isStatInitiated = true;
    }

    private void Update()
    {
        if (!isStatInitiated)
            return;

        FindNearestEnemyWithCoolTime();

        AttackWithCoolTime();
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
    #endif

    #region 적 탐지 파트
    private void FindNearestEnemyWithCoolTime()
    {
        if (curDetectionCool < enemyDetectionCool)
        {
            curDetectionCool += Time.deltaTime;
            if (curDetectionCool >= enemyDetectionCool)
            {
                curDetectionCool = 0;
                FindNearestEnemy();
            }
        }
    }

    private void FindNearestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius, enemyLayer);
        if (colliders.Length == 0)
        {
            Target = null;
            return;
        }

        Transform nearest = null;
        float minSqrDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            float sqrDist = (col.transform.position - transform.position).sqrMagnitude;
            if (sqrDist < minSqrDistance)
            {
                minSqrDistance = sqrDist;
                nearest = col.transform;
            }
        }

        if (nearest != null)
        {
            Target = nearest.gameObject;
        }
    }
    #endregion

    #region 적 공격 파트
    private void AttackWithCoolTime()
    {
        if (Target == null)
        {
            if (curFireCool != 0)
                curFireCool = 0;
            return;
        }

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
        // Debug.Log(transform.position);
        arrow.GetComponent<ProjectileController>().SetOwnerTransform(transform);
    }
    #endregion
}
