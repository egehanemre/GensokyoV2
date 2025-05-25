using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    public Transform allySpawnParent;
    public Transform enemySpawnParent;
    public float spawnRadius = 5f;

    private bool combatEnded = false;

    void Start()
    {
        MatchingManager.Instance.playerFairy.Clear();
        MatchingManager.Instance.enemyFairy.Clear();

        foreach (var allyPrefab in CombatPrepData.SelectedAllies)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = allySpawnParent.position + new Vector3(randomOffset.x, 0, randomOffset.y);
            var allyGO = Instantiate(allyPrefab, spawnPos, Quaternion.identity, allySpawnParent);
            var fairy = allyGO.GetComponent<Fairy>();
            if (fairy != null)
            {
                MatchingManager.Instance.playerFairy.Add(fairy);
            }
        }

        foreach (var enemyPrefab in CombatPrepData.SelectedEnemies)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = enemySpawnParent.position + new Vector3(randomOffset.x, 0, randomOffset.y);
            var enemyGO = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, enemySpawnParent);
            var fairy = enemyGO.GetComponent<Fairy>();
            if (fairy != null)
            {
                MatchingManager.Instance.enemyFairy.Add(fairy);
            }
        }
        MatchingManager.Instance.MatchFairies();
        StartCoroutine(CheckCombatEnd());
    }

    private System.Collections.IEnumerator CheckCombatEnd()
    {
        while (!combatEnded)
        {
            MatchingManager.Instance.playerFairy.RemoveAll(f => f == null);
            MatchingManager.Instance.enemyFairy.RemoveAll(f => f == null);

            if (MatchingManager.Instance.playerFairy.Count == 0)
            {
                combatEnded = true;
                OnCombatEnd(false); 
            }
            else if (MatchingManager.Instance.enemyFairy.Count == 0)
            {
                combatEnded = true;
                OnCombatEnd(true); 
            }
            yield return null;
        }
    }
    private void OnCombatEnd(bool alliesWin)
    {
        if (alliesWin)
        {
            Debug.Log("Allies Win!");
            EnemyUnits.Instance.currentStageIndex++;
        }
        else
        {
            Debug.Log("Enemies Win!");
        }
        SceneManager.LoadScene("WaitingRoom");
        EnemyUnits.Instance.LoadStage(EnemyUnits.Instance.currentStageIndex);
    }
}