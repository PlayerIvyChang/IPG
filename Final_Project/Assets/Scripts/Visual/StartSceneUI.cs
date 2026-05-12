using UnityEngine;
using UnityEngine.UI;

public class StartSceneUI : MonoBehaviour
{
    [SerializeField] private Button startButton;

    private void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(() =>
            {
                SceneLoader.Instance.LoadGameScene();
            });
        }

    }

    private void OnDestroy()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
        }

    }
}