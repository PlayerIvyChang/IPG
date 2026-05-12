using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    private State state;
    private float timer;

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }

    private void Start()
    {
        if (TurnSystem.Instance != null)
        {
            TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChanged;
        }
        else
        {
            Debug.LogError("EnemyAI: TurnSystem.Instance is null!");
        }
    }

    private void OnDestroy()
    {
        if (TurnSystem.Instance != null)
        {
            TurnSystem.Instance.OnTurnChange -= TurnSystem_OnTurnChanged;
        }
    }

    private void Update()
    {
        if (TurnSystem.Instance == null || TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }
                    else
                    {
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }
    }

    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (TurnSystem.Instance != null && !TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 2f;
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        if (UnitManager.Instance == null)
        {
            Debug.LogWarning("EnemyAI: UnitManager.Instance is null!");
            return false;
        }

        if (PathFinding.Instance == null)
        {
            Debug.LogWarning("EnemyAI: PathFinding.Instance is null!");
            return false;
        }

        foreach (Units enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryTakeEnemyAIAction(Units enemyUnit, Action onEnemyAIActionComplete)
    {
        if (enemyUnit.GetActionPoints() <= 0)
        {
            return false;
        }

        // 评估所有可能的动作决策
        AIDecision bestDecision = EvaluateBestDecision(enemyUnit);

        if (bestDecision != null && bestDecision.score > 0)
        {
            ExecuteDecision(enemyUnit, bestDecision, onEnemyAIActionComplete);
            return true;
        }

        return false;
    }

    private AIDecision EvaluateBestDecision(Units enemyUnit)
    {
        AIDecision bestDecision = null;
        float bestScore = 0;

        ShootAction shootAction = enemyUnit.GetComponent<ShootAction>();
        SwordAction swordAction = enemyUnit.GetComponent<SwordAction>();
        GrenadeAction grenadeAction = enemyUnit.GetComponent<GrenadeAction>();
        MoveAction moveAction = enemyUnit.GetComponent<MoveAction>();

        // 策略1: 直接攻击（Shoot/Sword/Grenade）
        if (shootAction != null && shootAction.enabled)
        {
            AIDecision shootDecision = EvaluateDirectAttack(enemyUnit, shootAction);
            if (shootDecision != null && shootDecision.score > bestScore)
            {
                bestScore = shootDecision.score;
                bestDecision = shootDecision;
            }
        }

        if (swordAction != null && swordAction.enabled)
        {
            AIDecision swordDecision = EvaluateDirectAttack(enemyUnit, swordAction);
            if (swordDecision != null && swordDecision.score > bestScore)
            {
                bestScore = swordDecision.score;
                bestDecision = swordDecision;
            }
        }

        if (grenadeAction != null && grenadeAction.enabled)
        {
            AIDecision grenadeDecision = EvaluateDirectAttack(enemyUnit, grenadeAction);
            if (grenadeDecision != null && grenadeDecision.score > bestScore)
            {
                bestScore = grenadeDecision.score;
                bestDecision = grenadeDecision;
            }
        }

        // 策略2: 移动后攻击
        if (moveAction != null && moveAction.enabled)
        {
            AIDecision moveAttackDecision = EvaluateMoveAndAttack(enemyUnit, moveAction, shootAction, swordAction, grenadeAction);
            if (moveAttackDecision != null && moveAttackDecision.score > bestScore)
            {
                bestScore = moveAttackDecision.score;
                bestDecision = moveAttackDecision;
            }
        }

        return bestDecision;
    }

    private AIDecision EvaluateDirectAttack(Units enemyUnit, BaseAction attackAction)
    {
        if (!enemyUnit.CanSpendActionPointsToTakeActioin(attackAction))
        {
            return null;
        }

        List<GridPosition> validPositions = attackAction.GetValidActionGridPositions();
        if (validPositions.Count == 0)
        {
            return null;
        }

        GridPosition bestTarget = new GridPosition();
        float bestTargetScore = 0;

        foreach (GridPosition targetPos in validPositions)
        {
            Units targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(targetPos);
            if (targetUnit == null || targetUnit.IsEnemyUnit())
            {
                continue;
            }

            float targetScore = CalculateTargetScore(targetUnit, attackAction);

            if (targetScore > bestTargetScore)
            {
                bestTargetScore = targetScore;
                bestTarget = targetPos;
            }
        }

        if (bestTargetScore > 0)
        {
            return new AIDecision
            {
                action = attackAction,
                targetPosition = bestTarget,
                score = bestTargetScore,
                movePosition = null
            };
        }

        return null;
    }

    private AIDecision EvaluateMoveAndAttack(Units enemyUnit, MoveAction moveAction, 
        ShootAction shootAction, SwordAction swordAction, GrenadeAction grenadeAction)
    {
        if (!enemyUnit.CanSpendActionPointsToTakeActioin(moveAction))
        {
            return null;
        }

        List<GridPosition> validMovePositions = moveAction.GetValidActionGridPositions();
        if (validMovePositions.Count == 0)
        {
            return null;
        }

        AIDecision bestDecision = null;
        float bestScore = 0;

        foreach (GridPosition movePos in validMovePositions)
        {
            // 评估从这个位置能够攻击到的敌人
            AIDecision decision = EvaluateAttackFromPosition(enemyUnit, movePos, shootAction, swordAction, grenadeAction);
            
            if (decision != null && decision.score > bestScore)
            {
                bestScore = decision.score;
                bestDecision = decision;
                bestDecision.movePosition = movePos;
            }
        }

        // 移动+攻击的价值略低于直接攻击（因为需要消耗更多行动点）
        if (bestDecision != null)
        {
            bestDecision.score *= 0.8f;
        }

        return bestDecision;
    }

    private AIDecision EvaluateAttackFromPosition(Units enemyUnit, GridPosition fromPosition,
        ShootAction shootAction, SwordAction swordAction, GrenadeAction grenadeAction)
    {
        AIDecision bestAttack = null;
        float bestScore = 0;

        // 评估射击
        if (shootAction != null && shootAction.enabled)
        {
            AIDecision shootDecision = EvaluateAttackFromPositionForAction(enemyUnit, fromPosition, shootAction);
            if (shootDecision != null && shootDecision.score > bestScore)
            {
                bestScore = shootDecision.score;
                bestAttack = shootDecision;
            }
        }

        // 评估近战
        if (swordAction != null && swordAction.enabled)
        {
            AIDecision swordDecision = EvaluateAttackFromPositionForAction(enemyUnit, fromPosition, swordAction);
            if (swordDecision != null && swordDecision.score > bestScore)
            {
                bestScore = swordDecision.score;
                bestAttack = swordDecision;
            }
        }

        // 评估手雷
        if (grenadeAction != null && grenadeAction.enabled)
        {
            AIDecision grenadeDecision = EvaluateAttackFromPositionForAction(enemyUnit, fromPosition, grenadeAction);
            if (grenadeDecision != null && grenadeDecision.score > bestScore)
            {
                bestScore = grenadeDecision.score;
                bestAttack = grenadeDecision;
            }
        }

        return bestAttack;
    }

    private AIDecision EvaluateAttackFromPositionForAction(Units enemyUnit, GridPosition fromPosition, BaseAction attackAction)
    {
        int actionPointCost = attackAction.GetActionPointsCost();
        int totalCostWithMove = 1 + actionPointCost; // 移动1点 + 攻击成本

        if (enemyUnit.GetActionPoints() < totalCostWithMove)
        {
            return null;
        }

        // 获取从该位置能够攻击的目标
        List<Units> targetableUnits = GetTargetableUnitsFromPosition(fromPosition, attackAction);

        if (targetableUnits.Count == 0)
        {
            return null;
        }

        // 选择最佳目标
        Units bestTarget = null;
        float bestTargetScore = 0;

        foreach (Units target in targetableUnits)
        {
            float targetScore = CalculateTargetScore(target, attackAction);
            if (targetScore > bestTargetScore)
            {
                bestTargetScore = targetScore;
                bestTarget = target;
            }
        }

        if (bestTarget != null)
        {
            return new AIDecision
            {
                action = attackAction,
                targetPosition = bestTarget.GetGridPosition(),
                score = bestTargetScore
            };
        }

        return null;
    }

    private List<Units> GetTargetableUnitsFromPosition(GridPosition fromPosition, BaseAction attackAction)
    {
        List<Units> targetableUnits = new List<Units>();
        
        if (PathFinding.Instance == null)
        {
            return targetableUnits;
        }

        // 根据攻击类型获取范围
        int range = 0;
        if (attackAction is ShootAction shootAction)
        {
            range = shootAction.GetShootRange();
        }
        else if (attackAction is SwordAction swordAction)
        {
            range = swordAction.GetMaxSwordDistance();
        }
        else if (attackAction is GrenadeAction grenadeAction)
        {
            range = grenadeAction.GetGrenadeRange();
        }

        // 扫描范围内的敌人
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testPos = fromPosition + new GridPosition(x, z);
                
                if (!LevelGrid.Instance.IsValidGridPosition(testPos))
                {
                    continue;
                }

                Units targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testPos);
                if (targetUnit != null && !targetUnit.IsEnemyUnit())
                {
                    // 检查视线（仅对远程攻击）
                    if (attackAction is ShootAction || attackAction is GrenadeAction)
                    {
                        if (PathFinding.Instance.HasLineOfSight(fromPosition, testPos))
                        {
                            targetableUnits.Add(targetUnit);
                        }
                    }
                    else
                    {
                        targetableUnits.Add(targetUnit);
                    }
                }
            }
        }

        return targetableUnits;
    }

    private float CalculateTargetScore(Units targetUnit, BaseAction attackAction)
    {
        float score = 100f;

        // 获取目标的生命值系统
        HealthSystem targetHealth = targetUnit.GetComponent<HealthSystem>();
        if (targetHealth != null)
        {
            float healthPercent = targetHealth.GetHealthNormalized();
            
            // 优先攻击低血量目标（容易击杀）
            score += (1f - healthPercent) * 50f;

            // 预测伤害
            int predictedDamage = GetActionDamage(attackAction);
            if (predictedDamage >= targetHealth.GetHealth())
            {
                // 可以击杀，给予更高分数
                score += 100f;
            }
        }

        // 距离惩罚（优先目标近的）
        float distance = Vector3.Distance(attackAction.GetComponent<Transform>().position, targetUnit.GetWorldPosition());
        score += (10f - distance) * 2f;

        return score;
    }

    private int GetActionDamage(BaseAction attackAction)
    {
        if (attackAction is ShootAction shootAction)
        {
            return shootAction.GetShootDamage();
        }
        else if (attackAction is SwordAction swordAction)
        {
            return 50; // 默认近战伤害
        }
        else if (attackAction is GrenadeAction)
        {
            return 30; // 默认手雷伤害
        }
        return 0;
    }

    private void ExecuteDecision(Units enemyUnit, AIDecision decision, Action onComplete)
    {
        if (decision.movePosition.HasValue)
        {
            // 需要先移动
            MoveAction moveAction = enemyUnit.GetComponent<MoveAction>();
            if (moveAction != null && enemyUnit.TrySpendActionPointsToTakeAction(moveAction))
            {
                moveAction.TakeAction(decision.movePosition.Value, () =>
                {
                    // 移动完成后攻击
                    if (enemyUnit.TrySpendActionPointsToTakeAction(decision.action))
                    {
                        decision.action.TakeAction(decision.targetPosition, onComplete);
                    }
                    else
                    {
                        onComplete?.Invoke();
                    }
                });
                return;
            }
        }

        // 直接攻击
        if (enemyUnit.TrySpendActionPointsToTakeAction(decision.action))
        {
            decision.action.TakeAction(decision.targetPosition, onComplete);
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private class AIDecision
    {
        public BaseAction action;
        public GridPosition targetPosition;
        public GridPosition? movePosition;
        public float score;
    }
}
