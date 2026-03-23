using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Level")]
public class LevelData : ScriptableObject
{
    [field: SerializeField] public int LevelNumber { get; private set; }
    [field: SerializeField] public List<EnemyData> Enemies { get; private set; }
    [field: SerializeField] public List<CardData> CardRewards { get; private set; } // 三选一的卡牌奖励
}