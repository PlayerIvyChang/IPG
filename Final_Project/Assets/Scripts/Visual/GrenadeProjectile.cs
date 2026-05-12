using System;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float damageRadius = 3f;
    [SerializeField] private int damageAmount = 30;
    [SerializeField] private Transform grenadeVFX;

    public static event EventHandler OnAnyGrenadeExploded;
    private Vector3 targetPosition;
    private Vector3 positionXZ;
    private float totalDistance;
    private Action onGrenadeBehaviourComplete;
    
    private void Update()
    {
        Vector3 moveDir = (targetPosition - positionXZ).normalized;
        positionXZ += moveDir * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        
        // 防止除以0
        if (totalDistance > 0)
        {
            float distanceNormalized = 1 - distance / totalDistance;

            // 抛物线轨迹
            float maxHeight = totalDistance / 4f;
            float positionY = Mathf.Sin(distanceNormalized * Mathf.PI) * maxHeight;
            transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);
        }

        float reachedDistance = 0.2f;
        if (Vector3.Distance(positionXZ, targetPosition) < reachedDistance)
        {
            // 使用目标位置的 Y 值进行检测,确保检测到地面上的物体
            Vector3 explosionCenter = new Vector3(targetPosition.x, 1f, targetPosition.z);
            
            // 范围伤害
            Collider[] colliderArray = Physics.OverlapSphere(explosionCenter, damageRadius);
            
            foreach (Collider collider in colliderArray)
            {
                // 检测单位
                if (collider.TryGetComponent<Units>(out Units targetUnit))
                {
                    targetUnit.Damage(damageAmount);
                }

                // 检测可破坏箱子
                if (collider.TryGetComponent<DestructableCrate>(out DestructableCrate crate))
                {
                    crate.Damage();
                }
            }

            // 触发事件
            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);
            
            // 通知完成
            onGrenadeBehaviourComplete?.Invoke();
            
            // 生成爆炸特效
            if (grenadeVFX != null)
            {
                Instantiate(grenadeVFX, explosionCenter, Quaternion.identity);
            }
            
            Destroy(gameObject);
        }
    }

    public void Setup(GridPosition targetGridPosition, Action onGrenadeBehaviourComplete)
    {
        this.onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
