using UnityEngine;
using UnityEngine.Rendering;

public class CardViewCreator : Singleton<CardViewCreator>
{
    [SerializeField] private CardView cardViewPrefab;

    public CardView CreateCardView(Card card, Vector3 position, Quaternion rotation)
    {
        CardView cardview = Instantiate(cardViewPrefab, position, rotation);
        cardview.transform.localScale = Vector3.zero;

        cardview.gameObject.SetActive(true);
        cardview.Setup(card);
        return cardview;
    }
}
