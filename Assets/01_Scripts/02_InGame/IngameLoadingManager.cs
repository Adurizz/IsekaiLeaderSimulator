using UnityEngine;

public class IngameLoadingManager : MonoBehaviour
{
    [SerializeField] private StageManager stageManager;

    private void Start()
    {
        LoadStageElementsSequencially();
    }

    private void LoadStageElementsSequencially()
    {
        stageManager.CreateMap();
    }
}
