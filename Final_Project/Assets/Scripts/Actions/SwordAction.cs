using System;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;

    private enum State
    {
        SwingingSwordBeforeHit,
        SwingingSwordAfterHit,
    }

    [SerializeField] private int maxSwordDistance = 1;
    [SerializeField] private int damageAmount = 50;
    [SerializeField] private int actionPointsCost = 1;

    private State state;
    private float stateTimer;
    private Units targetUnit;
    private DestructableCrate targetCrate;
    private Action onActionComplete;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log($"SwordAction 初始化: 伤害值 = {damageAmount}");
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
            case State.SwingingSwordBeforeHit:
                Vector3 aimDir;
                if (targetUnit != null)
                {
                    aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                }
                else if (targetCrate != null)
                {
                    aimDir = (targetCrate.transform.position - unit.GetWorldPosition()).normalized;
                }
                else
                {
                    aimDir = transform.forward;
                }

                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, Time.deltaTime * rotateSpeed);
                break;
            case State.SwingingSwordAfterHit:
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
            case State.SwingingSwordBeforeHit:
                state = State.SwingingSwordAfterHit;
                float afterHitStateTime = 0.5f;
                stateTimer = afterHitStateTime;

                // 造成伤害
                if (targetUnit != null)
                {
                    Debug.Log($"<color=red>剑攻击造成 {damageAmount} 点伤害给 {targetUnit.name}</color>");
                    targetUnit.Damage(damageAmount);
                }
                else if (targetCrate != null)
                {
                    targetCrate.Damage();
                }

                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                break;
            case State.SwingingSwordAfterHit:
                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "Sword";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 200
        };
    }

    public override List<GridPosition> GetValidActionGridPositions()
    {
        List<GridPosition> validGridPositions = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxSwordDistance; x <= maxSwordDistance; x++)
        {
            for (int z = -maxSwordDistance; z <= maxSwordDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                // 跳过当前位置
                if (testGridPosition == unitGridPosition)
                {
                    continue;
                }

                Units foundUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if (foundUnit != null && foundUnit.IsEnemyUnit() != unit.IsEnemyUnit())
                {
                    validGridPositions.Add(testGridPosition);
                    continue;
                }

                // 检查是否有可破坏的箱子
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);
                Collider[] colliders = Physics.OverlapSphere(worldPosition, 0.5f);

                foreach (Collider collider in colliders)
                {
                    if (collider.TryGetComponent<DestructableCrate>(out DestructableCrate crate))
                    {
                        validGridPositions.Add(testGridPosition);
                        break;
                    }
                }
            }
        }

        return validGridPositions;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Debug.Log($"<color=cyan>SwordAction.TakeAction 被调用 - isActive={isActive}, CanTakeAction={CanTakeAction()}</color>");

        if (!CanTakeAction())
        {
            Debug.LogWarning("SwordAction: CanTakeAction() 返回 false!");
            return;
        }

        this.onActionComplete = onActionComplete;

        // 检查目标是单位还是箱子
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        targetCrate = null;

        if (targetUnit == null)
        {
            // 没有单位，检查箱子
            Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
            Collider[] colliders = Physics.OverlapSphere(worldPosition, 0.5f);

            foreach (Collider collider in colliders)
            {
                if (collider.TryGetComponent<DestructableCrate>(out DestructableCrate crate))
                {
                    targetCrate = crate;
                    break;
                }
            }
        }

        Debug.Log($"目标: Unit={targetUnit?.name ?? "null"}, Crate={targetCrate?.name ?? "null"}");

        state = State.SwingingSwordBeforeHit;
        float beforeHitStateTime = 0.7f;
        stateTimer = beforeHitStateTime;

        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);
        isActive = true;
        
        Debug.Log($"SwordAction 开始 - isActive={isActive}");
    }

    private void ActionComplete()
    {
        Debug.Log($"<color=green>SwordAction 完成 - 清理状态</color>");
        
        isActive = false;
        
        // 清理目标引用
        targetUnit = null;
        targetCrate = null;
        
        // 调用回调
        onActionComplete?.Invoke();
    }

    public override int GetActionPointsCost()
    {
        return actionPointsCost;
    }

    public int GetMaxSwordDistance()
    {
        return maxSwordDistance;
    }

    public void SetSwordRange(int range)
    {
        maxSwordDistance = range;
    }

    public void SetSwordDamage(int damage)
    {
        damageAmount = damage;
    }
}
