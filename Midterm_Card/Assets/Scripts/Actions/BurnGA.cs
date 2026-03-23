using UnityEngine;

public class BurnGA : GameAction
{
    public int BurnDamage { get; private set; }
    public CombatView Target { get; private set; }

    public BurnGA(CombatView target, int burnDamage)
    {
        Target = target;
        BurnDamage = burnDamage;
    }
}
