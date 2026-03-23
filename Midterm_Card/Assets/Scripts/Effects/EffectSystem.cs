using UnityEngine;
using System.Collections;

public class EffectSystem : MonoBehaviour
{
    void OnEnable()
    {
        ActionSystem.AttachPerformer<EffectGA>(EffectPerformer);
    }

    void OnDisable()
    {
        ActionSystem.DetachPerformer<EffectGA>();
    }

    private IEnumerator EffectPerformer(EffectGA effectGA)
    {
        if (effectGA.Effect != null)
        {
            GameAction effectAction = effectGA.Effect.GetGameAction(effectGA.Targets);
            if (effectAction != null)
            {
                ActionSystem.Instance.AddReaction(effectAction);
            }
        }
        yield return null;
    }
}
