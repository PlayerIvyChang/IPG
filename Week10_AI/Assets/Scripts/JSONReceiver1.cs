using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class JSONReceiver1
{
    public Comment[] comments;
}

[System.Serializable]
public class Comment
{
    public string title;
    public int year;
    public string[] cast;
    public string[] genres;
    public string href;
    public string extract;
}