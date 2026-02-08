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
        GameAction effectAction = effectGA.Effect.GetGameAction();
        ActionSystem.Instance.AddReaction(effectAction);
        yield return null;
    }
}
