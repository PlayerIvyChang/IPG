using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private Units unit;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private HealthSystem healthSystem;

    private void Start()
    {
        if (healthSystem == null && unit != null)
        {
            healthSystem = unit.GetComponent<HealthSystem>();
        }

        UpdateHealthBar();
        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
        healthSystem.OnDie += HealthSystem_OnDie;
    }

    private void OnDestroy()
    {
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
            healthSystem.OnDie -= HealthSystem_OnDie;
        }
    }

    private void HealthSystem_OnHealthChanged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        if (healthBarImage != null && healthSystem != null)
        {
            float healthNormalized = healthSystem.GetHealthNormalized();
            healthBarImage.transform.localScale = new Vector3(healthNormalized, 1f, 1f);
        }
    }

    private void HealthSystem_OnDie(object sender, EventArgs e)
    {
        // 单位死亡时的处理
    }
}
