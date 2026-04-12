using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class JSONLoader : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(GetJsonFromUrl("https://raw.githubusercontent.com/prust/wikipedia-movie-data/master/movies.json", ReceivedJSON1));
    }

    IEnumerator GetJsonFromUrl(string url, System.Action<string> callback)
    {
        string jsonText;

        UnityWebRequest www = UnityWebRequest.Get(url);
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            jsonText = www.error;
        }
        else
        {
            jsonText = www.downloadHandler.text;
        }

        callback(jsonText);
        www.Dispose();
    }
    void ReceivedJSON1(string jsonText)
    {
        JSONReceiver1 receiver = JsonUtility.FromJson<JSONReceiver1>("{\"comments\":" + jsonText + "}");
        Comment[] comments = receiver.comments;
                
    }
}
