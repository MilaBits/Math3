using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Results : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI solvedText;

    [SerializeField]
    private TextMeshProUGUI timeText;

    public void SetResults(int solved, float time)
    {
        TimeSpan span = TimeSpan.FromMilliseconds(time);

        string sm = solved != 1 ? "es" : "";
        solvedText.text = $"You made {solved} match{sm}!";
        
        string ms = span.Minutes != 1 ? "s" : "";
        string ss = span.Seconds != 1 ? "s" : "";
        timeText.text = $"You lasted {span.Minutes} minute{ms} and {span.Seconds} second{ss}";
    }
}