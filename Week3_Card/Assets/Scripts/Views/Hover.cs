using UnityEngine;
using UnityEngine.Rendering;

public class Hover : Singleton<Hover>
{
    [SerializeField] private CardView cardViewHover;
    [SerializeField] private float hoverScaleFactor = 1.2f;

    private void Awake()
    {
        if (cardViewHover != null && !cardViewHover.gameObject.scene.IsValid())
        {
            CardView instance = Instantiate(cardViewHover, Vector3.zero, Quaternion.identity);
            instance.name = cardViewHover.name + "_Instance";
            cardViewHover = instance;
        }

        if (cardViewHover != null)
        {
            var sortingGroups = cardViewHover.GetComponentsInChildren<SortingGroup>(true);
            foreach (var sg in sortingGroups)
            {
                sg.sortingOrder = 0;
            }

            var colliders = cardViewHover.GetComponentsInChildren<Collider>(true);
            foreach (var c in colliders) c.enabled = false;
            var colliders2D = cardViewHover.GetComponentsInChildren<Collider2D>(true);
            foreach (var c in colliders2D) c.enabled = false;

            SetLayerRecursively(cardViewHover.gameObject, 2);
        }
    }

    private void Start()
    {

        if (cardViewHover.DefaultScale == Vector3.zero)
            cardViewHover.DefaultScale = cardViewHover.transform.localScale != Vector3.zero ? cardViewHover.transform.localScale : Vector3.one;

        cardViewHover.gameObject.SetActive(false);

    }

    public void Show(Card card, Vector3 position)
    {

        cardViewHover.Setup(card);

        Vector3 baseScale = cardViewHover.DefaultScale != Vector3.zero ? cardViewHover.DefaultScale : Vector3.one;
        cardViewHover.transform.localScale = baseScale * hoverScaleFactor;

        Camera cam = Camera.main ?? Camera.current;
        if (cam != null)
        {
            Vector3 dir = cam.transform.position - position;
            if (dir.sqrMagnitude > Mathf.Epsilon)
            {
                cardViewHover.transform.rotation = Quaternion.LookRotation((-dir).normalized, cam.transform.up);
            }
        }

        cardViewHover.transform.position = position;
        cardViewHover.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (cardViewHover != null && cardViewHover.gameObject.activeSelf)
        {
            cardViewHover.gameObject.SetActive(false);
        }
    }

    private void SetLayerRecursively(GameObject go, int layer)
    {
        if (go == null) return;
        go.layer = layer;
        foreach (Transform child in go.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
