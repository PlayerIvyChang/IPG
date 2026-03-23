using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private GameObject classSelectionPanel;
    [SerializeField] private Button archerButton;
    [SerializeField] private Button flameManButton;

    private void Start()
    {
        // 初始状态：只显示 Start 按钮，隐藏职业选择
        classSelectionPanel.SetActive(false);

        // 绑定按钮事件
        startButton.onClick.AddListener(OnStartButtonClicked);
        archerButton.onClick.AddListener(OnArcherSelected);
        flameManButton.onClick.AddListener(OnFlameManSelected);
        
        // 重置游戏进度
        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.ResetProgress();
        }
    }

    private void OnStartButtonClicked()
    {
        // 点击 Start 按钮，显示职业选择
        startButton.gameObject.SetActive(false);
        classSelectionPanel.SetActive(true);
    }

    private void OnArcherSelected()
    {
        // 保存选择并加载游戏场景
        GameData.Instance.SelectedClass = PlayerClass.Archer;
        SceneManager.LoadScene("Game");
    }

    private void OnFlameManSelected()
    {
        // 保存选择并加载游戏场景
        GameData.Instance.SelectedClass = PlayerClass.FlameMan;
        SceneManager.LoadScene("Game");
    }
}