using UnityEngine;

public class EffectGA : GameAction
{
    public Effects Effect { get; set; }
    public EffectGA(Effects effect)
    {
        Effect = effect;
    }
}
