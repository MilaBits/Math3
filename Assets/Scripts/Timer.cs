using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private TileGrid grid;

    [SerializeField]
    private TextMeshProUGUI timeText;

    public bool GameOver { get; private set; }

    public bool Running { get; private set; }

    float remainingTime;

    protected void Start()
    {
        grid.GameRules.NextAnswerEvent.AddListener(AddBonusTime);
        SetTimeText(grid.GameRules.GameTime);
        timeText.text += "0";
    }

    public void StartTimer()
    {
        remainingTime = grid.GameRules.GameTime;
        Running = true;
    }

    public void StopTimer()
    {
        Running = false;
    }

    public void AddBonusTime()
    {
        remainingTime += grid.GameRules.ScoreBonusTime;
        Debug.Log("Time Added!");
    }

    void Update()
    {
        if (Running)
        {
            remainingTime -= Time.deltaTime * 1000;

            SetTimeText(remainingTime);

            if (remainingTime < 0)
            {
                Debug.Log("Game over");
                GameOver = true;
            }
        }
    }

    private void SetTimeText(float time)
    {
        int minutes = (int) ((time / (1000 * 60)) % 60);
        int seconds = (int) (time / 1000) % 60;
        timeText.text = string.Format("{0}:{1}", minutes, seconds);
    }
}