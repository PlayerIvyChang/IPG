using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public abstract class Effects
{
    public abstract GameAction GetGameAction(List<CombatView> targets);

}
