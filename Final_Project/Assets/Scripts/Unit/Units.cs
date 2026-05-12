using UnityEngine;
using System;

public class Units : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 3;

    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    [Header("Unit Configuration")]
    [SerializeField] private UnitType unitType;
    [SerializeField] private Animator unitAnimator;
    [SerializeField] private bool IsEnemy;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private MoveAction moveAction;
    private SpinAction spinAction;
    private ShootAction shootAction;
    private SwordAction swordAction;
    private GrenadeAction grenadeAction;
    private BaseAction[] baseActions;
    private int actionPoints = ACTION_POINTS_MAX;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        shootAction = GetComponent<ShootAction>();
        swordAction = GetComponent<SwordAction>();
        grenadeAction = GetComponent<GrenadeAction>();
        baseActions = GetComponents<BaseAction>();

        // Ó¦ÓÃ UnitType ÅäÖÃ
        if (unitType != null)
        {
            ApplyUnitType();
        }
    }

    private void ApplyUnitType()
    {
        // ÅäÖÃÉúÃüÖµ
        if (healthSystem != null)
        {
            healthSystem.SetMaxHealth(unitType.maxHealth);
        }

        // ÅäÖÃÒÆ¶¯¾àÀë
        if (moveAction != null)
        {
            moveAction.SetMoveDistance(unitType.moveDistance);
        }

        // ÅäÖÃÉä»÷
        if (shootAction != null)
        {
            shootAction.SetShootRange(unitType.shootRange);
            shootAction.SetShootDamage(unitType.shootDamage);
            shootAction.enabled = unitType.hasShootAction;
        }

        // ÅäÖÃ½üÕ½
        if (swordAction != null)
        {
            swordAction.SetSwordRange(unitType.swordRange);
            swordAction.SetSwordDamage(unitType.swordDamage);
            swordAction.enabled = unitType.hasSwordAction;
        }

        // ÅäÖÃÊÖÁñµ¯
        if (grenadeAction != null)
        {
            grenadeAction.SetGrenadeRange(unitType.grenadeRange);
            grenadeAction.enabled = unitType.hasGrenadeAction;
        }
    }

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this); 
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChanged;

        healthSystem.OnDie += HealthSystem_OnDie;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            LevelGrid.Instance.UnitMoveGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }

    public MoveAction GetMoveAction()
    {
        return moveAction;
    }

    public SpinAction GetSpinAction()
    {
        return spinAction;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public BaseAction[] GetBaseActions()
    {
        return baseActions;
    }
    public ShootAction GetShootAction()
    {
        return shootAction;
    }

    public UnitType GetUnitType()
    {
        return unitType;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointsToTakeActioin(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanSpendActionPointsToTakeActioin(BaseAction baseAction)
    {
        if (actionPoints >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpendActionPoints(int amount)
    {
        actionPoints -= amount;
        OnActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public int GetActionPoints()
    {
        return actionPoints;
    }

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e)
    {
        if ((IsEnemy && !TurnSystem.Instance.IsPlayerTurn()) || (!IsEnemy && TurnSystem.Instance.IsPlayerTurn()))
            actionPoints = ACTION_POINTS_MAX;

        OnActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsEnemyUnit()
    {
        return IsEnemy;
    }

    public void Damage(int damageAmount)
    {
        healthSystem.Damage(damageAmount);
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    private void HealthSystem_OnDie(object sender, System.EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        Destroy(gameObject);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler OnActionPointsChanged;
}
