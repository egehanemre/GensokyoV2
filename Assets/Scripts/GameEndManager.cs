using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameEndManager : MonoBehaviour
{
    public Animator transitionAnimator;
    public void GoToMainMenu()
    {
        ResetGameState();
        StartCoroutine(FadeAndGoToMainMenu());
    }
    private IEnumerator FadeAndGoToMainMenu()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.FadeOutMusic(1f);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainMenu");
    }
    public void RetryStage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    private void ResetGameState()
    {
        EnemyUnits.Instance?.ResetGame();
        GoldManager.Instance?.ResetGame();
        PlayerUnits.Instance?.ResetGame();
        StageManager.Instance.stageManagerObject.SetActive(true);
        CombatManager.consecutiveLosses = 0;
    }
}