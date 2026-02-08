using NUnit.Framework;
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
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyPreReaction, ReactionTiming.PRE);
        ActionSystem.SubscribeReaction<EnemyTurnGA>(EnemyPostReaction, ReactionTiming.POST);
    }
    private void OnDisable()
    {
        ActionSystem.DetachPerformer<DrawCardGA>();
        ActionSystem.DetachPerformer<DiscardAllGA>();
        ActionSystem.DetachPerformer<PlayCardGA>();
    }
    public void Setup(List<CardData> deckData)
    {
        foreach (var cardData in deckData)
        {
            Card card = new(cardData);
            drawPile.Add(card);
        }
    }
    private IEnumerator DrawCardsPerformer(DrawCardGA drawCardsGA)
    {
        // 在开始抽牌前检查抽牌堆是否足够
        // 如果不足，将整个弃牌堆转移到抽牌堆
        if (drawPile.Count < drawCardsGA.Amount)
        {
            Debug.Log($"Draw pile has {drawPile.Count} cards, need {drawCardsGA.Amount}. Refilling entire discard pile ({discardPile.Count} cards) into draw pile.");
            RefillDeck();
        }

        // 抽取所需数量的牌
        for (int i = 0; i < drawCardsGA.Amount; i++)
        {
            Debug.Log("Drawing card " + (i + 1) + " of " + drawCardsGA.Amount);
            yield return DrawCard();
        }
    }

    private IEnumerator DiscardAllPerformer(DiscardAllGA discardAllGA)
    {
        foreach (Card card in hand)
        {
            discardPile.Add(card);
            CardView cardView = handView.RemoveCard(card);
            yield return DiscardCard(cardView);
        }
        hand.Clear();
    }
    private void EnemyPreReaction(EnemyTurnGA enemyTurnGA)
    {
        DiscardAllGA discardAllGA = new();
        ActionSystem.Instance.AddReaction(discardAllGA);
    }
    private void EnemyPostReaction(EnemyTurnGA enemyTurnGA)
    {
        DrawCardGA drawCardGA = new(5);
        ActionSystem.Instance.AddReaction(drawCardGA);
    }
    private IEnumerator DrawCard()
    {
        Debug.Log("Drawing a card from draw pile.");
        Card card = drawPile.Draw();
        hand.Add(card);
        CardView cardView = CardViewCreator.Instance.CreateCardView(card, drawPilePoint.position, drawPilePoint.rotation);
        yield return handView.AddCard(cardView);
    }
    private void RefillDeck()
    {
        Debug.Log($"Moving entire discard pile ({discardPile.Count} cards) into draw pile.");
        // 将整个弃牌堆的所有牌转移到抽牌堆
        drawPile.AddRange(discardPile);
        // 清空弃牌堆
        discardPile.Clear();
    }
    private IEnumerator DiscardCard(CardView cardView)
    {
        if (cardView == null)
            yield break;

        float duration = 0.25f;
        yield return cardView.AnimateTo(discardPilePoint.position, discardPilePoint.rotation, Vector3.zero, duration);
        Destroy(cardView.gameObject);
        yield break;
    }
    private IEnumerator PlayCardPerformer(PlayCardGA playCardGA)
    {
        hand.Remove(playCardGA.Card);
        CardView cardView = handView.RemoveCard(playCardGA.Card);
        yield return DiscardCard(cardView);

        SpendManaGA spendManaGA = new(playCardGA.Card.Cost);
        ActionSystem.Instance.AddReaction(spendManaGA);

        foreach (var effect in playCardGA.Card.Effects)
        {
            GameAction gameAction = effect.GetGameAction();
            ActionSystem.Instance.AddReaction(gameAction);
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
