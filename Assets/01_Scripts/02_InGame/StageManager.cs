using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField] private StageInfo stageInfo;
    [SerializeField] private GameObject[] stagePrefabs = new GameObject[GlobalValueHolder.maxStageNum + 1];

    public void CreateMap()
    {
        Instantiate(stagePrefabs[stageInfo.GetCurStage()]);
    }
}
