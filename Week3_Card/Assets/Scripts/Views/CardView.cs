using UnityEngine;
using TMPro;
using System.Collections;

public class CardView : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text cost;

    [SerializeField] private SpriteRenderer imageSR;

    [SerializeField] private GameObject wrapper;
    [SerializeField] private LayerMask dropLayer;
    public Card Card { get; private set; }

    public Vector3 dragStartPosition;
    public Quaternion dragStartRotation;

    public Vector3 DefaultScale { get; set; } = Vector3.one;

    public void Setup(Card card)
    {
        Card = card;
        title.text = card.Title;
        description.text = card.Description;
        cost.text = card.Cost.ToString();
        imageSR.sprite = card.Image;
    }

    void OnMouseEnter()
    {
        if (!Interaction.Instance.CanHover()) return;
        wrapper.SetActive(false);
        Vector3 pos = new(transform.position.x, -2, 0);
        Hover.Instance.Show(Card, pos);
    }

    void OnMouseExit()
    {
        if (!Interaction.Instance.CanHover()) return;
        Hover.Instance.Hide();
        wrapper.SetActive(true);
    }

    void OnMouseDown()
    {
        if(!Interaction.Instance.CanInteract()) return;
        Interaction.Instance.IsDragging = true;
        wrapper.SetActive(true);
        Hover.Instance.Hide();
        dragStartPosition = transform.position;
        dragStartRotation = transform.rotation;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = MouseUtils.GetMouseWorldPosition(-1);
    }
    void OnMouseUp()
    {
        if (!Interaction.Instance.CanInteract()) return;
        if(Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hit, 10f, dropLayer)
            && ManaSystem.Instance.HasEnoughMana(Card.Cost))
        {
            PlayCardGA playCardGA = new(Card);
            ActionSystem.Instance.Perform(playCardGA);
        }
        else
        {
            transform.position = dragStartPosition;
            transform.rotation = dragStartRotation;
        }
        Interaction.Instance.IsDragging = false;
    }
    void OnMouseDrag()
    {
        if (!Interaction.Instance.CanInteract()) return;
        transform.position = MouseUtils.GetMouseWorldPosition(-1);
    }

    // Animate position, rotation and scale over duration (world-space)
    public IEnumerator AnimateTo(Vector3 targetPosition, Quaternion targetRotation, Vector3 targetScale, float duration)
    {

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Vector3 startScale = transform.localScale;

        string cardName = Card != null ? Card.Title : gameObject.name;

        if (duration <= 0f)
        {
            transform.position = targetPosition;
            transform.rotation = targetRotation;
            transform.localScale = targetScale;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float ease = t * t * (3f - 2f * t);
            transform.position = Vector3.LerpUnclamped(startPos, targetPosition, ease);
            transform.rotation = Quaternion.SlerpUnclamped(startRot, targetRotation, ease);
            transform.localScale = Vector3.LerpUnclamped(startScale, targetScale, ease);
            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;
        transform.localScale = targetScale;

    }
}
