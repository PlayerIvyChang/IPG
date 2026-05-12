using UnityEngine;
using System.Collections.Generic;

public class GridVisual : MonoBehaviour
{
    public static GridVisual Instance { get; private set; }
    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    private GridVisualSingle[,] gridVisualSingleArray;
    private bool isInitialized = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one GridVisual! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    private void Start()
    {
        gridVisualSingleArray = new GridVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];
        for (int i = 0; i < LevelGrid.Instance.GetWidth(); i++)
        {
            for (int j = 0; j < LevelGrid.Instance.GetHeight(); j++)
            {
                GridPosition gridPosition = new GridPosition(i, j);
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                gridVisualSingleArray[i, j] = gridSystemVisualSingleTransform.GetComponent<GridVisualSingle>();
            }
        }

        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        isInitialized = true;
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, System.EventArgs e)
    {
        UpdateGridVisual();
    }

    private void Update()
    {
        if (!isInitialized)
        {
            return;
        }
        UpdateGridVisual();
    }

    private void UpdateGridVisual()
    {
        if (!isInitialized)
        {
            return;
        }
        
        HideAllGridPosition();

        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        if (selectedAction == null)
        {
            return;
        }

        ShowGridPositionList(selectedAction.GetValidActionGridPositions());
    }

    public void HideAllGridPosition()
    {
        if (gridVisualSingleArray == null || !isInitialized)
        {
            return;
        }
        
        for (int i = 0; i < LevelGrid.Instance.GetWidth(); i++)
        {
            for (int j = 0; j < LevelGrid.Instance.GetHeight(); j++)
            {
                gridVisualSingleArray[i, j].Hide();
            }
        }
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList)
    {
        if (gridVisualSingleArray == null || !isInitialized)
        {
            return;
        }
        
        foreach (GridPosition gridPosition in gridPositionList)
        {
            if (!IsValidGridVisualPosition(gridPosition))
            {
                Debug.LogWarning($"Trying to show invalid grid position: {gridPosition}. Grid size: ({LevelGrid.Instance.GetWidth()}, {LevelGrid.Instance.GetHeight()})");
                continue;
            }
            gridVisualSingleArray[gridPosition.x, gridPosition.z].Show(Color.white);
        }
    }
    
    private bool IsValidGridVisualPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 && 
               gridPosition.z >= 0 && 
               gridPosition.x < LevelGrid.Instance.GetWidth() && 
               gridPosition.z < LevelGrid.Instance.GetHeight();
    }
}
