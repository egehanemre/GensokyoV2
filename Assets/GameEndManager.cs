using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndManager : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
    public void GoToWaitingRoom()
    {
        ResetGameState();
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