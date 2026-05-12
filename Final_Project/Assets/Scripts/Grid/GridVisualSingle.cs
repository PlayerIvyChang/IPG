using UnityEngine;

public class GridVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    public void Show(Color color)
    {
        meshRenderer.enabled = true;
    }
    public void Hide()
    {
        meshRenderer.enabled = false;
    }
}
