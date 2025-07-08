using UnityEngine;

[CreateAssetMenu(fileName = "StageInfo", menuName = "StageInfo")]
public class StageInfo : ScriptableObject
{
    [SerializeField] private int curStageNum;
    [SerializeField] private int maxOpenedStageNum;

    public void AddCurStageNum()
    {
        if (curStageNum < GlobalValueHolder.maxStageNum)
            curStageNum += 1;
    }

    public void MinusCurStageNum()
    {
        if (curStageNum > 1)
            curStageNum -= 1;
    }

    public int GetCurStage()
    {
        return curStageNum;
    }

    public int GetMaxOpenedStageNum()
    {
        return maxOpenedStageNum;
    }

    public void AddMaxOpenedStageNum()
    {
        if (maxOpenedStageNum >= GlobalValueHolder.maxStageNum)
            return;

        if (curStageNum == maxOpenedStageNum)
        {
            curStageNum++;
            maxOpenedStageNum++;
        }
    }
}
