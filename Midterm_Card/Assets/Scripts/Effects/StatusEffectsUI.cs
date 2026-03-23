using System.Collections.Generic;
using UnityEngine;

public class StatusEffectsUI : MonoBehaviour
{
    [SerializeField] private StatusEffectUI[] statusEffectUIPrefab;
    [SerializeField] private Sprite armorSprite, burnSprite;

    private Dictionary<StatusEffectType, StatusEffectUI> statusEffectUIs = new();

    public void UpdateStatusEffectUI(StatusEffectType statusEffectType, int stackCount)
    {
        if (stackCount == 0)
        {
            if (statusEffectUIs.ContainsKey(statusEffectType))
            {
                StatusEffectUI statusEffectUI = statusEffectUIs[statusEffectType];
                statusEffectUIs.Remove(statusEffectType);
                Destroy(statusEffectUI.gameObject);
            }
        }
        else
        {
            if (!statusEffectUIs.ContainsKey(statusEffectType))
            {
                StatusEffectUI statusEffectUI = Instantiate(GetStatusEffectUIPrefab(statusEffectType), transform);
                statusEffectUIs.Add(statusEffectType, statusEffectUI);
            }
            Sprite sprite = GetSpriteByType(statusEffectType);
            statusEffectUIs[statusEffectType].Set(sprite, stackCount);
        }
    }

    private StatusEffectUI GetStatusEffectUIPrefab(StatusEffectType statusEffectType)
    {
        int index = (int)statusEffectType;
        if (index >= 0 && index < statusEffectUIPrefab.Length)
        {
            return statusEffectUIPrefab[index];
        }
        return statusEffectUIPrefab[0]; // 默认返回第一个
    }

    private Sprite GetSpriteByType(StatusEffectType statusEffectType)
    {
        return statusEffectType switch
        {
            StatusEffectType.ARMOR => armorSprite,
            StatusEffectType.BURN => burnSprite,
            _ => null
        };
    }
}