using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Vector3 targetPosition;
    [SerializeField] private Transform bulletHitVFX;
    
    public void Setup(Transform target)
    {
        this.targetPosition = target.position;
    }

    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    private void Update()
    {
        Vector3 moveDir = (targetPosition - transform.position).normalized;
        float distanceBeforeMove = Vector3.Distance(transform.position, targetPosition);
        float moveSpeed = 200f;
        transform.position += moveDir * Time.deltaTime * moveSpeed;

        float distanceAfterMove = Vector3.Distance(transform.position, targetPosition);
        if (distanceAfterMove > distanceBeforeMove)
        {
            Destroy(gameObject);
            Instantiate(bulletHitVFX, targetPosition, Quaternion.identity);
        }
    }
}
