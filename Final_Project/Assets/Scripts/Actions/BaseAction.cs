using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    protected Units unit;
    protected bool isActive;

    protected virtual void Awake()
    {
        unit = GetComponent<Units>();
    }

    public bool IsActive()
    {
        return isActive;
    }

    protected bool CanTakeAction()
    {
        // 检查是否有任何其他动作正在执行
        BaseAction[] allActions = GetComponents<BaseAction>();
        foreach (BaseAction action in allActions)
        {
            if (action != this && action.IsActive())
            {
                return false;
            }
        }
        return true;
    }
    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition gridPosition, System.Action onActionComplete);

    public abstract List<GridPosition> GetValidActionGridPositions();

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositions = GetValidActionGridPositions();
        return validGridPositions.Contains(gridPosition);
    }

    public virtual int GetActionPointsCost()
    {
        return 1;
    }

    public EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();
        List<GridPosition> validGridPositions = GetValidActionGridPositions();

        foreach (GridPosition validGridPosition in validGridPositions)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(validGridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }

        if (enemyAIActionList.Count == 0)
        {
            return null;
        }

        enemyAIActionList.Sort((a, b) => b.actionValue.CompareTo(a.actionValue));
        return enemyAIActionList[0];
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);
}
