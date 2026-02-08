using System.Collections.Generic;
using UnityEngine;

public class Card : Singleton<Card>
{
    public string Title => data.name;
    public string Description => data.Description;
    public Sprite Image => data.Image;
    public List<Effects> Effects => data.Effects;
    public int Cost { get; private set; }

    private readonly CardData data;
    public Card(CardData cardData)
    {
        data = cardData;
        Cost = cardData.cost;
    }

    private void OnMouseDown()
    {
        if (ActionSystem.Instance.IsPerforming) return;
        DrawCardGA drawCardGA = new(5);
        ActionSystem.Instance.Perform(drawCardGA);
        Destroy(gameObject);
    }
}
