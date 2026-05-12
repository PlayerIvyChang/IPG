using TMPro;
using UnityEngine;

public class PathFindingDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro gText;
    [SerializeField] private TextMeshPro hText;
    [SerializeField] private TextMeshPro fText;
    private PathNode pathNode;

    public void SetPathNode(PathNode pathNode)
    {
        this.pathNode = pathNode;
    }

    private void Update()
    {
        if (pathNode != null)
        {
            gText.text = pathNode.GetGCost().ToString();
            hText.text = pathNode.GetHCost().ToString();
            fText.text = pathNode.GetFCost().ToString();
        }
    }
}
