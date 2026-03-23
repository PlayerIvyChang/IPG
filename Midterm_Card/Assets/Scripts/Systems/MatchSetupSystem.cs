using System.Collections.Generic;
using UnityEngine;

public class MatchSetupSystem : MonoBehaviour
{
    [Header("Class Data")]
    [SerializeField] private PlayerData archerPlayerData;
    [SerializeField] private PlayerData flameManPlayerData;

    private void Start()
    {
        // 初始化 GameProgress
        if (GameProgress.Instance == null)
        {
            GameObject progressObj = new GameObject("GameProgress");
            progressObj.AddComponent<GameProgress>();
        }

        // 获取所选择的职业数据
        PlayerData selectedPlayerData = GetSelectedPlayerData();
        
        // 第一关，初始化卡组和生命值
        if (GameProgress.Instance.CurrentLevel == 1)
        {
            GameProgress.Instance.InitializeDeck(selectedPlayerData.Deck);
            GameProgress.Instance.MaxHealth = selectedPlayerData.Health;
            GameProgress.Instance.CurrentHealth = selectedPlayerData.Health;
        }

        // 设置玩家
        PlayerSystem.Instance.Setup(selectedPlayerData);
        
        // 使用当前卡组（会随着游戏添加的卡牌）
        CardSystem.Instance.Setup(GameProgress.Instance.CurrentDeck);
        
        // 获取当前关卡的敌人
        if (LevelManager.Instance != null)
        {
            LevelData currentLevel = LevelManager.Instance.GetCurrentLevel();
            if (currentLevel != null)
            {
                EnemySystem.Instance.Setup(currentLevel.Enemies);
            }
        }
        
        // 抽初始手牌
        DrawCardGA drawCardGA = new(5);
        ActionSystem.Instance.Perform(drawCardGA);
    }

    private PlayerData GetSelectedPlayerData()
    {
        if (GameData.Instance != null)
        {
            return GameData.Instance.SelectedClass == PlayerClass.Archer 
                ? archerPlayerData 
                : flameManPlayerData;
        }
        return archerPlayerData;
    }
}
