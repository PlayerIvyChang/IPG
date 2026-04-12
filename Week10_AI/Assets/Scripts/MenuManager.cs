using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    PersonalityDatabase personDB;
    [SerializeField]
    GameSettings settings;

    [SerializeField]
    ToggleGroup toggleGroup;
    [SerializeField]
    Toggle[] toggles;

    void Start()
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            toggles[i].GetComponentInChildren<Text>().text = personDB.personalities[i].name;
            
            // 为每个 Toggle 添加监听器
            int index = i; // 捕获当前索引
            toggles[i].onValueChanged.AddListener((isOn) => 
            {
                if (isOn) // 只在 Toggle 被选中时触发
                {
                    Debug.Log("Toggle " + index + " selected");
                    settings.selectedIndex = index;
                    UpdateSelectedPersonality(index);
                }
            });
        }

        //Load data
        settings.gameTimer = PlayerPrefs.GetFloat("GameTimer", 0);
        settings.selectedIndex = PlayerPrefs.GetInt("SelectedIndex", 0);
        
        if (toggles.Length > settings.selectedIndex)
        {
            toggles[settings.selectedIndex].isOn = true;
        }
        
        UpdateSelectedPersonality(settings.selectedIndex);
    }
    
    public void StartGame()
    {
        PlayerPrefs.SetFloat("GameTimer", settings.gameTimer);
        PlayerPrefs.SetInt("SelectedIndex", settings.selectedIndex);
        PlayerPrefs.Save();
        
        Debug.Log("Starting game with personality index: " + settings.selectedIndex);
        SceneManager.LoadScene("GameState");
    }

    public void OnValueChanges()
    {
        var currentToggle = toggleGroup.ActiveToggles().FirstOrDefault();

        int currentSelectedIndex = 0;
        for (int i = 0; i < toggles.Length; i++)
        {
            if (currentToggle == toggles[i])
            {
                currentSelectedIndex = i;
                break;
            }
        }

        settings.selectedIndex = currentSelectedIndex;
        UpdateSelectedPersonality(currentSelectedIndex);
    }

    private void UpdateSelectedPersonality(int index)
    {
        if (personDB != null && personDB.personalities.Length > index)
        {
            string selectedPrompt = personDB.personalities[index].initialPrompt;
            string selectedName = personDB.personalities[index].name;
            
            PlayerPrefs.SetString("SelectedPrompt", selectedPrompt);
            PlayerPrefs.SetString("SelectedName", selectedName);
            PlayerPrefs.SetInt("SelectedIndex", index);
            PlayerPrefs.Save(); // 立即保存
                    }
        else
        {
            Debug.LogError("PersonDB is null or index out of range!");
        }
    }
}