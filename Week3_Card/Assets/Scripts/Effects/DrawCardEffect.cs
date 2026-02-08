using UnityEngine;

public class DrawCardEffect : Effects
{
    [SerializeField] private int drawAmount;
    public override GameAction GetGameAction()
    {
        DrawCardGA drawCardGA = new (drawAmount);
        return drawCardGA;
    }
}
