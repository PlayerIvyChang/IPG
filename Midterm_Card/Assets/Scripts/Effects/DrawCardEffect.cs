using System.Collections.Generic;
using UnityEngine;

public class DrawCardEffect : Effects
{
    [SerializeField] private int drawAmount;
    public override GameAction GetGameAction(List<CombatView> targets)
    {
        DrawCardGA drawCardGA = new (drawAmount);
        return drawCardGA;
    }
}
