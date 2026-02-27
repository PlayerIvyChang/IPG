using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameManager");
                    instance = go.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    private bool isGameOver = false;
    private int pizzaCount = 0;

    public Transform PlayerTransform => playerTransform;
    public bool IsGameOver => isGameOver;
    public int PizzaCount => pizzaCount;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        UpdateScoreUI();
    }

    public void SetPlayer(Transform player)
    {
        playerTransform = player;
    }

    public void AddPizza(int amount = 1)
    {
        pizzaCount += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Pizzas: " + pizzaCount;
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Pizzas Collected: " + pizzaCount;
        }

        Debug.Log("Game Over! Pizzas Collected: " + pizzaCount);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        pizzaCount = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}