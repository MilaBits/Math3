using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class GameRules : ScriptableObject {
    public int ChangingTileCount = 2;
    public int CurrentAnswer;

    public List<int> Answers;

    [HideInInspector] public UnityEvent NextAnswerEvent;

    public void NextAnswer() {
        int lastAnswer = CurrentAnswer;
        do {
            CurrentAnswer = Answers[Random.Range(0, Answers.Count)];
        } while (lastAnswer == CurrentAnswer);

        NextAnswerEvent.Invoke();
    }

    private void Awake() {
        NextAnswer();
    }

    public List<string> Values;
    public List<string> Operators;
}