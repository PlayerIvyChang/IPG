using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Configuration")]
    [SerializeField] private List<LevelData> levels;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // 确保 GameProgress 存在
        if (GameProgress.Instance == null)
        {
            GameObject progressObj = new GameObject("GameProgress");
            progressObj.AddComponent<GameProgress>();
        }
        
        // 确保 GameData 存在
        if (GameData.Instance == null)
        {
            GameObject dataObj = new GameObject("GameData");
            dataObj.AddComponent<GameData>();
        }
    }

    public LevelData GetCurrentLevel()
    {
        if (GameProgress.Instance == null)
        {
            return null;
        }
        
        int levelIndex = GameProgress.Instance.CurrentLevel - 1;
        
        if (levelIndex >= 0 && levelIndex < levels.Count)
        {
            return levels[levelIndex];
        }
        
        return null;
    }

    public void OnLevelComplete()
    {
        StartCoroutine(HandleLevelComplete());
    }

    private IEnumerator HandleLevelComplete()
    {
        yield return new WaitForSeconds(1f);

        if (GameProgress.Instance == null)
        {
            yield break;
        }

        // 保存当前生命值
        if (PlayerSystem.Instance != null && PlayerSystem.Instance.PlayerView != null)
        {
            GameProgress.Instance.CurrentHealth = PlayerSystem.Instance.PlayerView.CurrentHealth;
        }

        // 检查是否完成所有关卡
        if (GameProgress.Instance.CurrentLevel >= levels.Count)
        {
            if (GameData.Instance != null)
            {
                GameData.Instance.IsVictory = true;
            }
            SceneManager.LoadScene("EndScene");
        }
        else
        {
            // 显示卡牌奖励选择
            LevelData currentLevel = GetCurrentLevel();
            
            if (currentLevel == null)
            {
                LoadNextLevel();
                yield break;
            }

            if (currentLevel.CardRewards != null && currentLevel.CardRewards.Count >= 3)
            {
                if (CardRewardPanel.Instance != null)
                {
                    CardRewardPanel.Instance.ShowRewards(
                        currentLevel.CardRewards,
                        OnRewardSelected
                    );
                }
                else
                {
                    LoadNextLevel();
                }
            }
            else
            {
                LoadNextLevel();
            }
        }
    }

    private void OnRewardSelected()
    {
        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.NextLevel();
        }
        
        SceneManager.LoadScene("Game");
    }
}