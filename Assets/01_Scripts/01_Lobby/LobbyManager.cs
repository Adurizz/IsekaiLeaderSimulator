using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private ELobbyType lobbyType;

    [SerializeField] private StageInfo stageInfo;

    [SerializeField] private GameObject expeditionUIGroup;
    [SerializeField] private GameObject trainingUIGroup;
    [SerializeField] private GameObject shopUIGroup;
    [SerializeField] private GameObject missionUIGroup;

    [SerializeField] private GameObject formerRegionButton;
    [SerializeField] private GameObject nextRegionButton;

    [SerializeField] private Scrollbar regionSelectionScrollbar;

    private Coroutine regionSelectionUIRefreshCor;

    private void Start()
    {
        SetRegionSelectionUI();
    }

    public void SetLobbyType(int newlobbyType)
    {
        lobbyType = (ELobbyType)newlobbyType;
    }

    public void MoveToIngameScene()
    {
        SceneManager.LoadScene(GlobalValueHolder.ingameSceneIndex);
    }

    public void DisableAllLobbyUI()
    {
        expeditionUIGroup.SetActive(false);
        trainingUIGroup.SetActive(false);
        shopUIGroup.SetActive(false);
        missionUIGroup.SetActive(false);
        SetLobbyUIAfterWhile(2f);
    }

    private void SetLobbyUIAfterWhile(float amount)
    {
        Invoke(nameof(SetLobbyUI), amount);
    }

    private void SetLobbyUI()
    {
        switch (lobbyType)
        {
            case ELobbyType.Expedition:
                expeditionUIGroup.SetActive(true);
                trainingUIGroup.SetActive(false);
                shopUIGroup.SetActive(false);
                missionUIGroup.SetActive(false);
                break;
            case ELobbyType.Training:
                expeditionUIGroup.SetActive(false);
                trainingUIGroup.SetActive(true);
                shopUIGroup.SetActive(false);
                missionUIGroup.SetActive(false);
                break;
            case ELobbyType.Shop:
                expeditionUIGroup.SetActive(false);
                trainingUIGroup.SetActive(false);
                shopUIGroup.SetActive(true);
                missionUIGroup.SetActive(false);
                break;
            case ELobbyType.Mission:
                expeditionUIGroup.SetActive(false);
                trainingUIGroup.SetActive(false);
                shopUIGroup.SetActive(false);
                missionUIGroup.SetActive(true);
                break;
        }
    }

    public void SetRegionSelectionUI()
    {
        // regionSelectionScrollbar.value = (stageInfo.GetCurStage() - 1) / (float)(GlobalValueHolder.maxStageNum - 1);
        if (regionSelectionUIRefreshCor == null)
            regionSelectionUIRefreshCor = StartCoroutine(RegionSelectionUIRefreshCor());

        #region 이전/다음 지역 버튼 활성/비활성 처리
        if (stageInfo.GetCurStage() == 1)
            formerRegionButton.SetActive(false);
        else if (stageInfo.GetCurStage() == GlobalValueHolder.maxStageNum)
            nextRegionButton.SetActive(false);
        else
        {
            formerRegionButton.SetActive(true);
            nextRegionButton.SetActive(true);
        }
        #endregion
    }

    IEnumerator RegionSelectionUIRefreshCor()
    {
        Debug.Log("Coroutine Start");
        float tolerance = 0.0001f;

        while (Mathf.Abs(regionSelectionScrollbar.value - (stageInfo.GetCurStage() - 1) / (float)(GlobalValueHolder.maxStageNum - 1)) > tolerance)
        {
            regionSelectionScrollbar.value = Mathf.Lerp(regionSelectionScrollbar.value, (stageInfo.GetCurStage() - 1) / (float)(GlobalValueHolder.maxStageNum - 1), Time.deltaTime * 3f);
            // Debug.Log("Moving");
            yield return null;
        }
        regionSelectionScrollbar.value = (stageInfo.GetCurStage() - 1) / (float)(GlobalValueHolder.maxStageNum - 1);
        regionSelectionUIRefreshCor = null;
        Debug.Log("Coroutine End");
    }
}
