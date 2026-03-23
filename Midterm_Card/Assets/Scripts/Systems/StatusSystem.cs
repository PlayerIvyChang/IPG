using System.Collections;
using UnityEngine;

public class StatusSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<AddStatusGA>(AddStatusPerformer);
    }
    
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<AddStatusGA>();
    }
    
    public IEnumerator AddStatusPerformer(AddStatusGA addStatusGA)
    {
        if (addStatusGA.Targets != null)
        {
            foreach (var target in addStatusGA.Targets)
            {
                if (target != null)
                {
                    target.AddStatusEffect(addStatusGA.StatusEffectType, addStatusGA.StackCount);
                }
            }
        }
        yield return null;
    }
}
