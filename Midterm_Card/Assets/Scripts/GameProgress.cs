using UnityEngine;
using System.Collections.Generic;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance { get; private set; }

    public int CurrentLevel { get; set; } = 1;
    
    // 玩家当前的卡组（会随着游戏进行增加卡牌）
    public List<CardData> CurrentDeck { get; private set; } = new();
    
    // 保存玩家当前的生命值
    public int CurrentHealth { get; set; } = -1; // -1 表示未初始化
    public int MaxHealth { get; set; } = 0;

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
        }
    }

    public void InitializeDeck(List<CardData> startingDeck)
    {
        CurrentDeck.Clear();
        CurrentDeck.AddRange(startingDeck);
        CurrentLevel = 1;
    }

    public void AddCardToDeck(CardData card)
    {
        CurrentDeck.Add(card);
    }

    public void NextLevel()
    {
        CurrentLevel++;
    }

    public void ResetProgress()
    {
        CurrentLevel = 1;
        CurrentDeck.Clear();
        CurrentHealth = -1;
        MaxHealth = 0;
    }
}