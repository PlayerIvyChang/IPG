using UnityEngine;
using System.Collections.Generic;

public class LevelGrid : MonoBehaviour
{
    [SerializeField] private Transform gridDebugObjectPrefab;

    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    public static LevelGrid Instance { get; private set; }
    private GridSystem gridSystem;
    
    private void Awake()
    {
        Instance = this;
        gridSystem = new GridSystem(width, height, cellSize);
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }
    private void Start()
    {
        PathFinding.Instance.SetUp(width, height, cellSize);
    }
    
    public void AddUnitAtGridPosition(GridPosition gridPosition, Units unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        if (gridObject != null)
        {
            gridObject.AddUnit(unit);
        }
    }
    public List<Units> GetUnitListGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        if (gridObject != null)
        {
            return gridObject.GetUnitList();
        }
        return null;
    }
    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Units unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }
    public void UnitMoveGridPosition(Units unit, GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        RemoveUnitAtGridPosition(fromGridPosition, unit);
        AddUnitAtGridPosition(toGridPosition, unit);
    }
    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return gridSystem.GetGridPosition(worldPosition);
    }
    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return gridSystem.GetWorldPosition(gridPosition);
    }
    public int GetWidth()
    {
        return gridSystem.GetWidth();
    }
    public int GetHeight()
    {
        return gridSystem.GetHeight();
    }
    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridSystem.IsValidGridPosition(gridPosition);
    }
    public Units GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);

        return gridObject.GetUnits();
    }
}
