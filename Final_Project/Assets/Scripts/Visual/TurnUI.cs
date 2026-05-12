using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnUI : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnText;

    private void Start()
    {
        endTurnButton.onClick.AddListener(() => TurnSystem.Instance.NextTurn());
        UpdateTurnText();
        UpdateEndTurnButtonVisibility();
        TurnSystem.Instance.OnTurnChange += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e)
    {
        UpdateTurnText();
        UpdateEndTurnButtonVisibility();
    }

    private void UpdateTurnText()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            turnText.text = "Player Turn";
        }
        else
        {
            turnText.text = "Enemy Turn";
        }
    }

    private void UpdateEndTurnButtonVisibility()
    {
        endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}