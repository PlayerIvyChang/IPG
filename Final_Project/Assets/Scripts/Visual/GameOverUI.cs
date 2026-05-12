using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Button returnToMenuButton;
    [SerializeField] private Button restartButton;

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        GameManager.Instance.OnPlayerVictory += GameManager_OnPlayerVictory;
        GameManager.Instance.OnPlayerDefeat += GameManager_OnPlayerDefeat;

        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.AddListener(() =>
            {
                SceneLoader.Instance.LoadStartScene();
            });
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(() =>
            {
                SceneLoader.Instance.RestartGame();
            });
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerVictory -= GameManager_OnPlayerVictory;
            GameManager.Instance.OnPlayerDefeat -= GameManager_OnPlayerDefeat;
        }

        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.RemoveAllListeners();
        }

        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
        }
    }

    private void GameManager_OnPlayerVictory(object sender, EventArgs e)
    {
        ShowGameOver("Victory!", Color.green);
    }

    private void GameManager_OnPlayerDefeat(object sender, EventArgs e)
    {
        ShowGameOver("Defeat!", Color.red);
    }

    private void ShowGameOver(string message, Color color)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (resultText != null)
        {
            resultText.text = message;
            resultText.color = color;
        }

        Time.timeScale = 0f; // ‘›Õ£”Œœ∑
    }
}