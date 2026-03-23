using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    public PlayerClass SelectedClass { get; set; } = PlayerClass.Archer;
    public bool IsVictory { get; set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // È·±£ GameProgress ´æÔÚ
            if (GameProgress.Instance == null)
            {
                GameObject progressObj = new GameObject("GameProgress");
                progressObj.AddComponent<GameProgress>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}