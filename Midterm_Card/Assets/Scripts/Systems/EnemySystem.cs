using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySystem : Singleton<EnemySystem>
{
    [SerializeField] private EnemyBoardView enemyBoardView;
    
    private bool gameEnded = false;
    
    // 添加公共属性以访问敌人列表
    public List<EnemyView> Enemies => enemyBoardView.EnemyViews;
    
    private void OnEnable()
    {   
        ActionSystem.AttachPerformer<EnemyTurnGA>(EnemyTurnPerformer);
        ActionSystem.AttachPerformer<AttackPlayerGA>(AttackPlayerPerformer);
        ActionSystem.AttachPerformer<EnemyStatusResolveGA>(EnemyStatusResolvePerformer);
    }
    
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<EnemyTurnGA>();
        ActionSystem.DetachPerformer<AttackPlayerGA>();
        ActionSystem.DetachPerformer<EnemyStatusResolveGA>();
    }
    
    public void Setup(List<EnemyData> enemyDatas)
    {
        if (enemyDatas == null || enemyDatas.Count == 0)
        {
            Debug.LogWarning("Enemy data list is null or empty.");
            return;
        }

        foreach (var enemyData in enemyDatas)
        {
            if (enemyData == null)
            {
                Debug.LogWarning("Found null EnemyData in list. Skipping...");
                continue;
            }
            
            enemyBoardView.AddEnemy(enemyData);
        }
        
        gameEnded = false;
    }

    public IEnumerator RemoveDeadEnemy(EnemyView enemy)
    {
        if (enemy != null && enemyBoardView.EnemyViews.Contains(enemy))
        {
            yield return enemyBoardView.RemoveEnemy(enemy);
            
            // 检查是否所有敌人都死了
            CheckVictory();
        }
    }

    private void CheckVictory()
    {
        if (enemyBoardView.EnemyViews.Count == 0 && !gameEnded)
        {
            gameEnded = true;
            StartCoroutine(TriggerVictory());
        }
    }

    private IEnumerator EnemyStatusResolvePerformer(EnemyStatusResolveGA enemyStatusResolve)
    {
        // 遍历所有敌人，结算燃烧状态
        for (int i = enemyBoardView.EnemyViews.Count - 1; i >= 0; i--)
        {
            EnemyView enemy = enemyBoardView.EnemyViews[i];
            
            int burnStacks = enemy.GetStatusEffectStackCount(StatusEffectType.BURN);
            if (burnStacks > 0)
            {
                BurnGA burnGA = new(enemy, burnStacks);
                ActionSystem.Instance.AddReaction(burnGA);
            }
        }
        yield return null;
    }

    private IEnumerator EnemyTurnPerformer(EnemyTurnGA enemyTurn)
    {
        // 移除死亡的敌人
        for (int i = enemyBoardView.EnemyViews.Count - 1; i >= 0; i--)
        {
            EnemyView enemy = enemyBoardView.EnemyViews[i];
            if (enemy.CurrentHealth <= 0)
            {
                yield return enemyBoardView.RemoveEnemy(enemy);
            }
        }
        
        // 检查胜利
        CheckVictory();
        
        if (gameEnded)
        {
            yield break;
        }
        
        // 剩余的敌人攻击玩家
        foreach (var enemy in enemyBoardView.EnemyViews)
        {
            AttackPlayerGA attackPlayerGA = new(enemy);
            ActionSystem.Instance.AddReaction(attackPlayerGA);
        }
        yield return null;
    }
    
    private IEnumerator AttackPlayerPerformer(AttackPlayerGA attackPlayer)
    {
        EnemyView attacker = attackPlayer.Attacker;
        Vector3 originalScale = attacker.transform.localScale;
        
        // 放大
        float time = 0f;
        while (time < 0.15f)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 1.3f, time / 0.15f);
            attacker.transform.localScale = originalScale * scale;
            yield return null;
        }
        
        // 缩小
        time = 0f;
        while (time < 0.15f)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(1.3f, 1f, time / 0.15f);
            attacker.transform.localScale = originalScale * scale;
            yield return null;
        }
        
        attacker.transform.localScale = originalScale;
        
        // 造成伤害
        DealDamageGA dealDamageGA = new(attacker.AttackPower, new() { PlayerSystem.Instance.PlayerView });
        ActionSystem.Instance.AddReaction(dealDamageGA);
    }

    private IEnumerator TriggerVictory()
    {
        yield return new WaitForSeconds(1.5f);
        
        Debug.Log("All enemies defeated! Triggering victory...");
        
        // 通知关卡管理器
        if (LevelManager.Instance != null)
        {
            Debug.Log("LevelManager found, calling OnLevelComplete");
            LevelManager.Instance.OnLevelComplete();
        }
        else
        {
            Debug.LogWarning("LevelManager not found! Loading EndScene directly.");
            // 如果没有关卡管理器，直接显示胜利画面
            if (GameData.Instance != null)
            {
                GameData.Instance.IsVictory = true;
            }
            SceneManager.LoadScene("EndScene");
        }
    }
}
