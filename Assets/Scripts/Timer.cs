using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField]
    private TileGrid grid;

    [SerializeField]
    private TextMeshPro timeText;

    [SerializeField]
    private TextMeshPro bonusTimeText;

    [SerializeField]
    private Animator bonusTimeAnimator;

    public bool GameOver { get; private set; }

    public bool Running { get; private set; }

    float remainingTime;

    public UnityEvent TimeOver;

    private TimeSpan timeSpan;

    protected void Start()
    {
        timeSpan = TimeSpan.FromMilliseconds(grid.GameRules.ScoreBonusTime);
        grid.GameRules.NextAnswerEvent.AddListener(AddBonusTime);
        SetTimeText(grid.GameRules.GameTime);
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

        bonusTimeText.text = $"+{timeSpan.Seconds.ToString()}";
        bonusTimeAnimator.SetTrigger("Pop");
    }

    void Update()
    {
        if (Running)
        {
            remainingTime -= Time.deltaTime * 1000;

            SetTimeText(remainingTime);

            if (remainingTime < 0)
            {
                GameOver = true;
                Running = false;
                TimeOver.Invoke();
            }
        }
    }

    private void SetTimeText(float time)
    {
        TimeSpan t = TimeSpan.FromMilliseconds(time);
        timeText.text = new DateTime(t.Ticks).ToString("m:ss");
    }
}