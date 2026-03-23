using UnityEngine;
using System.Collections.Generic;

public class AddStatusGA : GameAction
{
    public StatusEffectType StatusEffectType { get; private set; }
    public int StackCount { get; private set; }
    public List<CombatView> Targets { get; private set; }
    public AddStatusGA(StatusEffectType statusEffectType, int stackCount, List<CombatView> targets)
    {
        StatusEffectType = statusEffectType;
        StackCount = stackCount;
        Targets = targets;
    }
}
