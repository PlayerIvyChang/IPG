using UnityEngine;
using System.Collections.Generic;

public class AddStatusEffect : Effects
{
    [SerializeField] private StatusEffectType statusEffectType;
    [SerializeField] private int stackCount;

    public GameAction GetGameAction(List<CombatView> targets, CombatView _)
    {
        return new AddStatusGA(statusEffectType, stackCount, targets);
    }

    public override GameAction GetGameAction(List<CombatView> targets)
    {
        return GetGameAction(targets, null);
    }
}
