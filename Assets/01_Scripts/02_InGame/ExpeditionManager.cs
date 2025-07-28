using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExpeditionManager : MonoBehaviour
{
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private StageInfo stageInfo;
    [SerializeField] private GameObject expeditionPausePanel;
    [SerializeField] private GameObject expeditionResumeButton;
    private ExpeditionTimeChecker timeChecker;
    private const float goaltime = 10f;
    [SerializeField] private int earnedGold;
    public int EarnedGold
    {
        get { return earnedGold; }
        set 
        { 
            earnedGold = value; 
            IngameUIManager.Instance.SetGoldText(earnedGold);
        }
    }
    [SerializeField] private EExpeditionResult curExpeditionResult;
    [SerializeField] private int curReward;
    [SerializeField] private TextMeshProUGUI expeditionResultText;
    [SerializeField] private TextMeshProUGUI earnedGoldText;
    private PartyManager partyManager;

    private void Awake()
    {
        timeChecker = FindAnyObjectByType<ExpeditionTimeChecker>();
        partyManager = FindAnyObjectByType<PartyManager>();
    }

    public void OnReachedBaseCamp()
    {
        MeasureExpeditionResult();
        RevealExpeditionPausePanel();
    }

    private void RevealExpeditionPausePanel()
    {
        string result = "";
        switch (curExpeditionResult)
        {
            case EExpeditionResult.Success:
                result = "성공";
                expeditionResumeButton.SetActive(true);
                expeditionResultText.color = Color.green; 
                break;
            case EExpeditionResult.EarlyReturn:
                result = "조기 귀환";
                expeditionResumeButton.SetActive(true);
                expeditionResultText.color = Color.yellow;
                break;
            case EExpeditionResult.Failure:
                expeditionResumeButton.SetActive(false);
                expeditionResultText.color = Color.red;
                result = "실패";
                break;
        }
        expeditionResultText.text = result;
        earnedGoldText.text = curReward.ToString();
        expeditionPausePanel.SetActive(true);
    }

    public void StopExpedition()
    {
        Time.timeScale = 0f;
    }

    public void OnPlayerDied()
    {
        curExpeditionResult = EExpeditionResult.Failure;
        curReward = 0;
        RevealExpeditionPausePanel();
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

        reward *= partyManager.GetPartySize() + 1;

        return reward;
    }

    public void EarnGold(int amount)
    {
        earnedGold += amount;
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
        playerStat.ResetStone();
        
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(GlobalValueHolder.lobbySceneIndex);
    }
}