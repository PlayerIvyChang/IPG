using System;
using UnityEngine;

public class PathNode
{
    private int gCost;
    private int hCost;
    private int fCost;
    private PathNode cameFromNode;
    private bool isWalkable = true;
    private GridPosition gridPosition;

    public PathNode(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public override string ToString()
    {
        return gridPosition.ToString() + "\n" + (isWalkable ? "Walkable" : "Blocked");
    }

    public int GetGCost()
    {
        return gCost;
    }

    public int GetHCost()
    {
        return hCost;
    }

    public int GetFCost()
    {
        return fCost;
    }
    
    public void SetGCost(int gCost)
    {
        this.gCost = gCost;
    }
    
    public void SetHCost(int hCost)
    {
        this.hCost = hCost;
    }
    
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
    
    public void SetCameFromNode(PathNode node)
    {
        cameFromNode = node;
    }
    
    public PathNode GetCameFromNode()
    {
        return cameFromNode;
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }
    
    public bool IsWalkable()
    {
        return isWalkable;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }
}
