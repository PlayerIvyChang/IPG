using UnityEngine;
using System.Collections.Generic;

public class DealDamageEffect : Effects
{
    [SerializeField] private int damageAmount;

    public override GameAction GetGameAction(List<CombatView> targets)
    {
        DealDamageGA dealDamageGA = new(damageAmount, targets);
        return dealDamageGA;
    }
}
