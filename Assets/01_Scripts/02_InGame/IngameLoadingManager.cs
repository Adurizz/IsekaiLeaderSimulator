using UnityEngine;

public class IngameLoadingManager : MonoBehaviour
{
    [SerializeField] private StageInfo stageInfo;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private EnemySpawnManager enemySpawnManager;
    [SerializeField] private CampSpawnManager campSpawnManager;

    private void Start()
    {
        LoadStageElementsSequencially();
    }

    private void LoadStageElementsSequencially()
    {
        LoadMap();
        LoadEnemies();
    }

    private void LoadMap()
    {
        stageManager.CreateMap();
    }

    private void LoadEnemies()
    {
        enemySpawnManager.InitStageSpawnInfoDict();
        enemySpawnManager.SetStageSpawnEnemy(stageInfo.GetCurStage());
        enemySpawnManager.InitStageSpawnInfo(stageInfo.GetCurStage());
        enemySpawnManager.CreateEnemyPool();
        StartCoroutine(enemySpawnManager.SpawnEnemyWithInterval());
    }
}
