using System;
using UnityEngine;

public class DestructableCrate : MonoBehaviour
{
    public static event EventHandler<DestructableCrate> OnAnyCrateDestroyed;
    
    private GridPosition gridPosition;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        
        // 在初始化时将箱子位置设置为不可行走
        PathFinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
        
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public void Damage()
    {
        
        // 先触发事件，再销毁
        OnAnyCrateDestroyed?.Invoke(this, this);
        
        // 销毁游戏对象
        Destroy(gameObject);
    }
}
