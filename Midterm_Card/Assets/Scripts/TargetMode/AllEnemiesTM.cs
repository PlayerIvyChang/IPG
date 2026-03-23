using UnityEngine;
using System.Collections.Generic;

public class AllEnemiesTM : TargetMode
{
    public override List<CombatView> GetTargets()
    {
        return new(EnemySystem.Instance.Enemies);
    }
}
