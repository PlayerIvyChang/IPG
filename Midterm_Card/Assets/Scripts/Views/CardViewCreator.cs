using UnityEngine;

public class CardViewCreator : MonoBehaviour
{
    public static CardViewCreator Instance { get; private set; }
    
    [SerializeField] private CardView cardViewPrefab;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    public CardView CreateCardView(Card card, Vector3 position, Quaternion rotation)
    {
        if (cardViewPrefab == null)
        {
            Debug.LogError("CardView prefab is null!");
            return null;
        }
        
        if (card == null)
        {
            Debug.LogError("Card is null in CreateCardView!");
            return null;
        }
        
        Debug.Log($"Creating CardView for: {card.Title ?? "NULL TITLE"}");
        
        CardView cardView = Instantiate(cardViewPrefab, position, rotation);
        
        if (cardView == null)
        {
            Debug.LogError("Failed to instantiate CardView!");
            return null;
        }
        
        cardView.Setup(card);
        return cardView;
    }

    public GameObject CreateCardViewForReward(CardData cardData, Vector3 position, Quaternion rotation)
    {
        Card tempCard = new Card(cardData);
        CardView cardView = Instantiate(cardViewPrefab, position, rotation);
        cardView.Setup(tempCard);
        return cardView.gameObject;
    }
}
