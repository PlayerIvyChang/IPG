using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text stackText;

    private void Awake()
    {
        Graphic[] graphics = GetComponentsInChildren<Graphic>(true);
        foreach (var graphic in graphics)
        {
            graphic.raycastTarget = false;
        }
    }

    public void Set(Sprite sprite, int stackCount)
    {
        if (icon != null)
        {
            icon.sprite = sprite;
        }
        
        if (stackText != null)
        {
            stackText.text = stackCount.ToString();
        }
    }
}
