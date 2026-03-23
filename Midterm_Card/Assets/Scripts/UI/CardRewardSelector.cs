using UnityEngine;

public class RewardCardSelector : MonoBehaviour
{
    private int cardIndex;
    private CardRewardPanel rewardPanel;

    public void Setup(int index, CardRewardPanel panel)
    {
        cardIndex = index;
        rewardPanel = panel;
    }

    private void OnMouseDown()
    {
        if (rewardPanel != null)
        {
            rewardPanel.OnCardClicked(cardIndex);
        }
    }
}