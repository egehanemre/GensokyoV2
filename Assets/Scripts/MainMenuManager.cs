using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Canvas settingsCanvas;
    [SerializeField] private Animator transitionAnimator; // Assign in Inspector
    private void Awake()
    {
        settingsCanvas.enabled = false;
        MusicManager.Instance?.PlayMusic(MusicManager.Instance.menuMusic, true, 1f);
    }
    public void LoadWaitingRoomScene()
    {
        StartCoroutine(PlayTransitionAndLoadScene("WaitingRoom"));
    }
    private IEnumerator PlayTransitionAndLoadScene(string sceneName)
    {
        // Fade out music before transition
        if (MusicManager.Instance != null)
            MusicManager.Instance.FadeOutMusic(1f);

        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Start");
            yield return new WaitForSeconds(1f);
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        asyncLoad.allowSceneActivation = true;
    }
}