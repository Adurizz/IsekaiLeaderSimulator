using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExpeditionManager : MonoBehaviour
{
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private StageInfo stageInfo;
    [SerializeField] private GameObject expeditionPausePanel;
    private ExpeditionTimeChecker timeChecker;
    private const float goaltime = 10f;
    [SerializeField] private int earnedGold;
    [SerializeField] private EExpeditionResult curExpeditionResult;
    [SerializeField] private int curReward;
    [SerializeField] private TextMeshProUGUI expeditionResultText;
    [SerializeField] private TextMeshProUGUI earnedGoldText;

    private void Awake()
    {
        timeChecker = FindAnyObjectByType<ExpeditionTimeChecker>();
        earnedGold = 10;
    }
    
    public void OnReachedBaseCamp()
    {
        MeasureExpeditionResult();
        RevealExpeditionPausePanel();
    }

    private void RevealExpeditionPausePanel()
    {
        string result = (curExpeditionResult == EExpeditionResult.Success) ? "성공" : "조기 귀환";
        expeditionResultText.text = "탐사결과: " + result;
        earnedGoldText.text = "예상 수익: " + curReward;
        expeditionPausePanel.SetActive(true);
    }

    public void StopExpedition()
    {
        Time.timeScale = 0f;
    }

    private void MeasureExpeditionResult()
    {
        float curPassedTime = timeChecker.GetCurPassedTime();
        if (curPassedTime >= goaltime) 
        {
            curExpeditionResult = EExpeditionResult.Success;
        }
        else
        {
            curExpeditionResult = EExpeditionResult.EarlyReturn;
        }
        curReward = SetReward(curExpeditionResult);
    }

    private int SetReward(EExpeditionResult result)
    {
        int reward = 0;
        switch (result)
        {
            case EExpeditionResult.Success:
                reward = earnedGold;
                break;
            case EExpeditionResult.EarlyReturn:
                reward = earnedGold / 3;
                break;
            case EExpeditionResult.Failure:
                break;
        }
        // TODO: 파티 인원 만큼 뻥튀기
        return reward;
    }

    /// <summary>
    /// 베이스 캠프 판넬 - 게임 재개 버튼에 바인딩
    /// </summary>
    public void ResumeExpedition()
    {
        Time.timeScale = 1.0f;
    }

    public void TerminateExpedition()
    {
        if (curExpeditionResult == EExpeditionResult.Success) 
        { 
            stageInfo.AddMaxOpenedStageNum();
        }

        playerStat.EarnGold(curReward);
        
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(GlobalValueHolder.lobbySceneIndex);
    }
}