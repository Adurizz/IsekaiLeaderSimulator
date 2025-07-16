using System.Collections;
using System.Collections.Generic;
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
    private float curFireCool;
    [SerializeField] private float fireCool;
    [SerializeField] private float attackDistance = 10f;
    private LayerMask enemyLayer;
    private float curDetectionCool;
    private float enemyDetectionCool = 0.1f;
    private PlayerMoveHandler moveHandler;
    private const int arrowPoolNum = 20;
    private Queue<GameObject> arrowPoolQueue = new();
    private Animator animator;

    private void Awake()
    {
        playerScript = GetComponent<Player>();

        playerScript.onStatInitiated.RemoveListener(CheckPlayerStatInitiated);
        playerScript.onStatInitiated.AddListener(CheckPlayerStatInitiated);

        enemyLayer = LayerMask.GetMask("Enemy");
        moveHandler = GetComponent<PlayerMoveHandler>();
        animator = GetComponentInChildren<Animator>();
    }

    public void SetAttackDistance(float value)
    {
        attackDistance = value;
    }

    public void SetFireCool(float value)
    {
        fireCool = value;
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
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
    #endif

    public void CreateArrowPool()
    {
        GameObject arrowPoolGO = new GameObject("ArrowPool");
        arrowPoolGO.transform.SetParent(GameObject.Find(GlobalValueHolder.objectPoolName).transform);

        for (int i = 0; i < arrowPoolNum; ++i)
        {
            GameObject temp = Instantiate(arrowPrefab, arrowPoolGO.transform);
            temp.SetActive(false);
            arrowPoolQueue.Enqueue(temp);
        }
    }

    public void EnqueueArrowOnDisable(GameObject arrow)
    {
        arrowPoolQueue.Enqueue(arrow);
    }

    #region 적 탐지 파트
    private void FindNearestEnemyWithCoolTime()
    {
        curDetectionCool += Time.deltaTime;
        if (curDetectionCool >= enemyDetectionCool)
        {
            curDetectionCool = 0;
            FindNearestEnemy();
        }
    }

    private void FindNearestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackDistance, enemyLayer);
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
        if (arrowPoolQueue.Count <= 0)
            return;

        GameObject arrow = arrowPoolQueue.Dequeue();
        arrow.SetActive(true);
        // Debug.Log(transform.position);
        arrow.GetComponent<ProjectileController>().SetOwnerTransform(transform);
        arrow.GetComponent<ProjectileController>().SetTargetTransform(target.transform);
        animator.SetTrigger("Shot");
    }
    #endregion
}
