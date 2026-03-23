using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardRewardPanel : MonoBehaviour
{
    public static CardRewardPanel Instance { get; private set; }

    [Header("UI Elements")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    
    [Header("World Space Card Positions")]
    [SerializeField] private Transform cardPosition1;
    [SerializeField] private Transform cardPosition2;
    [SerializeField] private Transform cardPosition3;

    private List<CardData> rewardCards;
    private List<GameObject> spawnedCards = new();
    private System.Action onCardSelected;

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

        panel.SetActive(false);
    }

    public void ShowRewards(List<CardData> cards, System.Action onComplete)
    {
        rewardCards = cards;
        onCardSelected = onComplete;

        ClearCards();

        // 删除玩家视图
        if (PlayerSystem.Instance != null && PlayerSystem.Instance.PlayerView != null)
        {
            Destroy(PlayerSystem.Instance.PlayerView.gameObject);
        }

        // 删除所有手牌（先禁用再销毁）
        DestroyAllCards();

        panel.SetActive(true);
        titleText.text = "Choose a Card to Add to Your Deck";

        Transform[] positions = { cardPosition1, cardPosition2, cardPosition3 };
        
        for (int i = 0; i < 3 && i < cards.Count; i++)
        {
            int index = i;
            CardData cardData = cards[i];
            
            Card tempCard = new Card(cardData);
            
            CardView cardView = CardViewCreator.Instance.CreateCardView(
                tempCard, 
                positions[i].position, 
                positions[i].rotation
            );

            RewardCardSelector selector = cardView.gameObject.AddComponent<RewardCardSelector>();
            selector.Setup(index, this);
            
            spawnedCards.Add(cardView.gameObject);
        }
    }

    public void OnCardClicked(int index)
    {
        GameProgress.Instance.AddCardToDeck(rewardCards[index]);

        // 清理所有卡牌
        ClearCards();
        DestroyAllCards();
        
        panel.SetActive(false);

        onCardSelected?.Invoke();
    }

    private void DestroyAllCards()
    {
        // 查找所有 CardView 并立即禁用再销毁
        CardView[] allCardViews = FindObjectsByType<CardView>(FindObjectsSortMode.None);
        foreach (var cardView in allCardViews)
        {
            if (cardView != null)
            {
                // 立即禁用 GameObject，停止所有鼠标事件
                cardView.gameObject.SetActive(false);
                // 然后销毁
                Destroy(cardView.gameObject);
            }
        }
    }

    private void ClearCards()
    {
        foreach (var cardObj in spawnedCards)
        {
            if (cardObj != null)
            {
                // 立即禁用，停止鼠标事件
                cardObj.SetActive(false);
                // 然后销毁
                Destroy(cardObj);
            }
        }
        spawnedCards.Clear();
    }
}