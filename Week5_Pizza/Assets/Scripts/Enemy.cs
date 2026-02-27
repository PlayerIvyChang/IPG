using UnityEngine;

public class Enemy : AIController
{
    [SerializeField] private EnemyType enemyType = EnemyType.Chaser;

    public enum EnemyType
    {
        Chaser,
        Ambusher,
        Patroller
    }

    protected override void Start()
    {
        base.Start();
        ConfigureByType();
    }

    protected override void FindTarget()
    {
        if (GameManager.Instance != null)
        {
            target = GameManager.Instance.PlayerTransform;
        }

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    protected override void UpdateBehavior()
    {
        if (!IsTargetInRange()) return;

        switch (enemyType)
        {
            case EnemyType.Chaser:
                ChaseTarget();
                break;
            case EnemyType.Ambusher:
                AmbushTarget();
                break;
            case EnemyType.Patroller:
                PatrolAndChase();
                break;
        }
    }

    private void ConfigureByType()
    {
        switch (enemyType)
        {
            case EnemyType.Chaser:
                stats.speed = 8f;
                stats.stoppingDistance = 3f;
                updateRate = 0.5f;
                break;
            case EnemyType.Ambusher:
                stats.speed = 10f;
                stats.stoppingDistance = 5f;
                updateRate = 1f;
                break;
            case EnemyType.Patroller:
                stats.speed = 5f;
                stats.stoppingDistance = 2f;
                updateRate = 1.5f;
                break;
        }
        InitializeAgent();
    }

    private void ChaseTarget()
    {
        agent.SetDestination(target.position);
    }

    private void AmbushTarget()
    {
        Vector3 predictedPosition = target.position + target.forward * 10f;
        agent.SetDestination(predictedPosition);
    }

    private void PatrolAndChase()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < stats.detectionRange * 0.5f)
        {
            agent.SetDestination(target.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }
}
