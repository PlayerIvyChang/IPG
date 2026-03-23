using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoardView : MonoBehaviour
{
    [SerializeField] private List<Transform> slots;
    public List<EnemyView> EnemyViews { get; private set; } = new();

    public void AddEnemy(EnemyData enemyData)
    {
        Transform slot = slots[EnemyViews.Count];
        EnemyView enemyView = EnemyViewCreator.Instance.CreateEnemyView(enemyData, slot.position, slot.rotation);
        enemyView.transform.parent = slot;
        EnemyViews.Add(enemyView);
    }

    public IEnumerator RemoveEnemy(EnemyView enemyView)
    {
        EnemyViews.Remove(enemyView);

        // 缩小至消失的动画
        Vector3 originalScale = enemyView.transform.localScale;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            enemyView.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        Destroy(enemyView.gameObject);
    }
}
