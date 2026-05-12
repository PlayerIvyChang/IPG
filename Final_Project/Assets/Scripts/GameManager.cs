using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnPlayerVictory;
    public event EventHandler OnPlayerDefeat;

    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Units.OnAnyUnitDead += Units_OnAnyUnitDead;
    }

    private void OnDestroy()
    {
        Units.OnAnyUnitDead -= Units_OnAnyUnitDead;
    }

    private void Units_OnAnyUnitDead(object sender, EventArgs e)
    {
        if (gameEnded)
        {
            return;
        }

        // 延迟检查，确保单位列表已更新
        StartCoroutine(CheckGameEndConditionsDelayed());
    }

    private IEnumerator CheckGameEndConditionsDelayed()
    {
        // 等待一帧，确保单位已从列表中移除
        yield return null;

        CheckGameEndConditions();
    }

    private void CheckGameEndConditions()
    {
        if (UnitManager.Instance == null)
        {
            Debug.LogWarning("UnitManager.Instance is null in GameManager.CheckGameEndConditions");
            return;
        }

        int friendlyCount = UnitManager.Instance.GetFriendlyUnitList().Count;
        int enemyCount = UnitManager.Instance.GetEnemyUnitList().Count;

        if (friendlyCount == 0)
        {
            // 所有玩家单位死亡 - 失败
            gameEnded = true;
            OnPlayerDefeat?.Invoke(this, EventArgs.Empty);
        }
        else if (enemyCount == 0)
        {
            // 所有敌人死亡 - 胜利
            gameEnded = true;
            OnPlayerVictory?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsGameEnded()
    {
        return gameEnded;
    }
}