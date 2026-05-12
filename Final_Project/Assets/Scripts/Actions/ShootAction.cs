using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    public EventHandler OnShootComplete;
    public class OnShootCompleteEventArgs : EventArgs
    {
        public Units targetUnit;
        public Units shootingUnit;
    }
    private enum State
    {
        Aiming,
        Shooting,
        Cooloff
    }
    private State state;

    private Action onActionComplete; 
    private int maxShootDistance = 5;
    private int shootDamage = 20;
    private float stateTimer;
    private Units targetUnit;
    private bool CanShootBullet;

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

        stateTimer -= Time.deltaTime;
        switch (state)
        {
            case State.Aiming:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDir.normalized, Time.deltaTime * rotateSpeed);
                break;
            case State.Shooting:
                if (CanShootBullet)
                {
                    Shoot();
                    CanShootBullet = false;
                }
                break;
            case State.Cooloff:
                break;
        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.Aiming:
                state = State.Shooting;
                stateTimer = 0.1f;
                break;
            case State.Shooting:
                state = State.Cooloff;
                stateTimer = 0.5f;
                break;
            case State.Cooloff:
                isActive = false;
                onActionComplete?.Invoke();
                break;
        }
    }

    private void Shoot()
    {
        OnShootComplete?.Invoke(this, new OnShootCompleteEventArgs
        {
            targetUnit = targetUnit,
            shootingUnit = unit
        });
        if (targetUnit != null)
        {
            targetUnit.Damage(shootDamage);
        }
    }

    public override string GetActionName()
    {
        return "Shoot";
    }

    public override List<GridPosition> GetValidActionGridPositions()
    {
        GridPosition unitGridPosition = unit.GetGridPosition();
        return GetValidActionGridPositions(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositions(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();
        
        for (int x = -maxShootDistance; x <= maxShootDistance; x++)
        {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;
                
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }
                
                if (testGridPosition == gridPosition)
                {
                    continue;
                }

                // ¼ÆËãÊµ¼Ê¾àÀë
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxShootDistance)
                {
                    continue;
                }

                Units targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if (targetUnit == null)
                {
                    continue;
                }
                
                if (targetUnit.IsEnemyUnit() == unit.IsEnemyUnit())
                {
                    continue;
                }

                if (!PathFinding.Instance.HasLineOfSight(gridPosition, testGridPosition))
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
        isActive = true;

        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        state = State.Aiming;
        stateTimer = 1f;
        CanShootBullet = true;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Units targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        EnemyAIAction enemyAIAction = new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 100,
        };
        return enemyAIAction;
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositions(gridPosition).Count;
    }

    public void SetShootRange(int range)
    {
        maxShootDistance = range;
    }

    public void SetShootDamage(int damage)
    {
        shootDamage = damage;
    }

    public int GetShootRange()
    {
        return maxShootDistance;
    }

    public int GetShootDamage()
    {
        return shootDamage;
    }
}
