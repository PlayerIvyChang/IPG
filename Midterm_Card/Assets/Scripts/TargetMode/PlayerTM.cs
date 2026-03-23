using UnityEngine;
using System.Collections.Generic;

public class PlayerTM : TargetMode
{
    public override List<CombatView> GetTargets()
    {
        List<CombatView> targets = new()
        {
            PlayerSystem.Instance.PlayerView
        };
        return targets;
    }
}
