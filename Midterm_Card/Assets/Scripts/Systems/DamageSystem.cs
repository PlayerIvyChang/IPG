using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DamageSystem : MonoBehaviour
{
    [SerializeField] private GameObject damageVFX;
    [SerializeField] private Camera mainCamera;
    
    private bool gameEnded = false;
    
    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }
    
    void OnEnable()
    {
        ActionSystem.AttachPerformer<DealDamageGA>(DealDamagePerformer);
    }
    
    void OnDisable()
    {
        ActionSystem.DetachPerformer<DealDamageGA>();
    }
    
    private IEnumerator DealDamagePerformer(DealDamageGA dealDamage)
    {
        foreach(var target in dealDamage.Targets)
        {
            // 检查目标是否仍然有效
            if (target == null)
            {
                continue;
            }
            
            target.Damage(dealDamage.Amount);
            
            StartCoroutine(ShakeCamera(0.2f, 0.3f));
            
            // 检查对象是否仍然存在再访问 transform
            if (target != null)
            {
                Vector3 vfxPosition = target.transform.position + new Vector3(-0.5f, 1f, 0f);
                GameObject vfx = Instantiate(damageVFX, vfxPosition, Quaternion.identity);
                Destroy(vfx, 1f);
            }
            
            yield return new WaitForSeconds(0.15f);
            
            // 检查敌人是否死亡
            if (target is EnemyView enemyView && enemyView != null && enemyView.CurrentHealth <= 0)
            {
                yield return EnemySystem.Instance.RemoveDeadEnemy(enemyView);
            }
            
            // 检查玩家是否死亡
            if (target is PlayerView playerView && playerView != null && playerView.CurrentHealth <= 0 && !gameEnded)
            {
                gameEnded = true;
                yield return new WaitForSeconds(1f);
                
                if (GameData.Instance != null)
                {
                    GameData.Instance.IsVictory = false;
                }
                
                SceneManager.LoadScene("EndScene");
                yield break;
            }
        }
    }
    
    private IEnumerator ShakeCamera(float duration, float magnitude)
    {
        if (mainCamera == null)
        {
            yield break;
        }
        
        Vector3 originalPosition = mainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            mainCamera.transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = originalPosition;
    }
}
