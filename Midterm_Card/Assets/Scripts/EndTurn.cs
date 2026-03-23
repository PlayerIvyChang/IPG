using UnityEngine;
using TMPro;

public class EndTurn : MonoBehaviour
{
    private void Start()
    {
        // 自动查找按钮下的 TextMeshPro 组件
        TMP_Text buttonText = GetComponentInChildren<TMP_Text>();
        if (buttonText != null)
        {
            buttonText.text = "End Turn";
        }
    }

    public void OnClick()
    {
        EnemyTurnGA enemyTurnGA = new();
        ActionSystem.Instance.Perform(enemyTurnGA);
    }
}
