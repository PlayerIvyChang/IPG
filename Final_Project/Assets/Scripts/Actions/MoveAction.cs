using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveAction : BaseAction
{
    private List<Vector3> pathPositions;
    private int currentPathIndex;
    private System.Action onActionComplete;
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    [SerializeField] private int maxMoveDistance = 4;
    [SerializeField] private int actionPointsCost = 1;
    
    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        if (pathPositions == null || pathPositions.Count == 0)
        {
            isActive = false;
            OnStopMoving?.Invoke(this, EventArgs.Empty);
            onActionComplete?.Invoke();
            return;
        }

        Vector3 targetPosition = pathPositions[currentPathIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float stoppingDistance = 0.1f;
        
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {            
            float moveSpeed = 5f;
            transform.position += moveDirection * Time.deltaTime * moveSpeed;
        }
        else
        {
            currentPathIndex++;
            
            if (currentPathIndex >= pathPositions.Count)
            {
                isActive = false;
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                onActionComplete?.Invoke();
            }
        }

        float rotateSpeed = 10f;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
    }
    
    public override void TakeAction(GridPosition gridPosition, System.Action onActionComplete)
    {
        if (!CanTakeAction())
        {
            onActionComplete?.Invoke();
            return;
        }

        this.onActionComplete = onActionComplete;

        List<GridPosition> path = PathFinding.Instance.FindPath(unit.GetGridPosition(), gridPosition);
        
        if (path == null || path.Count == 0)
        {
            onActionComplete?.Invoke();
            return;
        }

        pathPositions = new List<Vector3>();
        foreach (GridPosition pathGridPosition in path)
        {
            pathPositions.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        currentPathIndex = 0;
        isActive = true;
        OnStartMoving?.Invoke(this, EventArgs.Empty);
    }

    public override List<GridPosition> GetValidActionGridPositions()
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();
        
        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                
                if (testGridPosition == unitGridPosition)
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxMoveDistance)
                {
                    continue;
                }
                
                if (LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition) != null)
                {
                    continue;
                }

                if (!PathFinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }

                if (!PathFinding.Instance.HasLineOfSight(unitGridPosition, testGridPosition))
                {
                    continue;
                }

                validGridPositions.Add(testGridPosition);
            }
        }
        return validGridPositions;
    }

    public override string GetActionName()
    {
        return "Move";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = 0;

        ShootAction shootAction = unit.GetComponent<ShootAction>();
        if (shootAction != null && shootAction.enabled)
        {
            targetCountAtGridPosition = shootAction.GetTargetCountAtPosition(gridPosition);
        }

        EnemyAIAction enemyAIAction = new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10
        };
        return enemyAIAction;
    }

    public void SetMoveDistance(int distance)
    {
        maxMoveDistance = distance;
    }

    public int GetMoveDistance()
    {
        return maxMoveDistance;
    }

    public override int GetActionPointsCost()
    {
        return actionPointsCost;
    }
}
