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
    [BoxGroup("Game Time"), SuffixLabel("ms"), LabelWidth(115)]
    public int GameTime;

	[BoxGroup("Game Time"), SuffixLabel("ms"), LabelText("Score Bonus"), LabelWidth(115)]
    public int ScoreBonusTime;

	[BoxGroup("Tiles")]
    public List<string> TileValues;

	[BoxGroup("Tiles"), LabelText("Switching Tiles"), LabelWidth(115)]
    public int ChangingTileCount = 2;

	[HideInInspector]
    public int CurrentAnswer;

	[BoxGroup("$Combined"), EnumToggleButtons, LabelWidth(115)]
    public AnswerFilter answerFilters;

	[BoxGroup("$Combined"), MinMaxSlider(1, 1000, ShowFields = true), LabelText("Answer Frequency"), LabelWidth(115)]
    public Vector2Int frequencyRange;

    [HideInInspector]
    public UnityEvent NextAnswerEvent;

	[EnumToggleButtons, LabelWidth(115), BoxGroup("Tiles")]
    public Operator ActiveOperators;

    private TileGrid grid;

    private int calculationsDone = 0;

    public static string Plus = "\u002B";
    public static string Minus = "-";
    public static string Multiply = "\u00D7";
    public static string Divide = "\u00F7";

	public string Combined { get { return "Answer: " + this.CurrentAnswer; } }
	
    public void Instantiate(TileGrid grid)
    {
        this.grid = grid;
        Random.InitState((int) System.DateTime.Now.Ticks);
        GenerateAnswer(0);
    }

    private IEnumerable<KeyValuePair<int, int>> GetPossibleAnswers()
    {
        calculationsDone = 0;
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
            { }
        }

        for (int i = 0; i < inputs.Count - 1; i++)
        {
            List<int> filteredInputs = inputs;
            filteredInputs.RemoveAt(i);

            IEnumerable combinations2 = Combinations(inputs, 2);
            foreach (IEnumerable combination in combinations2)
            {
                int[] comb = combination.Cast<int>().ToArray();
                if ((ActiveOperators & Operator.Plus) != 0) AddPotentialAnswer(answers, comb[0] + comb[1]);
                if ((ActiveOperators & Operator.Minus) != 0) AddPotentialAnswer(answers, comb[0] - comb[1]);
                if ((ActiveOperators & Operator.Multiply) != 0) AddPotentialAnswer(answers, comb[0] * comb[1]);
                if ((ActiveOperators & Operator.Divide) != 0) AddPotentialAnswer(answers, comb[0] / comb[1]);
            }

            IEnumerable combinations3 = Combinations(inputs, 3);
            foreach (IEnumerable combination in combinations3)
            {
                int[] comb = combination.Cast<int>().ToArray();

                if ((ActiveOperators & Operator.Plus & Operator.Minus) != 0)
                    AddPotentialAnswer(answers, comb[0] + comb[1] - comb[2]);
                if ((ActiveOperators & Operator.Plus & Operator.Multiply) != 0)
                    AddPotentialAnswer(answers, comb[0] + comb[1] * comb[2]);
                if ((ActiveOperators & Operator.Plus & Operator.Divide) != 0)
                    AddPotentialAnswer(answers, comb[0] + comb[1] / comb[2]);

                if ((ActiveOperators & Operator.Minus & Operator.Plus) != 0)
                    AddPotentialAnswer(answers, comb[0] - comb[1] + comb[2]);
                if ((ActiveOperators & Operator.Minus & Operator.Multiply) != 0)
                    AddPotentialAnswer(answers, comb[0] - comb[1] * comb[2]);
                if ((ActiveOperators & Operator.Minus & Operator.Divide) != 0)
                    AddPotentialAnswer(answers, comb[0] - comb[1] / comb[2]);

                if ((ActiveOperators & Operator.Multiply & Operator.Plus) != 0)
                    AddPotentialAnswer(answers, comb[0] * comb[1] + comb[2]);
                if ((ActiveOperators & Operator.Multiply & Operator.Minus) != 0)
                    AddPotentialAnswer(answers, comb[0] * comb[1] - comb[2]);
                if ((ActiveOperators & Operator.Multiply & Operator.Divide) != 0)
                    AddPotentialAnswer(answers, comb[0] * comb[1] / comb[2]);

                if ((ActiveOperators & Operator.Divide & Operator.Plus) != 0)
                    AddPotentialAnswer(answers, comb[0] / comb[1] + comb[2]);
                if ((ActiveOperators & Operator.Divide & Operator.Minus) != 0)
                    AddPotentialAnswer(answers, comb[0] / comb[1] - comb[2]);
                if ((ActiveOperators & Operator.Divide & Operator.Multiply) != 0)
                    AddPotentialAnswer(answers, comb[0] / comb[1] * comb[2]);
            }
        }

        string debug = string.Empty;
        foreach (KeyValuePair<int, int> keyValuePair in answers.OrderByDescending(i => i.Value))
        {
            debug += $"potential answer: {keyValuePair.Key}, frequency: {keyValuePair.Value}\n";
        }

        Debug.Log("Calculations done:" + calculationsDone + "\n" + debug);

        return answers.Where(a => a.Value >= frequencyRange.x && a.Value <= frequencyRange.y);
    }

    private void AddPotentialAnswer(Dictionary<int, int> answers, int result)
    {
        calculationsDone++;

        if (answerFilters.HasFlag(AnswerFilter.NoPositive) && result > 0) return;
        if (answerFilters.HasFlag(AnswerFilter.NoNegative) && result < 0) return;
        if (answerFilters.HasFlag(AnswerFilter.NoEven) && result % 2 == 0) return;
        if (answerFilters.HasFlag(AnswerFilter.NoOdd) && result % 2 != 0) return;

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

        GenerateAnswer(lastAnswer);

        NextAnswerEvent.Invoke();
    }

    private void GenerateAnswer(int lastAnswer)
    {
        IEnumerable<KeyValuePair<int, int>> answers = GetPossibleAnswers();

        do
        {
            CurrentAnswer = answers.ElementAt(Random.Range(0, answers.Count())).Key;
        } while (lastAnswer == CurrentAnswer);
    }

    [Flags]
    public enum AnswerFilter
    {
        None = 0,
        NoPositive = 1,
        NoNegative = 2,
        NoEven = 4,
        NoOdd = 8
    }

    [Flags]
    public enum Operator
    {
        None = 0,
        Plus = 1,
        Minus = 2,
        Multiply = 4,
        Divide = 8
    }
}