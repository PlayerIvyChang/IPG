using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionNameText;
    [SerializeField] private Button actionButton;
    [SerializeField] private GameObject selected;
    [SerializeField] private Image selectedImage;

    private BaseAction baseAction;
    
    public void SetBaseAction(BaseAction baseAction)
    {
        this.baseAction = baseAction;
        actionNameText.text = baseAction.GetActionName().ToUpper();
        actionButton.onClick.AddListener(() =>
        {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
        });
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();
        bool isSelected = selectedAction == baseAction;
        
        if (selected != null)
        {
            selected.SetActive(isSelected);
        }
        
        if (selectedImage != null && selectedImage.gameObject != selected)
        {
            selectedImage.enabled = isSelected;
        }
    }
}
