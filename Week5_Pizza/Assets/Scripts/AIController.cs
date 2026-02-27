using UnityEngine;
using UnityEngine.AI;

public struct AIStats
{
    public float speed;
    public float angularSpeed;
    public float acceleration;
    public float stoppingDistance;
    public float detectionRange;

    public AIStats(float speed, float angularSpeed, float acceleration, float stoppingDistance, float detectionRange)
    {
        this.speed = speed;
        this.angularSpeed = angularSpeed;
        this.acceleration = acceleration;
        this.stoppingDistance = stoppingDistance;
        this.detectionRange = detectionRange;
    }
}

public abstract class AIController : MonoBehaviour
{
    [SerializeField] protected AIStats stats = new AIStats(10f, 120f, 8f, 3f, 50f);
    [SerializeField] protected float updateRate = 1f;

    protected NavMeshAgent agent;
    protected Transform target;
    protected float timer;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is missing on " + gameObject.name);
            return;
        }

        InitializeAgent();
        FindTarget();
    }

    protected virtual void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            if (agent.enabled)
            {
                agent.isStopped = true;
            }
            return;
        }

        if (target == null || agent == null) return;

        timer += Time.deltaTime;
        if (timer >= updateRate)
        {
            UpdateBehavior();
            timer = 0f;
        }
    }

    protected virtual void InitializeAgent()
    {
        agent.speed = stats.speed;
        agent.angularSpeed = stats.angularSpeed;
        agent.acceleration = stats.acceleration;
        agent.stoppingDistance = stats.stoppingDistance;
    }

    protected abstract void FindTarget();
    protected abstract void UpdateBehavior();

    protected bool IsTargetInRange()
    {
        if (target == null) return false;
        return Vector3.Distance(transform.position, target.position) <= stats.detectionRange;
    }
}