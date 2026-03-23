using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string Title => data.name;
    public string Description => data.Description;
    public Sprite Image => data.Image;
    public Effects ManualTargetEffects => data.ManualTargetEffect;
    public List<AutoTargetEffect> OtherEffects => data.OtherEffects;
    public int Cost { get; private set; }

    private readonly CardData data;
    
    public Card(CardData cardData)
    {
        data = cardData;
        Cost = cardData.cost;
    }
}
