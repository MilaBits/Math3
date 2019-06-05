using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathParserTK;
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
    [MinMaxSlider(1, 100)]
    public Vector2Int randomRange;
    
    [MinMaxSlider(1, 100)]
    public Vector2Int frequencyRange;

    [HideInInspector]
    public UnityEvent NextAnswerEvent;

    public List<string> Values;
    public List<string> Operators;

    private TileGrid grid;

    private MathParser parser;

    private bool isRandom()
    {
        return answerType == AnswerType.RandomRange;
    }

    private bool isSetValues()
    {
        return answerType == AnswerType.SetValues;
    }

    public void Instantiate(TileGrid grid)
    {
        this.grid = grid;
        parser = new MathParser();
        Random.InitState((int) System.DateTime.Now.Ticks);
        CurrentAnswer = Random.Range(randomRange.x, randomRange.y);
    }

    private int[] GetPossibleAnswers()
    {
        List<int> inputs = new List<int>();
        Dictionary<int, int> answers = new Dictionary<int, int>();
        foreach (Tile gridTile in grid.tiles)
        {
            int value;
            try
            {
                value = Int32.Parse(gridTile.GetValue());
                inputs.Add(value);
            }
            catch (Exception e)
            {
            }
        }

        for (int i = 0; i < inputs.Count - 1; i++)
        {
            List<int> filteredInputs = inputs;
            filteredInputs.RemoveAt(i);

//            IEnumerable combinations2 = Combinations(inputs, 2);
//            foreach (IEnumerable combination in combinations2)
//            {
//
//                int[] comb = combination.Cast<int>().ToArray();
//            }
            
            IEnumerable combinations3 = Combinations(inputs, 3);
            foreach (IEnumerable combination in combinations3)
            {

                int[] comb = combination.Cast<int>().ToArray();
                
                AddPotentialAnswer(answers, (int) parser.Parse($"{comb[0]}+{comb[1]}"));
                AddPotentialAnswer(answers, (int) parser.Parse($"{comb[0]}-{comb[1]}"));
                AddPotentialAnswer(answers, (int) parser.Parse($"{comb[0]}*{comb[1]}"));
                AddPotentialAnswer(answers, (int) parser.Parse($"{comb[0]}/{comb[1]}"));
                
                AddPotentialAnswer(answers, (int) parser.Parse($"{comb[0]}+{comb[1]}+{comb[2]}"));
                AddPotentialAnswer(answers, (int) parser.Parse($"{comb[0]}+{comb[1]}-{comb[2]}"));
                AddPotentialAnswer(answers, (int) parser.Parse($"{comb[0]}+{comb[1]}*{comb[2]}"));
                AddPotentialAnswer(answers, (int) parser.Parse($"{comb[0]}+{comb[1]}/{comb[2]}"));
            }
        }

        string debug = string.Empty;
        foreach (KeyValuePair<int, int> keyValuePair in answers.OrderByDescending(i => i.Value))
        {
            debug += $"potential answer: {keyValuePair.Key}, frequency: {keyValuePair.Value}\n";
        }

        Debug.Log(debug);

        return inputs.ToArray();
    }

    private static void AddPotentialAnswer(Dictionary<int, int> answers, int result)
    {
        if (answers.ContainsKey(result))
        {
            answers[result]++;
            return;
        }

        answers.Add(result, 1);
    }

    private static bool NextCombination(IList<int> num, int n, int k)
    {
        bool finished;

        var changed = finished = false;

        if (k <= 0) return false;

        for (var i = k - 1; !finished && !changed; i--)
        {
            if (num[i] < n - 1 - (k - 1) + i)
            {
                num[i]++;

                if (i < k - 1)
                    for (var j = i + 1; j < k; j++)
                        num[j] = num[j - 1] + 1;
                changed = true;
            }

            finished = i == 0;
        }

        return changed;
    }

    private static IEnumerable Combinations<T>(IEnumerable<T> elements, int k)
    {
        var elem = elements.ToArray();
        var size = elem.Length;

        if (k > size) yield break;

        var numbers = new int[k];

        for (var i = 0; i < k; i++)
            numbers[i] = i;

        do
        {
            yield return numbers.Select(n => elem[n]);
        } while (NextCombination(numbers, size, k));
    }

    public void NextAnswer()
    {
        int lastAnswer = CurrentAnswer;

        GetPossibleAnswers();

        switch (answerType)
        {
            case AnswerType.RandomRange:
                do
                {
                    CurrentAnswer = Random.Range(randomRange.x, randomRange.y);
                } while (lastAnswer == CurrentAnswer);

                break;
            case AnswerType.SetValues:
                do
                {
                    CurrentAnswer = Answers[Random.Range(0, Answers.Count)];
                } while (lastAnswer == CurrentAnswer);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        NextAnswerEvent.Invoke();
    }

    private void Awake()
    {
        NextAnswer();
    }

    public enum AnswerType
    {
        RandomRange,
        SetValues
    }
}