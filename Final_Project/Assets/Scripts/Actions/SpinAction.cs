using UnityEngine;
using System.Collections.Generic;
using System;

public class SpinAction : BaseAction
{
    private float spinAmount;
    private System.Action onActionComplete;
    
    [SerializeField] private int actionPointsCost = 1;
    
    protected override void Awake()
    {
        base.Awake();
    }
    
    private void Update()
    {
        if (isActive)
        {
            float spinSpeed = 360f * Time.deltaTime;
            transform.eulerAngles += new Vector3(0, spinSpeed, 0);
            spinAmount += spinSpeed;
            if (spinAmount >= 360f)
            {
                isActive = false;
                onActionComplete?.Invoke();
            }
        }
    }

    public override void TakeAction(GridPosition gridPosition, System.Action onActionComplete)
    {
        if (!CanTakeAction())
        {
            return;
        }

        this.onActionComplete = onActionComplete;
        isActive = true;
        spinAmount = 0f;
    }

    public override string GetActionName()
    {
        return "Spin";
    }

    public override List<GridPosition> GetValidActionGridPositions()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return new List<GridPosition> { unitGridPosition };
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }

    public override int GetActionPointsCost()
    {
        return actionPointsCost;
    }
}