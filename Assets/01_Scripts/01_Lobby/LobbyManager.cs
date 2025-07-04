using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private ELobbyType lobbyType;

    [SerializeField] private GameObject expeditionUIGroup;
    [SerializeField] private GameObject trainingUIGroup;
    [SerializeField] private GameObject shopUIGroup;
    [SerializeField] private GameObject missionUIGroup;

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
}
