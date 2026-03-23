using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSystem : Singleton<CardSystem>
{
    [SerializeField] private HandView handView;
    [SerializeField] private Transform drawPilePoint;
    [SerializeField] private Transform discardPilePoint;

    private readonly List<Card> drawPile = new();
    private readonly List<Card> discardPile = new();
    private readonly List<Card> hand = new();

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<DrawCardGA>(DrawCardsPerformer);
        ActionSystem.AttachPerformer<DiscardAllGA>(DiscardAllPerformer);
        ActionSystem.AttachPerformer<PlayCardGA>(PlayCardPerformer);
    }
    
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardGA>();
        ActionSystem.DetachPerformer<DiscardAllGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
    }
    
    public void Setup(List<CardData> deckData)
    {
        drawPile.Clear();
        discardPile.Clear();
        hand.Clear();
        
        foreach (var cardData in deckData)
        {
            Card card = new(cardData);
            drawPile.Add(card);
        }
        
        ShuffleDeck();
    }
    
    private IEnumerator DrawCardsPerformer(DrawCardGA drawCardsGA)
    {
        for (int i = 0; i < drawCardsGA.Amount; i++)
        {
            if (drawPile.Count == 0)
            {
                RefillDeck();
                
                if (drawPile.Count == 0)
                {
                    yield break;
                }
            }
            
            yield return DrawCard();
        }
    }

    private IEnumerator DiscardAllPerformer(DiscardAllGA discardAllGA)
    {
        List<Card> cardsToDiscard = new(hand);
        
        foreach (Card card in cardsToDiscard)
        {
            discardPile.Add(card);
            
            CardView cardView = handView.RemoveCard(card);
            if (cardView != null)
            {
                float duration = 0.25f;
                yield return cardView.AnimateTo(discardPilePoint.position, discardPilePoint.rotation, Vector3.zero, duration);
                Destroy(cardView.gameObject);
            }
        }
        
        hand.Clear();
    }
    
    private IEnumerator DrawCard()
    {
        if (drawPile.Count == 0)
        {
            yield break;
        }
        
        Card card = drawPile.Draw();
        
        if (card == null)
        {
            yield break;
        }
        
        hand.Add(card);
        CardView cardView = CardViewCreator.Instance.CreateCardView(card, drawPilePoint.position, drawPilePoint.rotation);
        
        if (cardView != null)
        {
            yield return handView.AddCard(cardView);
        }
    }
    
    private void RefillDeck()
    {
        if (discardPile.Count == 0)
        {
            return;
        }
        
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        ShuffleDeck();
    }
    
    private void ShuffleDeck()
    {
        for (int i = drawPile.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Card temp = drawPile[i];
            drawPile[i] = drawPile[randomIndex];
            drawPile[randomIndex] = temp;
        }
    }
    
    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
    {
        hand.Remove(playCardGA.Card);
        CardView cardView = handView.RemoveCard(playCardGA.Card);
        
        discardPile.Add(playCardGA.Card);
        
        if (cardView != null)
        {
            float duration = 0.25f;
            yield return cardView.AnimateTo(discardPilePoint.position, discardPilePoint.rotation, Vector3.zero, duration);
            Destroy(cardView.gameObject);
        }

        SpendManaGA spendManaGA = new(playCardGA.Card.Cost);
        ActionSystem.Instance.AddReaction(spendManaGA);

        if (playCardGA.Card.ManualTargetEffects != null)
        {
            EffectGA effectGA = new(playCardGA.Card.ManualTargetEffects, new () { playCardGA.ManualTarget});
            ActionSystem.Instance.AddReaction(effectGA);
        }

        foreach (var effectWrapper in playCardGA.Card.OtherEffects)
        {
            List<CombatView> targets = effectWrapper.TargetMode.GetTargets();
            EffectGA effectGA = new(effectWrapper.Effect, targets);
            ActionSystem.Instance.AddReaction(effectGA); 
        }
    }
}

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
