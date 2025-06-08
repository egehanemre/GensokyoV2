using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameEndManager : MonoBehaviour
{
    public Animator transitionAnimator;
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void GoToWaitingRoom()
    {
        StartCoroutine(PlayTransitionAndGoToWaitingRoom());
    }
    private IEnumerator PlayTransitionAndGoToWaitingRoom()
    {
        ResetGameState();
        Time.timeScale = 1f;
        if (transitionAnimator != null)
        {
            transitionAnimator.SetTrigger("Start");
            yield return new WaitForSeconds(1f); 
        }
        SceneManager.LoadScene("WaitingRoom");
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
        CombatManager.consecutiveLosses = 0;
    }
}