using ChatGPTWrapper;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] ChatGPTConversation chatGPT;
    [SerializeField] TMP_InputField iF_PlayerTalk;
    [SerializeField] TextMeshProUGUI tX_AIReply;
    [SerializeField] NPCController npc;

    string npcName = "Coco";
    string playerName = "Player Name";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        string selectedPrompt = PlayerPrefs.GetString("SelectedPrompt", "");
        string selectedName = PlayerPrefs.GetString("SelectedName", "Coco");
        
        if (!string.IsNullOrEmpty(selectedPrompt))
        {
            Debug.Log("Initial Prompt: " + selectedPrompt);
            
            chatGPT.Init();
            chatGPT.ResetChat(selectedPrompt);
            chatGPT.SendToChatGPT("{\"player_said\":\"Hello\"}");
        }
        else
        {
            Debug.LogWarning("No personality selected, using default.");
            chatGPT.Init();
        }
    }

    void Update()
    {
        if (Input.GetButtonUp("Submit"))
        {
            SubmitChatMessage();
        }
    }

    public void ReceivedChatGPTReply(string message)
    {
        try
        {
            if (!message.EndsWith("}"))
            {
                if (!message.Contains("}"))
                {
                    message = message.Substring(0, message.LastIndexOf("}") + 1);
                }
                else
                {
                    message += "}";
                }
            }

            // \ -> \\
            message = message.Replace("\\", "\\\\");
            // \\" -> \"
            message = message.Replace("\\\\\"", "\\\"");

            print(message);
            NPCJSONReceiver npcJSON = JsonUtility.FromJson<NPCJSONReceiver>(message);
            string talkline = npcJSON.reply_to_player;
            tX_AIReply.text = "<color=#ff7082>" + npcName + "</color>" + talkline;

            npc.ShowAnimation(npcJSON.animation_name);
        }

        catch (Exception e)
        {
            Debug.Log(e.Message);
            string talkLine = "Don't say that!";
            tX_AIReply.text = "<color=#ff7082>" + npcName + ": </color>" + talkLine;
        }
    }
    
    public void SubmitChatMessage()
    {
        if (iF_PlayerTalk.text != "")
        {
            Debug.Log("Message sent: " + iF_PlayerTalk.text);
            chatGPT.SendToChatGPT("{\"player_said\":\"" + iF_PlayerTalk.text + "\"}");
            ClearText();
        }
    }

    void ClearText()
    {
        iF_PlayerTalk.text = "";
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MenuState");
    }
}
