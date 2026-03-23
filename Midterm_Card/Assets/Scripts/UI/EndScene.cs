using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndScene : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Button restartButton;

    private void Start()
    {
        restartButton.onClick.AddListener(OnRestartClicked);
        DisplayResult();
    }

    private void DisplayResult()
    {
        if (GameData.Instance != null && GameData.Instance.IsVictory)
        {
            resultText.text = "VICTORY!";
            resultText.color = Color.green;

            if (levelText != null)
            {
                levelText.text = "You completed all 5 levels!";
            }
        }
        else
        {
            resultText.text = "GAME OVER";
            resultText.color = Color.red;

            if (levelText != null && GameProgress.Instance != null)
            {
                levelText.text = $"Reached Level {GameProgress.Instance.CurrentLevel}";
            }
        }
    }

    private void OnRestartClicked()
    {
        // 重置进度
        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.ResetProgress();
        }

        // 返回开始场景
        SceneManager.LoadScene(0);
    }
}
