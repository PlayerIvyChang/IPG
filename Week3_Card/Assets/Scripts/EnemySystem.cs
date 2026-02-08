using System.Collections;
using UnityEngine;

public class EnemySystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
    }
    private IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurn)
    {
        yield return new WaitForSeconds(2f); // Simulate some delay for enemy actions
        Debug.Log("Enemy turn performed.");
    }
}
