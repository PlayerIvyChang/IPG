using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerSystem : Singleton<PlayerSystem>
{
    [field: SerializeField] public PlayerView PlayerView { get; private set; }
    
    private bool gameEnded = false;
    
    public void OnEnable()
    {
        ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyPostReaction, ReactionTiming.POST);
    }

    public void OnDisable()
    {
        ActionSystem.DetachPerformer<DealDamageGA>();
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyPreReaction, ReactionTiming.PRE);
        ActionSystem.UnsubscribeReaction<EnemyTurnGA>(EnemyPostReaction, ReactionTiming.POST);
    }
    
    public void Setup(PlayerData playerData)
    {
        PlayerView.Setup(playerData);
        gameEnded = false;
    }
    
    private IEnumerator DealDamagePerformer(DealDamageGA dealDamage)
    {
        foreach (var target in dealDamage.Targets)
        {
            if (target == PlayerView)
            {
                target.Damage(dealDamage.Amount);
                
                // 保存当前生命值到 GameProgress
                if (GameProgress.Instance != null)
                {
                    GameProgress.Instance.CurrentHealth = PlayerView.CurrentHealth;
                    Debug.Log($"[PlayerSystem] 保存生命值: {PlayerView.CurrentHealth}");
                }
                
                // 检查是否死亡
                if (PlayerView.CurrentHealth <= 0 && !gameEnded)
                {
                    gameEnded = true;
                    yield return new WaitForSeconds(1f);
                    
                    // 游戏失败
                    GameData.Instance.IsVictory = false;
                    SceneManager.LoadScene("EndScene");
                    yield break;
                }
            }
        }
        yield return null;
    }

    private void EnemyPreReaction(EnemyTurnGA enemyTurnGA)
    {
        // 先丢弃所有手牌
        DiscardAllGA discardAllGA = new();
        ActionSystem.Instance.AddReaction(discardAllGA);
        
        // 处理敌人的状态效果（燃烧等）
        EnemyStatusResolveGA enemyStatusResolveGA = new();
        ActionSystem.Instance.AddReaction(enemyStatusResolveGA);
    }
    
    private void EnemyPostReaction(EnemyTurnGA enemyTurnGA)
    {
        // 处理玩家的燃烧状态
        int burnStacks = PlayerView.GetStatusEffectStackCount(StatusEffectType.BURN);
        if (burnStacks > 0)
        {
            BurnGA burnGA = new(PlayerView, burnStacks);
            ActionSystem.Instance.AddReaction(burnGA);
        }
        
        // 抽5张牌
        DrawCardGA drawCardGA = new(5);
        ActionSystem.Instance.AddReaction(drawCardGA);
    }
}
