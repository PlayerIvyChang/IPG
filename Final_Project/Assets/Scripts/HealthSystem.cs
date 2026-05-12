using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int healthMax = 100;
    private int health;
    
    public event EventHandler OnDie;
    public event EventHandler OnHealthChanged;

    private void Awake()
    {
        health = healthMax;
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        healthMax = newMaxHealth;
        health = healthMax;
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Damage(int damageAmount)
    {
        health -= damageAmount;
        
        if (health < 0)
        {
            health = 0;
        }

        OnHealthChanged?.Invoke(this, EventArgs.Empty);

        if (health == 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        health += healAmount;
        
        if (health > healthMax)
        {
            health = healthMax;
        }

        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Die()
    {
        OnDie?.Invoke(this, EventArgs.Empty);
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetHealthMax()
    {
        return healthMax;
    }

    public float GetHealthNormalized()
    {
        return (float)health / healthMax;
    }
}
