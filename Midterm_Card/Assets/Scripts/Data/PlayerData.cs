using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Data/Player")] 
public class PlayerData : ScriptableObject
{
    [field: SerializeField] public Sprite image { get; private set; }
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public List<CardData> Deck { get; private set; }
    
}
