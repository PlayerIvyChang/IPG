using UnityEngine;
using System.Collections.Generic;
public class RandomEnemyTM : TargetMode
{
    public override List<CombatView> GetTargets()
    {
        CombatView target = EnemySystem.Instance.Enemies[Random.Range(0,EnemySystem.Instance.Enemies.Count)];
        return new() { target };
    }
}
