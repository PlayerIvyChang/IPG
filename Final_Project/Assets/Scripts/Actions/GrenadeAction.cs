using System;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField] private Transform grenadeProjectilePrefab;
    [SerializeField] private int maxThrowDistance = 3;
    [SerializeField] private int actionPointsCost = 2;
    
    private Action onActionComplete;
    
    private void Update()
    {
        if (!isActive)
        {
            return;
        }
    }

    public override string GetActionName()
    {
        return "Grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }

    public override List<GridPosition> GetValidActionGridPositions()
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++)
        {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                // 计算实际距离(菱形范围)
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxThrowDistance)
                {
                    continue;
                }


                validGridPositions.Add(testGridPosition);
            }
        }

        return validGridPositions;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        if (!CanTakeAction())
        {
            return;
        }

        this.onActionComplete = onActionComplete;
        
        // 投掷手榴弹
        Transform grenadeProjectileTransform = Instantiate(
            grenadeProjectilePrefab,
            unit.GetWorldPosition(),
            Quaternion.identity
        );

        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(gridPosition, OnGrenadeBehaviourComplete);
        
        isActive = true;
    }

    private void OnGrenadeBehaviourComplete()
    {
        isActive = false;
        onActionComplete?.Invoke();
    }

    public override int GetActionPointsCost()
    {
        return actionPointsCost;
    }

    public void SetGrenadeRange(int range)
    {
        maxThrowDistance = range;
    }

    public int GetGrenadeRange()
    {
        return maxThrowDistance;
    }
}
