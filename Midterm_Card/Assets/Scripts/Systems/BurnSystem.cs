using System.Collections;
using UnityEngine;

public class BurnSystem : MonoBehaviour
{
    [SerializeField] private GameObject burnVFX;

    public void OnEnable()
    {
        ActionSystem.AttachPerformer<BurnGA>(BurnPerformer);
    }

    public void OnDisable()
    {
        ActionSystem.DetachPerformer<BurnGA>();
    }

    private IEnumerator BurnPerformer(BurnGA burnGA)
    {
        CombatView target = burnGA.Target;
        
        // 调整特效位置
        Vector3 vfxPosition = target.transform.position + new Vector3(-1f, 1f, 0f);
        GameObject vfx = Instantiate(burnVFX, vfxPosition, Quaternion.identity);
        
        // 调整特效大小
        vfx.transform.localScale = Vector3.one * 1.5f;
        
        // 销毁特效
        Destroy(vfx, 1f);
        
        target.Damage(burnGA.BurnDamage);
        target.RemoveStatusEffect(StatusEffectType.BURN, 1);
        yield return new WaitForSeconds(0.5f);
    }
}
