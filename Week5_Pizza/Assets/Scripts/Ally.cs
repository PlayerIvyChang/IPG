using UnityEngine;

public class Ally : AIController
{
    [SerializeField] private Vector3 followOffset = new Vector3(0, 0, -5f);

    protected override void Start()
    {
        stats = new AIStats(8f, 100f, 6f, 2f, 100f);
        base.Start();
    }

    protected override void FindTarget()
    {
        if (GameManager.Instance != null)
        {
            target = GameManager.Instance.PlayerTransform;
        }
    }

    protected override void UpdateBehavior()
    {
        if (target == null) return;

        Vector3 followPosition = target.position + target.TransformDirection(followOffset);
        agent.SetDestination(followPosition);
    }
}