using Unity.Cinemachine;
using UnityEngine;

public class LobbySceneCameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineBrain cineBrain;
    [SerializeField] private CinemachineCamera expeditionViewCamera;
    [SerializeField] private CinemachineCamera shopViewCamera;
    [SerializeField] private CinemachineCamera trainingViewCamera;
    [SerializeField] private CinemachineCamera missionViewCamera;

    private void SetLiveCam(ELobbyType lobbyType)
    {
        switch (lobbyType)
        {
            case ELobbyType.Expedition:
                expeditionViewCamera.gameObject.SetActive(true);
                shopViewCamera.gameObject.SetActive(false);
                trainingViewCamera.gameObject.SetActive(false);
                missionViewCamera.gameObject.SetActive(false);
                break;
            case ELobbyType.Shop:
                expeditionViewCamera.gameObject.SetActive(false);
                shopViewCamera.gameObject.SetActive(true);
                trainingViewCamera.gameObject.SetActive(false);
                missionViewCamera.gameObject.SetActive(false);
                break;
            case ELobbyType.Training:
                expeditionViewCamera.gameObject.SetActive(false);
                shopViewCamera.gameObject.SetActive(false);
                trainingViewCamera.gameObject.SetActive(true);
                missionViewCamera.gameObject.SetActive(false);
                break;
            case ELobbyType.Mission:
                expeditionViewCamera.gameObject.SetActive(false);
                shopViewCamera.gameObject.SetActive(false);
                trainingViewCamera.gameObject.SetActive(false);
                missionViewCamera.gameObject.SetActive(true);
                break;
        }
    }

    public void SetExpeditionView()
    {
        SetLiveCam(ELobbyType.Expedition);
    }

    public void SetTrainingView()
    {
        SetLiveCam(ELobbyType.Training);
    }

    public void SetShopView() 
    {
        SetLiveCam(ELobbyType.Shop);
    }

    public void SetMissionView() 
    {
        SetLiveCam(ELobbyType.Mission);
    }
}
