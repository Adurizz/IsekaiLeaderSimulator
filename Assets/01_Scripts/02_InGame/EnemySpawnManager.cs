using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public struct StageSpawnInfo
{
    public int stageNum;
    public List<SpawnEnemyInfo> stageEnemies;
    public GameObject bossPrefab;
    public List<SpawnAttribute> spawnAttributes;
}

[Serializable]
public struct SpawnEnemyInfo
{
    public GameObject enemyPrefab;
    public int probability;
}

[Serializable]
public struct SpawnAttribute
{
    public int spawnNumAtOnce;
    public float spawnInterval;
}

public class EnemySpawnManager : MonoBehaviour
{
    public bool test;
    [SerializeField] private GameObject player;
    [SerializeField] private TextMeshProUGUI phaseText;
    [SerializeField] private List<GameObject> curStageEnemies;
    [SerializeField] private List<int> curStageEnemySpawnProbability;
    [SerializeField] private GameObject curStageBoss;
    [SerializeField] private int phase = 0;
    public int Phase
    {
        get { return phase; }
        set
        {
            phase = value;
            SetStageSpawnInfo(phase);
        }
    }
    [SerializeField] private List<SpawnAttribute> curStageSpawnAttribute;
    [SerializeField] private int spawnNumAtOnce;
    [SerializeField] private float spawnInterval;

    private const float limitX = 250f;
    private const float limitZ = 250f;
    private const int enemyPoolNum = 100;
    private Queue<GameObject> enemyPoolQueue = new();
    private ExpeditionTimeChecker timeChecker;

    [SerializeField] private GameObject enemyArrow;
    private Queue<GameObject> enemyArrowPool = new();
    private const int initEnemyArrowNum = 500;
    [SerializeField] private GameObject enemyEnergyBall;
    private Queue<GameObject> enemyEnergyBallPool = new();
    private const int initEnemyEnergyBallNum = 150;

    #region 스폰 관련 데이터
    [Header("스폰 관련 데이터")]
    [SerializeField] private List<StageSpawnInfo> stageEnemyInfoList;
    private Dictionary<int, List<SpawnEnemyInfo>> stageSpawnEnemyDict = new();
    private Dictionary<int, List<SpawnAttribute>> stageSpawnAttributeDict = new();
    private Dictionary<int, GameObject> stageBossDict = new();
    #endregion

    private void Awake()
    {
        timeChecker = FindAnyObjectByType<ExpeditionTimeChecker>();
    }

    private void Start()
    {
        timeChecker.informNewPhase.RemoveListener(AdjustPhase);
        timeChecker.informNewPhase.AddListener(AdjustPhase);
        // SpawnBoss();
    }

    #region 스폰 관련 데이터 세팅
    public void InitStageSpawnInfoDict()
    {
        foreach (StageSpawnInfo info in stageEnemyInfoList)
        {
            stageSpawnEnemyDict[info.stageNum] = info.stageEnemies;
            stageSpawnAttributeDict[info.stageNum] = info.spawnAttributes;
            stageBossDict[info.stageNum] = info.bossPrefab;
        }
    }

    private List<SpawnEnemyInfo> GetStageSpawnInfo(int stageNum)
    {
        return stageSpawnEnemyDict[stageNum];
    }

    private List<SpawnAttribute> GetStageSpawnAttribute(int stageNum)
    {
        return stageSpawnAttributeDict[stageNum];
    }

    private GameObject GetBoss(int stageNum)
    {
        return stageBossDict[stageNum];
    }

    /// <summary>
    /// 이번 스테이지에 스폰될 적들과 출현 확률 세팅
    /// </summary>
    /// <param name="curStageNum"></param>
    public void SetStageSpawnEnemy(int curStageNum)
    {
        List<SpawnEnemyInfo> curStageEnemyInfo = GetStageSpawnInfo(curStageNum);

        foreach (var enemyInfo in curStageEnemyInfo)
        {
            curStageEnemies.Add(enemyInfo.enemyPrefab);
            curStageEnemySpawnProbability.Add(enemyInfo.probability);
        }

        curStageBoss = GetBoss(curStageNum);
    }

    /// <summary>
    /// 이번 스테이지 스폰 정보(간격, 수) 세팅
    /// </summary>
    /// <param name="curStageNum"></param>
    public void InitStageSpawnInfo(int curStageNum)
    {
        curStageSpawnAttribute = GetStageSpawnAttribute(curStageNum);
        SetStageSpawnInfo(phase);
    }

    /// <summary>
    /// 이번 스테이지에서 페이지 변화에 따른 스폰 정보 변화 반영 로직
    /// </summary>
    /// <param name="phaseNum"></param>
    public void SetStageSpawnInfo(int phaseNum)
    {
        spawnNumAtOnce = curStageSpawnAttribute[phaseNum].spawnNumAtOnce;
        spawnInterval = curStageSpawnAttribute[phaseNum].spawnInterval;
        phaseText.text = "페이즈 " + (Phase + 1);
        if (phaseNum == 4)
            SpawnBoss();
    }

    private void AdjustPhase(int phaseNum)
    {
        Phase = phaseNum - 1;
    }
    #endregion

    private GameObject PickRandomEnemy()
    {
        int totalWeight = 0;

        foreach (int weight in curStageEnemySpawnProbability)
        {
            totalWeight += weight;
        }

        int randomVal = UnityEngine.Random.Range(0, totalWeight);

        int criteria = 0;

        for (int i = 0; i < curStageEnemySpawnProbability.Count; ++i)
        {
            criteria += curStageEnemySpawnProbability[i];
            if (randomVal < criteria)
            {
                return curStageEnemies[i];

            }
        }

        return null;
    }

    public void CreateEnemyPool()
    {
        GameObject enemyPoolGO = new GameObject("EnemyPool");
        enemyPoolGO.transform.SetParent(GameObject.Find(GlobalValueHolder.objectPoolName).transform);
        
        for (int i = 0; i < enemyPoolNum; ++i)
        {
            GameObject temp = Instantiate(PickRandomEnemy(), enemyPoolGO.transform);
            temp.SetActive(false);
            enemyPoolQueue.Enqueue(temp);
        }
    }

    public void CreateEnemyProjectilePool()
    {
        GameObject enemyPoolGO = new GameObject("EnemyArrowPool");
        enemyPoolGO.transform.SetParent(GameObject.Find(GlobalValueHolder.objectPoolName).transform);

        for (int i = 0; i < initEnemyArrowNum; ++i)
        {
            GameObject temp = Instantiate(enemyArrow, enemyPoolGO.transform);
            temp.GetComponent<EnemyArrowController>().SetEnemySpawnManager(this);
            temp.SetActive(false);
            enemyArrowPool.Enqueue(temp);
        }

        GameObject enemyEnergyBallPoolGO = new GameObject("EnemyEnergyBallPool");
        enemyEnergyBallPoolGO.transform.SetParent(GameObject.Find(GlobalValueHolder.objectPoolName).transform);

        for (int i = 0; i < initEnemyEnergyBallNum; ++i)
        {
            GameObject temp = Instantiate(enemyEnergyBall, enemyEnergyBallPoolGO.transform);
            temp.GetComponent<EnemyEnergyBallController>().SetEnemySpawnManager(this);
            temp.SetActive(false);
            enemyEnergyBallPool.Enqueue(temp);
        }
    }

    public GameObject GetEnemyArrow()
    {
        if (enemyArrowPool.Count > 0)
            return enemyArrowPool.Dequeue();
        else
        {
            GameObject newArrow = Instantiate(enemyArrow);
            newArrow.transform.SetParent(GameObject.Find(GlobalValueHolder.objectPoolName).transform);
            newArrow.GetComponent<EnemyArrowController>().SetEnemySpawnManager(this);
            newArrow.SetActive(false);
            enemyArrowPool.Enqueue(newArrow);
            return enemyArrowPool.Dequeue();
        }
    }

    public void SpawnBoss()
    {
        GameObject temp = Instantiate(curStageBoss);
        temp.transform.position = SetSpawnPosition();
    }

    public void EnqueueArrow(GameObject arrow)
    {
        enemyArrowPool.Enqueue(arrow);
    }

    public GameObject GetEnergyBall()
    {
        if (enemyEnergyBallPool.Count > 0)
            return enemyEnergyBallPool.Dequeue();
        else
        {
            GameObject newEnergyBall = Instantiate(enemyEnergyBall);
            newEnergyBall.transform.SetParent(GameObject.Find(GlobalValueHolder.objectPoolName).transform);
            newEnergyBall.GetComponent<EnemyArrowController>().SetEnemySpawnManager(this);
            newEnergyBall.SetActive(false);
            enemyEnergyBallPool.Enqueue(newEnergyBall);
            return enemyEnergyBallPool.Dequeue();
        }
    }
    public void EnqueueEnergyBall(GameObject energyBall)
    {
        enemyEnergyBallPool.Enqueue(energyBall);
    }


    public IEnumerator SpawnEnemyWithInterval()
    {
        if (test)
        {
            GameObject temp = enemyPoolQueue.Dequeue();
            temp.transform.position = SetSpawnPosition();
            temp.SetActive(true);
        }
        
        float time = 0;
        while (true)
        {
            time += Time.deltaTime;
            if (time > spawnInterval)
            {
                SpawnMultipleEnemyAtOnce();
                time = 0;
            }
            yield return null;
        }
    }

    private void SpawnMultipleEnemyAtOnce()
    {
        for (int i = 0; i < spawnNumAtOnce; ++i)
        {
            if (enemyPoolQueue.Count <= 0)
                return;
            GameObject temp = enemyPoolQueue.Dequeue();
            temp.transform.position = SetSpawnPosition();
            temp.SetActive(true);
        }
    }

    public void EnqueueEnemyOnDead(GameObject enemy)
    {
        enemyPoolQueue.Enqueue(enemy);
    }

    private Vector3 SetSpawnPosition()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 spawnPos = Vector3.zero;
        int maxTry = 10;

        for (int i = 0; i < maxTry; ++i)
        {
            // 360도 랜덤 방향
            float angle = UnityEngine.Random.Range(0f, 360f);
            Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
            float spawnDistance = UnityEngine.Random.Range(80f, 90f);

            // 후보 위치 계산
            spawnPos = playerPos + dir * spawnDistance + new Vector3(0, 1, 0);
            spawnPos.y = 1f; // y값 고정

            // x, z 범위 체크
            if (spawnPos.x >= -limitX && spawnPos.x <= limitX &&
                spawnPos.z >= -limitZ && spawnPos.z <= limitZ)
            {
                return spawnPos;
            }
        }

        // 100번 시도해도 못 찾으면 기본 위치 반환
        return new Vector3(0, 1, -10);
    }
}
