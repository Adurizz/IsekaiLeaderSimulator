using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private Jun_TweenRuntime fadeOutJunTween;

    public void OnClickIntro()
    {
        LobbyFadeOut();
        Invoke(nameof(MoveToLobbyScene), 2.5f);
    }

    private void MoveToLobbyScene()
    {
        SceneManager.LoadScene(GlobalValueHolder.lobbySceneIndex);
    }

    private void LobbyFadeOut()
    {
        fadeOutJunTween.Play();
    }
}
