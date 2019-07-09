using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

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

    [SerializeField]
    private Results results;

    private float timePassed;

    public bool GameOver { get; private set; }

    public bool Running { get; private set; }

    float remainingTime;

    public UnityEvent TimeOver;

    private TimeSpan timeSpan;

    [BoxGroup("Pause")]
    public bool Paused;

    [SerializeField, BoxGroup("Pause")]
    private Image pauseButtonImage;

    [SerializeField, BoxGroup("Pause")]
    private Image pausedBackground;

    [SerializeField, BoxGroup("Pause")]
    private Sprite pauseSprite;

    [SerializeField, BoxGroup("Pause")]
    private Image quitButton;
    [SerializeField, BoxGroup("Pause")]
    private TextMeshProUGUI quitButtonText;

    [SerializeField, BoxGroup("Pause")]
    private Image continueButton;
    [SerializeField, BoxGroup("Pause")]
    private TextMeshProUGUI continueButtonText;

    [SerializeField, BoxGroup("Pause")]
    private Sprite playSprite;


    protected void Start()
    {
        timeSpan = TimeSpan.FromMilliseconds(grid.GameRules.ScoreBonusTime);
        grid.GameRules.NextAnswerEvent.AddListener(AddBonusTime);
        SetTimeText(grid.GameRules.GameTime);

        Theme theme = Resources.LoadAll<Settings>("Settings").First().Theme;
        pausedBackground.color = theme.TileColor;
        
        quitButton.color = theme.ChangeColor;
        quitButtonText.color = theme.TileTextColor;
        
        continueButton.color = theme.SolutionColor;
        continueButtonText.color = theme.TileTextColor;
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
            timePassed += Time.deltaTime * 1000;

            SetTimeText(remainingTime);

            if (remainingTime < 0)
            {
                GameOver = true;
                Running = false;
                TimeOver.Invoke();

                results.gameObject.SetActive(true);
                results.SetResults(grid.SolvedCount, timePassed);
            }
        }
    }

    public void Pause()
    {
        Paused = !Paused;
        if (Paused)
        {
            Time.timeScale = 0;
            pauseButtonImage.sprite = playSprite;
            pausedBackground.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pauseButtonImage.sprite = pauseSprite;
            pausedBackground.gameObject.SetActive(false);
        }
    }

    private void SetTimeText(float time)
    {
        TimeSpan t = TimeSpan.FromMilliseconds(time);
//        timeText.text = new DateTime(t.Ticks).ToString("m:ss");
        timeText.text = $"{t.Minutes}:{t.Seconds}";
    }
}