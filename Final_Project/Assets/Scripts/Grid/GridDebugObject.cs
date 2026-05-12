using UnityEngine;
using TMPro;
public class GridDebugObject : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    private object gridObject;
    public virtual void SetGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
    }
    protected virtual void Update()
    {
        textMesh.text = gridObject.ToString();
    }
}
