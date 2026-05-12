using System;
using UnityEngine;

public class PathFindingUpdater : MonoBehaviour
{
    private void Awake()
    {
        DestructableCrate.OnAnyCrateDestroyed += DestructableCrate_OnAnyCrateDestroyed;
    }

    private void OnDestroy()
    {
        DestructableCrate.OnAnyCrateDestroyed -= DestructableCrate_OnAnyCrateDestroyed;
    }

    private void DestructableCrate_OnAnyCrateDestroyed(object sender, DestructableCrate destructableCrate)
    {
        if (destructableCrate != null)
        {
            GridPosition cratePosition = destructableCrate.GetGridPosition();
            
            
            // 将箱子所在位置设置为可行走
            PathFinding.Instance.SetIsWalkableGridPosition(cratePosition, true);
            
        }
    }
}
