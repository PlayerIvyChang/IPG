using UnityEngine;
using TMPro;
using System;

public class EnemyView : CombatView
{
    [SerializeField] private TMP_Text attackText;
    public int AttackPower { get; private set; }
    
    public void Setup(EnemyData enemyData)
    {
        AttackPower = enemyData.AttackPower;
        UpdateAttackText();
        SetupBase(enemyData.Health, enemyData.image);
    }

    public void UpdateAttackText()
    {
        attackText.text = "ATK= " + AttackPower;
    }
}
