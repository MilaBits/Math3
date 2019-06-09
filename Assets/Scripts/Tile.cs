using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathParserTK;
using TMPro;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textMesh;

    [SerializeField]
    private string value;

    public TileType type;
    public LayerMask TileMask;

    public bool ToBeChanged;

    [Space]
    private SpriteRenderer renderer;

    [HideInInspector]
    public GameRules GameRules;

    public Theme Theme;

    private Color baseColor;

    public void SetRandomValue()
    {
        SetValue(GameRules.TileValues[Random.Range(0, GameRules.TileValues.Count - 1)]);
    }

    public void SetValue(string text)
    {
        value = text;
        textMesh.text = value;

        // Dirty way
        if (value == GameRules.Plus || value == GameRules.Minus)
        {
            type = TileType.Operator1;
            renderer.color = baseColor = Theme.Operator1Color;
        }
        else if (value == GameRules.Multiply || value == GameRules.Divide)
        {
            type = TileType.Operator2;
            renderer.color = baseColor = Theme.Operator2Color;
        }
        else
        {
            type = TileType.Number;
            renderer.color = baseColor = Theme.TileColor;
        }
    }

    private void Awake()
    {
        renderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        textMesh.color = Theme.TileTextColor;
    }

    public void UpdateTheme(Theme theme)
    {
        Theme = theme;
//        renderer.color = baseColor = theme.TileColor;
        textMesh.color = theme.TileTextColor;

        if (ToBeChanged)
        {
            renderer.color = theme.ChangeColor;
        }
        else
        {
            switch (type)
            {
                case TileType.Number:
                    renderer.color = baseColor = theme.TileColor;
                    break;
                case TileType.Operator1:
                    renderer.color = baseColor = theme.Operator1Color;
                    break;
                case TileType.Operator2:
                    renderer.color = baseColor = theme.Operator2Color;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public string GetValue()
    {
        if (value == "\u00D7") return "*";
        if (value == "\u00F7") return "/";
        return value;
    }

    public void Slide(Vector3 target, float time)
    {
        StartCoroutine(SlideCoroutine(target, time));
    }

    private IEnumerator SlideCoroutine(Vector3 target, float time)
    {
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(transform.position, target, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.position = target;

        CheckAnswer();
    }

    private IEnumerator ColorFade(Color start, Color end, float time)
    {
        float elapsedTime = 0;

        if (ToBeChanged) end = Theme.ChangeColor;

        while (elapsedTime < time)
        {
            renderer.color = Color.Lerp(start, end, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            renderer.color = end;
        }
    }

    private IEnumerator Resize(float start, float end, float time)
    {
        float elapsedTime = 0;

//        if (ToBeChanged) end = ToBeChangedColor;

        while (elapsedTime < time)
        {
            float scale = Mathf.Lerp(start, end, (elapsedTime / time));
            transform.localScale = new Vector3(scale, scale, scale);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private void CheckAnswer()
    {
        List<Tile> tilesInRow = new List<Tile>();
        List<Tile> tilesInColumn = new List<Tile>();

        // Horizontal
        RaycastHit2D[] HorizontalHits = Physics2D.RaycastAll(
            new Vector2(transform.position.x - 10f, transform.position.y),
            Vector2.right, 100f, TileMask);

        foreach (RaycastHit2D hit in HorizontalHits)
        {
            if (hit.collider != null)
            {
                tilesInRow.Add(hit.transform.GetComponent<Tile>());
            }
        }

        // Vertical
        RaycastHit2D[] VerticalHits = Physics2D.RaycastAll(
            new Vector2(transform.position.x, transform.position.y + 10f),
            Vector2.down, 100f, TileMask);

        foreach (RaycastHit2D hit in VerticalHits)
        {
            if (hit.collider != null)
            {
                tilesInColumn.Add(hit.transform.GetComponent<Tile>());
            }
        }

        CalculateChains(tilesInRow);
        CalculateChains(tilesInColumn);
    }


    private void CalculateChains(List<Tile> tiles)
    {
        int chainCount = tiles.Count - 2;
        int bigChainCount = tiles.Count - 4;

        List<Chain> chains = new List<Chain>();

        // Get chains of 3
        for (int i = 0; i < chainCount; i++)
        {
            chains.Add(new Chain(tiles.GetRange(i, 3)));
        }

        // Get chains of 5
        for (int i = 0; i < bigChainCount; i++)
        {
            chains.Add(new Chain(tiles.GetRange(i, 5)));
        }

        foreach (Chain chain in chains)
        {
            if (chain.isValid() && chain.GetValue().Equals(GameRules.CurrentAnswer))
            {

	            GameRules.NextAnswer();
                foreach (Tile tile in chain.Tiles)
                {
                    //TODO: Make green last a bit longer before switching.

                    StartCoroutine(Switch(tile, .5f));
//                    StartCoroutine(tile.ColorFade(tile.ComboColor, tile.baseColor, 1));
                }

//                Debug.Log("Correct Answer: " + chain + " = " + chain.GetValue());
//                Debug.Log(chain);
//                Debug.Log(chain + " = " + chain.GetValue());
            }
        }
    }

    private IEnumerator Switch(Tile tile, float time)
    {
        bool isNumber = tile.type == TileType.Number;

        StartCoroutine(tile.ColorFade(Theme.SolutionColor, tile.baseColor, 1));

        StartCoroutine(tile.Resize(1, 0, time));

        yield return new WaitForSeconds(time);
        if (isNumber) tile.SetRandomValue();

	    StartCoroutine(tile.Resize(0, 1, time));
        CheckAnswer();
    }


    private class Chain
    {
        public List<Tile> Tiles;
        private string mathString;

        private MathParser mathParser;

        public Chain(List<Tile> tiles)
        {
            this.Tiles = tiles;

            mathString = string.Empty;
            foreach (Tile tile in tiles)
            {
                mathString += tile.GetValue();
            }

            mathParser = new MathParser();
        }

        public bool isValid()
        {
            if (Tiles.Count == 3 &&
                Tiles[0].type == TileType.Number &&
                (Tiles[1].type == TileType.Operator1 || Tiles[1].type == TileType.Operator2) &&
                Tiles[2].type == TileType.Number)
            {
                return true;
            }

            if (Tiles.Count == 5 &&
                Tiles[0].type == TileType.Number &&
                (Tiles[1].type == TileType.Operator1 || Tiles[1].type == TileType.Operator2) &&
                Tiles[2].type == TileType.Number &&
                (Tiles[3].type == TileType.Operator1 || Tiles[3].type == TileType.Operator2) &&
                Tiles[4].type == TileType.Number)
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return mathString;
        }

        public double GetValue()
        {
            return mathParser.Parse(mathString);
        }
    }

    [Flags]
    public enum TileType
    {
        Number,
        Operator1,
        Operator2,
        None
    }

    public void MarkToBeChanged()
    {
        ToBeChanged = true;

        StartCoroutine(ColorFade(baseColor, Theme.ChangeColor, 1));
    }
}