using UnityEngine;

public class PathfindingGridSystem
{
    private int width;
    private int height;
    private float cellSize;
    private PathNode[,] gridObjectArray;
    private Vector3 originPosition;

    public PathfindingGridSystem(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        // 计算原点位置，使网格居中
        originPosition = new Vector3(-width * cellSize / 2f, 0, -height * cellSize / 2f);

        gridObjectArray = new PathNode[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                gridObjectArray[x, z] = new PathNode(gridPosition);
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize + originPosition;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - originPosition;
        return new GridPosition(
            Mathf.RoundToInt(localPosition.x / cellSize),
            Mathf.RoundToInt(localPosition.z / cellSize)
        );
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                PathFindingDebugObject gridDebugObject = debugTransform.GetComponent<PathFindingDebugObject>();
                gridDebugObject.SetPathNode(GetNode(gridPosition));
            }
        }
    }

    public PathNode GetNode(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.z];
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.z >= 0 && gridPosition.x < width && gridPosition.z < height;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }
}