using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class HandView : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    private readonly List<CardView> cards = new();

    public IEnumerator AddCard(CardView cardView)
    {
        cards.Add(cardView);
        yield return UpdateCardPositions(0.15f);

    }

    public CardView RemoveCard(Card card)
    {
        CardView cardView = GetCardView(card);
        if (cardView == null) return null;
        cards.Remove(cardView);
        StartCoroutine(UpdateCardPositions(0.15f));
        return cardView;
    }
    private CardView GetCardView(Card card)
    {
        return cards.Where(cardView => cardView.Card == card).FirstOrDefault();
    }


    public IEnumerator UpdateCardPositions(float duration)
    {
        if (cards.Count == 0)
            yield break;

        if (splineContainer == null)
        {
            yield break;
        }

        float cardSpacing = 1f / 10f;
        float firstCardPosition = 0.5f - (cards.Count - 1) * cardSpacing / 2f;

        Spline spline = splineContainer.Spline;
        Camera cam = Camera.main ?? Camera.current;

        for (int i = 0; i < cards.Count; i++) {
            float p = firstCardPosition + i * cardSpacing;

            Vector3 localSplinePos = spline.EvaluatePosition(p);
            Vector3 localForward = spline.EvaluateTangent(p);
            Vector3 localUp = spline.EvaluateUpVector(p);

            Vector3 worldPos = splineContainer.transform.TransformPoint(localSplinePos);
            if (cam != null)
                worldPos += cam.transform.forward * (-0.01f * i);

            Quaternion worldRot;

            if (cam != null)
            {
                Vector3 toCam = (cam.transform.position - worldPos);
                if (toCam.sqrMagnitude <= Mathf.Epsilon)
                {
                    Quaternion localRot = Quaternion.LookRotation(-localUp, Vector3.Cross(-localUp, localForward).normalized);
                    worldRot = splineContainer.transform.rotation * localRot;
                }
                else
                {
                    worldRot = Quaternion.LookRotation((-toCam).normalized, cam.transform.up);
                }
            }
            else
            {
                Quaternion localRot = Quaternion.LookRotation(-localUp, Vector3.Cross(-localUp, localForward).normalized);
                worldRot = splineContainer.transform.rotation * localRot;
            }

            CardView cardView = cards[i];

            Vector3 targetScale = cardView.transform.localScale == Vector3.zero ? cardView.DefaultScale : cardView.transform.localScale;

            cardView.transform.position = worldPos;
            cardView.transform.rotation = worldRot;

            StartCoroutine(cardView.AnimateTo(worldPos, worldRot, targetScale, duration));
        }

        yield return new WaitForSeconds(duration);
    }
}
