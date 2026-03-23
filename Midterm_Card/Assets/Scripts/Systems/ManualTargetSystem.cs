using UnityEngine;

public class ManualTargetSystem : Singleton<ManualTargetSystem>
{
    [SerializeField] private ArrowView arrowView;
    [SerializeField] private LayerMask targetLayerMask;
    public void StartTargeting(Vector3 startPoint)
    {
        arrowView.gameObject.SetActive(true);
        arrowView.SetupArrow(startPoint);
    }

    public EnemyView EndTargeting(Vector3 endPoint)
    {
        arrowView.gameObject.SetActive(false);
        if (Physics.Raycast(endPoint, Vector3.forward, out RaycastHit hit, 10f, targetLayerMask)
            && hit.collider != null
            && hit.transform.TryGetComponent(out EnemyView enemyView))
        {
            return enemyView;
        }
        return null;
    }
}
