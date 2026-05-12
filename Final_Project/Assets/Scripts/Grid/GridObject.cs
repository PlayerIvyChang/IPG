using UnityEngine;
using System;
using System.Collections.Generic;

public class GridObject
{
    private GridSystem gridSystem;
    private GridPosition gridPosition;
    private List<Units> units;
    
    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        units = new List<Units>();
    }
    
    public override string ToString()
    {
        string unitString = "";
        foreach (Units unit in units)
        {
            unitString += unit + "\n";
        }
        return gridPosition.ToString() + "\n" + unitString;
    }
    
    public void AddUnit(Units unit)
    {
        units.Add(unit);
    }
    
    public void RemoveUnit(Units unit)
    {
        units.Remove(unit);
    }
    
    public List<Units> GetUnitList()
    {
        return units;
    }
    
    public bool HasAnyUnit()
    {
        return units.Count > 0;
    }
    
    public Units GetUnits()
    {
        if (HasAnyUnit())
        {
            return units[0];
        }
        else
        {
            return null;
        }
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }
    
    protected GridPosition GetGridPositionCore()
    {
        return gridPosition;
    }
}
