using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler OnActionStarted;

    [SerializeField] private Units selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private BaseAction selectedAction;
    private bool isBusy;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        SetSelectedUnit(selectedUnit);
    }
    private void Update()
    {
        if (isBusy)
        {
            return;
        }

        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (TryHandleUnitSelection())
            {
                return;
            }

        }
        
        HandleSelectedAction();
    }
    private void SetBusy()
    {
        isBusy = true;
    }
    private void ClearBusy()
    {
        isBusy = false; 
    }
    private bool TryHandleUnitSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Units>(out Units unit))
                {
                    if(unit == selectedUnit)
                    {
                        return false; // Clicked on the already selected unit, do nothing
                    }

                    if (unit.IsEnemyUnit())
                    {
                        return false; // Clicked on an enemy unit, do nothing
                    }
                    SetSelectedUnit(unit); // Select the unit that was clicked on
                    return true;
                }
                
            }
        }
        return false;
    }
    private void HandleSelectedAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

            if (!LevelGrid.Instance.IsValidGridPosition(mouseGridPosition))
            {
                return;
            }
            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition))
            {
                return;
            }

            // 尝试消耗行动点并执行动作
            if (selectedUnit.TrySpendActionPointsToTakeAction(selectedAction))
            {
                SetBusy();  
                selectedAction.TakeAction(mouseGridPosition, ClearBusy);
            }

            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }
    private void SetSelectedUnit(Units unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetMoveAction());
        
        if (selectedUnit != null)
        {
            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public void SetSelectedAction(BaseAction baseAction)
    {
        selectedAction = baseAction;
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }
    public Units GetSelectedUnit()
    {
        return selectedUnit;
    }
    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }
}
