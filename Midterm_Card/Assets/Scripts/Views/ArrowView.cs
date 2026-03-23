using UnityEngine;

public class ArrowView : MonoBehaviour
{
    [SerializeField] private GameObject arrowHead;
    [SerializeField] private LineRenderer line;
    private Vector3 startPoint;

    private void Update()
    {
        Vector3 endPosition = MouseUtils.GetMouseWorldPosition();
        Vector3 direction = -(startPoint - arrowHead.transform.position).normalized;
        line.SetPosition(1, endPosition - direction * 0.5f);
        arrowHead.transform.position = endPosition;
        arrowHead.transform.right = direction;
    }

    public void SetupArrow(Vector3 startPoint)
    {
        this.startPoint = startPoint;
        line.SetPosition(0, startPoint);
        line.SetPosition(1, MouseUtils.GetMouseWorldPosition());
    }
}
