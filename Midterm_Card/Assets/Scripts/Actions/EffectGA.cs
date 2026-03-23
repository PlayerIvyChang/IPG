using UnityEngine;
using System.Collections.Generic;

public class EffectGA : GameAction
{
    public Effects Effect { get; set; }
    public List<CombatView> Targets { get; set; }
    public EffectGA(Effects effect, List<CombatView> targets)
    {
        Effect = effect;
        Targets = targets == null? null: new(targets);
    }
}
