using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[CreateAssetMenu]
public class GameRules : ScriptableObject
{

	[SuffixLabel("ms")]
	public int GameTime;
	[SuffixLabel("ms")]
	public int ScoreBonusTime;

    public int ChangingTileCount = 2;
    public int CurrentAnswer;
    
    [BoxGroup("answertype", false)]    
    [EnumToggleButtons]
    public AnswerType answerType;
    
    [BoxGroup("answertype")]        
    [ShowIf("isSetValues")]
    public List<int> Answers;

    [BoxGroup("answertype")]    
    [ShowIf("isRandom")]
    [MinMaxSlider(1,100)]
    public Vector2Int randomRange;

	[HideInInspector] public UnityEvent NextAnswerEvent;

    public List<string> Values;
    public List<string> Operators;

    
    private bool isRandom()
    {
        return answerType == AnswerType.RandomRange;
    }
    
    private bool isSetValues()
    {
        return answerType == AnswerType.SetValues;
    }
    
    public void NextAnswer() {
	    int lastAnswer = CurrentAnswer;
        
	    Random.seed = (int)Time.time;
        
        switch (answerType)
        {
            case AnswerType.RandomRange:
                do {
                    CurrentAnswer = Random.Range(randomRange.x, randomRange.y);
                } while (lastAnswer == CurrentAnswer);
                break;
            case AnswerType.SetValues:
                do {
                    CurrentAnswer = Answers[Random.Range(0, Answers.Count)];
                } while (lastAnswer == CurrentAnswer);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

	    NextAnswerEvent.Invoke();
    }

    private void Awake() {
        NextAnswer();
    }

    public enum AnswerType
    {
        RandomRange,
        SetValues
    }
}