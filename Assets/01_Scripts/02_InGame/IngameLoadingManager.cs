using Unity.AI.Navigation;
using UnityEngine;

public class IngameLoadingManager : MonoBehaviour
{
    [SerializeField] private StageInfo stageInfo;
    [SerializeField] private StageManager stageManager;
    [SerializeField] private EnemySpawnManager enemySpawnManager;
    [SerializeField] private CampSpawnManager campSpawnManager;
    [SerializeField] private GameObject player;

    private void Start()
    {
        LoadStageElementsSequencially();
    }

    private void LoadStageElementsSequencially()
    {
        LoadMap();
        LoadCamps();
        BakeNavMeshSurface();
        LoadPlayer();
        LoadEnemies();
    }

    private void LoadMap()
    {
        stageManager.CreateMap();
    }

    private void LoadCamps()
    {
        campSpawnManager.SpawnBaseCamp();
        campSpawnManager.SpawnTrainingCamp();
    }

    private void BakeNavMeshSurface()
    {
        NavMeshSurface surface = GameObject.FindWithTag("Ground").GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }

    private void LoadPlayer()
    {
        player.SetActive(true);
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
