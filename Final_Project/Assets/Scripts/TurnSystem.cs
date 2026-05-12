using UnityEngine;
using System;

public class TurnSystem : MonoBehaviour
{
    private static TurnSystem instance;
    public static TurnSystem Instance => instance;

    private int turnNumber;
    private bool isPlayerTurn = true;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerVictory += GameManager_OnGameEnd;
            GameManager.Instance.OnPlayerDefeat += GameManager_OnGameEnd;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerVictory -= GameManager_OnGameEnd;
            GameManager.Instance.OnPlayerDefeat -= GameManager_OnGameEnd;
        }
    }

    private void GameManager_OnGameEnd(object sender, EventArgs e)
    {
        // 游戏结束时不再响应回合切换
    }

    public void NextTurn()
    {
        // 检查游戏是否已结束
        if (GameManager.Instance != null && GameManager.Instance.IsGameEnded())
        {
            return;
        }

        isPlayerTurn = !isPlayerTurn;
        turnNumber++;
        OnTurnChange?.Invoke(this, EventArgs.Empty);
    }

    public int GetTurnNumber()
    {
        return turnNumber;
    }

    public event EventHandler OnTurnChange;

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }
}
