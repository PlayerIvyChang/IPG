using System;
using UnityEngine;

public class HealthCollectable : MonoBehaviour
{
    public static event EventHandler OnAnyHealthCollected;

    [SerializeField] private int healAmount = 50;
    [SerializeField] private float rotationSpeed = 90f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.3f;

    private GridPosition gridPosition;
    private Vector3 startPosition;
    private bool isCollected = false;

    private void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        startPosition = transform.position;
    }

    private void Update()
    {
        if (isCollected)
        {
            return;
        }

        // 旋转
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // 上下浮动
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // 检查是否有单位在同一格子
        CheckForCollection();
    }

    private void CheckForCollection()
    {
        Units unit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        
        if (unit != null && !unit.IsEnemyUnit())
        {
            // 只治疗玩家单位
            Collect(unit);
        }
    }

    private void Collect(Units unit)
    {
        if (isCollected)
        {
            return;
        }

        isCollected = true;

        // 治疗单位
        HealthSystem healthSystem = unit.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            int currentHealth = healthSystem.GetHealth();
            int maxHealth = healthSystem.GetHealthMax();
            int actualHealAmount = Mathf.Min(healAmount, maxHealth - currentHealth);

            if (actualHealAmount > 0)
            {
                healthSystem.Heal(healAmount);
            }
        }

        // 触发事件
        OnAnyHealthCollected?.Invoke(this, EventArgs.Empty);

        // 启动收集动画
        StartCoroutine(CollectionAnimation());
    }

    private System.Collections.IEnumerator CollectionAnimation()
    {
        // 简单的缩小消失动画
        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // 缩小
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            
            // 向上移动
            transform.position += Vector3.up * Time.deltaTime * 2f;

            yield return null;
        }

        Destroy(gameObject);
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    private void OnDrawGizmos()
    {
        // 在 Scene 视图中显示收集范围
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Vector3 worldPos = LevelGrid.Instance.GetWorldPosition(gridPosition);
            Gizmos.DrawWireCube(worldPos, Vector3.one * 2f);
        }
    }
}