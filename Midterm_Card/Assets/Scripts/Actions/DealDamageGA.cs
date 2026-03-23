using UnityEngine;
using System.Collections.Generic;

public class DealDamageGA : GameAction
{
    public int Amount { get; set; }
    public List<CombatView> Targets { get; set; }
    public DealDamageGA(int amount, List<CombatView> targets)
    {
        Amount = amount;
        Targets = new(targets);
    }
}
