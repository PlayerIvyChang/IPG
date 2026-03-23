using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CombatView : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private StatusEffectsUI statusEffectsUI;
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    private Dictionary<StatusEffectType, int> statusEffects = new();
    
    protected void SetupBase(int health, Sprite image)
    {
        MaxHealth = CurrentHealth = health;
        spriteRenderer.sprite = image;
        UpdateHealthText();
    }
    
    protected void SetupBase(int maxHealth, int currentHealth, Sprite image)
    {
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        spriteRenderer.sprite = image;
        UpdateHealthText();
    }

    protected void UpdateHealthText()
    {
        healthText.text = "HP= " + CurrentHealth; 
    }

    public void Damage(int damageAmount)
    {
        int remainingDamage = damageAmount;
        int currentArmor = GetStatusEffectStackCount(StatusEffectType.ARMOR);

        if (currentArmor > 0)
        {
            if (currentArmor >= damageAmount)
            {
                RemoveStatusEffect(StatusEffectType.ARMOR, remainingDamage);
                remainingDamage = 0;
            }
            else if (currentArmor < damageAmount)
            {
                RemoveStatusEffect(StatusEffectType.ARMOR, currentArmor);
                remainingDamage -= currentArmor; 
            }
        }

        if (remainingDamage > 0)
        {
            CurrentHealth -= remainingDamage;
            if (CurrentHealth < 0)
            {
                CurrentHealth = 0;
            }
        }             

        UpdateHealthText();
    }

    public void AddStatusEffect(StatusEffectType type, int stackCount)
    {
        if ( statusEffects.ContainsKey(type))
        {
            statusEffects[type] += stackCount;
        }
        else
        {
            statusEffects.Add(type, stackCount);
        }
        statusEffectsUI.UpdateStatusEffectUI(type, GetStatusEffectStackCount(type));
    }

    public void RemoveStatusEffect(StatusEffectType type, int stackCount)
    {
        if (statusEffects.ContainsKey(type))
        {
            statusEffects[type] -= stackCount;
            if (statusEffects[type] <= 0)
            {
                statusEffects.Remove(type);
            }
            
        }
        statusEffectsUI.UpdateStatusEffectUI(type, GetStatusEffectStackCount(type));
    }

    public int GetStatusEffectStackCount(StatusEffectType type)
    {
        if (statusEffects.ContainsKey(type))
        {
            return statusEffects[type];
        } else
        {
            return 0;
        }
    }
}
