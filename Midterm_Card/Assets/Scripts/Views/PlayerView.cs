using UnityEngine;

public class PlayerView : CombatView
{
    public void Setup(PlayerData playerData)
    {
        if (GameProgress.Instance != null && GameProgress.Instance.CurrentHealth > 0)
        {
            // 非第一关，使用保存的生命值
            SetupBase(GameProgress.Instance.MaxHealth, GameProgress.Instance.CurrentHealth, playerData.image);
        }
        else
        {
            // 第一关，使用完整生命值
            SetupBase(playerData.Health, playerData.image);
        }
    }
}