using UnityEngine;
using System.Collections.Generic;

public class Testing : MonoBehaviour
{
    [SerializeField] private Units unit;
    
    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());
            GridPosition startGridPosition = new GridPosition(0, 0);

            List<GridPosition> gridPositionList = PathFinding.Instance.FindPath(startGridPosition, mouseGridPosition);
            
            if (gridPositionList == null || gridPositionList.Count == 0)
            {
                Debug.Log("No path found!");
                return;
            }
            
            for (int i = 0; i < gridPositionList.Count - 1; i++)
            {
                Debug.DrawLine(
                    LevelGrid.Instance.GetWorldPosition(gridPositionList[i]),
                    LevelGrid.Instance.GetWorldPosition(gridPositionList[i + 1]),
                    Color.green,
                    10f
                );
            }
        }
    }
}
