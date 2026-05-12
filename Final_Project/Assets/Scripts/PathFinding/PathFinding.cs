using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PathFinding : MonoBehaviour
{
    [SerializeField] private Transform gridDebugObjectPrefab;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private bool showDebugObjects = false;

    public static PathFinding Instance { get; private set; }

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private int width;
    private int height;
    private float cellSize;
    private PathfindingGridSystem gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one PathFinding! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetUp(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        gridSystem = new PathfindingGridSystem(width, height, cellSize);

        if (showDebugObjects && gridDebugObjectPrefab != null)
        {
            gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
        }

        // 扫描障碍物,标记不可行走的格子
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                float raycastOffsetDistance = 5f;

                if (Physics.Raycast(
                    worldPosition + Vector3.up * raycastOffsetDistance,
                    Vector3.down,
                    raycastOffsetDistance * 2,
                    obstacleLayerMask))
                {
                    GetNode(x, z).SetIsWalkable(false);
                }
            }
        }
    }

    public bool HasLineOfSight(Vector3 fromPosition, Vector3 toPosition)
    {
        Vector3 rayOrigin = fromPosition + Vector3.up * 1f;
        Vector3 rayTarget = toPosition + Vector3.up * 1f;
        
        Vector3 direction = (rayTarget - rayOrigin).normalized;
        float distance = Vector3.Distance(rayOrigin, rayTarget);

        if (Physics.Raycast(rayOrigin, direction, distance - 0.1f, obstacleLayerMask))
        {
            return false;
        }

        return true;
    }

    public bool HasLineOfSight(GridPosition fromGridPosition, GridPosition toGridPosition)
    {
        Vector3 fromWorldPosition = LevelGrid.Instance.GetWorldPosition(fromGridPosition);
        Vector3 toWorldPosition = LevelGrid.Instance.GetWorldPosition(toGridPosition);
        return HasLineOfSight(fromWorldPosition, toWorldPosition);
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = gridSystem.GetNode(startGridPosition);
        PathNode endNode = gridSystem.GetNode(endGridPosition);

        if (startNode == null || endNode == null)
        {
            pathLength = 0;
            return null;
        }

        if (!startNode.IsWalkable())
        {
            pathLength = 0;
            return null;
        }

        if (!endNode.IsWalkable())
        {
            pathLength = 0;
            return null;
        }

        openList.Add(startNode);

        for (int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for (int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetNode(gridPosition);
                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.CalculateFCost();
                pathNode.SetCameFromNode(null);
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistanceCost(startGridPosition, endGridPosition));
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode)
            {
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode))
                {
                    continue;
                }

                if (!neighbourNode.IsWalkable())
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() +
                    CalculateDistanceCost(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                if (tentativeGCost < neighbourNode.GetGCost())
                {
                    neighbourNode.SetCameFromNode(currentNode);
                    neighbourNode.SetGCost(tentativeGCost);
                    neighbourNode.SetHCost(CalculateDistanceCost(neighbourNode.GetGridPosition(), endGridPosition));
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        pathLength = 0;
        return null;
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength);
    }

    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        if (gridSystem == null)
        {
            return false;
        }

        if (!gridSystem.IsValidGridPosition(gridPosition))
        {
            return false;
        }

        PathNode node = gridSystem.GetNode(gridPosition);
        if (node == null)
        {
            return false;
        }

        return node.IsWalkable();
    }

    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        return FindPath(startGridPosition, endGridPosition, out int pathLength) != null;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out int pathLength);
        return pathLength;
    }

    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool isWalkable)
    {
        if (gridSystem.IsValidGridPosition(gridPosition))
        {
            gridSystem.GetNode(gridPosition).SetIsWalkable(isWalkable);
        }
    }

    public int CalculateDistanceCost(GridPosition a, GridPosition b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int zDistance = Mathf.Abs(a.z - b.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].GetFCost() < lowestFCostNode.GetFCost())
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    private PathNode GetNode(int x, int z)
    {
        return gridSystem.GetNode(new GridPosition(x, z));
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();
        GridPosition gridPosition = currentNode.GetGridPosition();

        if (gridSystem.IsValidGridPosition(new GridPosition(gridPosition.x - 1, gridPosition.z)))
        {
            neighbourList.Add(gridSystem.GetNode(new GridPosition(gridPosition.x - 1, gridPosition.z)));
        }
        if (gridSystem.IsValidGridPosition(new GridPosition(gridPosition.x + 1, gridPosition.z)))
        {
            neighbourList.Add(gridSystem.GetNode(new GridPosition(gridPosition.x + 1, gridPosition.z)));
        }
        if (gridSystem.IsValidGridPosition(new GridPosition(gridPosition.x, gridPosition.z - 1)))
        {
            neighbourList.Add(gridSystem.GetNode(new GridPosition(gridPosition.x, gridPosition.z - 1)));
        }
        if (gridSystem.IsValidGridPosition(new GridPosition(gridPosition.x, gridPosition.z + 1)))
        {
            neighbourList.Add(gridSystem.GetNode(new GridPosition(gridPosition.x, gridPosition.z + 1)));
        }

        // 对角线 (可选 - 如果你需要8方向移动)
        if (gridSystem.IsValidGridPosition(new GridPosition(gridPosition.x - 1, gridPosition.z - 1)))
        {
            neighbourList.Add(gridSystem.GetNode(new GridPosition(gridPosition.x - 1, gridPosition.z - 1)));
        }
        if (gridSystem.IsValidGridPosition(new GridPosition(gridPosition.x - 1, gridPosition.z + 1)))
        {
            neighbourList.Add(gridSystem.GetNode(new GridPosition(gridPosition.x - 1, gridPosition.z + 1)));
        }
        if (gridSystem.IsValidGridPosition(new GridPosition(gridPosition.x + 1, gridPosition.z - 1)))
        {
            neighbourList.Add(gridSystem.GetNode(new GridPosition(gridPosition.x + 1, gridPosition.z - 1)));
        }
        if (gridSystem.IsValidGridPosition(new GridPosition(gridPosition.x + 1, gridPosition.z + 1)))
        {
            neighbourList.Add(gridSystem.GetNode(new GridPosition(gridPosition.x + 1, gridPosition.z + 1)));
        }

        return neighbourList;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<GridPosition> path = new List<GridPosition>();
        path.Add(endNode.GetGridPosition());
        PathNode currentNode = endNode;
        
        while (currentNode.GetCameFromNode() != null)
        {
            currentNode = currentNode.GetCameFromNode();
            path.Add(currentNode.GetGridPosition());
        }
        
        path.Reverse();
        return path;
    }
}
