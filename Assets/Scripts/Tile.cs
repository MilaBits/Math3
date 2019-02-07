using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathParserTK;
using TMPro;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private string value;
    public TileType type;
    public LayerMask TileMask;

    [Space] public Color DefaultColor;
    public Color Operator1Color;
    public Color Operator2Color;
    public Color ComboColor;
    public Color ToBeChangedColor;
    public bool ToBeChanged;
    [Space] private SpriteRenderer renderer;

    [HideInInspector] public GameRules GameRules;

    private const string Plus = "+";
    private const string Minus = "-";
    private const string Multiply = "x";
    private const string Divide = "/";


    private Color baseColor;

    public void SetRandomValue() {
        SetValue(GameRules.Values[Random.Range(0, GameRules.Values.Count - 1)]);
    }

    public void SetValue(string text) {
        value = text;
        textMesh.text = value;

        // Dirty way
        switch (value) {
            case Plus:
            case Minus:
                type = TileType.Operator;
                renderer.color = baseColor = Operator1Color;
                break;
            case Multiply:
            case Divide:
                type = TileType.Operator;
                renderer.color = baseColor = Operator2Color;
                break;
            default:
                type = TileType.Number;
                renderer.color = baseColor = DefaultColor;
                break;
        }
    }

    private void Awake() {
        renderer = GetComponentInChildren<SpriteRenderer>();
    }

    public string GetValue() {
        if (value == "x") return "*";
        return value;
    }

    public void Slide(Vector3 target, float time) {
        StartCoroutine(SlideCoroutine(target, time));
    }

    private IEnumerator SlideCoroutine(Vector3 target, float time) {
        float elapsedTime = 0;

        while (elapsedTime < time) {
            transform.position = Vector3.Lerp(transform.position, target, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.position = target;

        CheckAnswer();
    }

    private IEnumerator ColorFade(Color start, Color end, float time) {
        float elapsedTime = 0;

        if (ToBeChanged) end = ToBeChangedColor;    

        while (elapsedTime < time) {
            renderer.color = Color.Lerp(start, end, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private void CheckAnswer() {
        List<Tile> tilesInRow = new List<Tile>();
        List<Tile> tilesInColumn = new List<Tile>();

        // Horizontal
        RaycastHit2D[] HorizontalHits = Physics2D.RaycastAll(
            new Vector2(transform.position.x - 10f, transform.position.y),
            Vector2.right, 100f, TileMask);

        foreach (RaycastHit2D hit in HorizontalHits) {
            if (hit.collider != null) {
                tilesInRow.Add(hit.transform.GetComponent<Tile>());
            }
        }

        // Vertical
        RaycastHit2D[] VerticalHits = Physics2D.RaycastAll(
            new Vector2(transform.position.x, transform.position.y + 10f),
            Vector2.down, 100f, TileMask);

        foreach (RaycastHit2D hit in VerticalHits) {
            if (hit.collider != null) {
                tilesInColumn.Add(hit.transform.GetComponent<Tile>());
            }
        }

        CalculateChains(tilesInRow);
        CalculateChains(tilesInColumn);
    }


    private void CalculateChains(List<Tile> tiles) {
        int chainCount = tiles.Count - 2;
        int bigChainCount = tiles.Count - 4;

        List<Chain> chains = new List<Chain>();

        // Get chains of 3
        for (int i = 0; i < chainCount; i++) {
            chains.Add(new Chain(tiles.GetRange(i, 3)));
        }

        // Get chains of 5
        for (int i = 0; i < bigChainCount; i++) {
            chains.Add(new Chain(tiles.GetRange(i, 5)));
        }

        foreach (Chain chain in chains) {
            if (chain.isValid() && chain.GetValue().Equals(GameRules.CurrentAnswer)) {
                GameRules.NextAnswer();

                foreach (Tile tile in chain.Tiles) {

                    //TODO: Make green last a bit longer before switching.
//                    StartCoroutine(Switch(tile, 1f));
                    StartCoroutine(tile.ColorFade(tile.ComboColor, tile.baseColor, 1));
                    if (tile.type == TileType.Number) {
                        tile.SetRandomValue();
                    }
                }

//                Debug.Log("Correct Answer: " + chain + " = " + chain.GetValue());
//                Debug.Log(chain);
//                Debug.Log(chain + " = " + chain.GetValue());
            }
        }
    }
    
    private IEnumerator Switch(Tile tile, float time) {

        renderer.color = ComboColor;
        yield return new WaitForSeconds(time);
        StartCoroutine(tile.ColorFade(tile.ComboColor, tile.baseColor, 1));
    }
    

    private class Chain {
        public List<Tile> Tiles { get; }
        private string mathString;

        private MathParser mathParser;

        public Chain(List<Tile> tiles) {
            this.Tiles = tiles;

            mathString = string.Empty;
            foreach (Tile tile in tiles) {
                mathString += tile.GetValue();
            }

            mathParser = new MathParser();
        }

        public bool isValid() {
            if (Tiles.Count == 3 &&
                Tiles[0].type == TileType.Number &&
                Tiles[1].type == TileType.Operator &&
                Tiles[2].type == TileType.Number) {
                return true;
            }

            if (Tiles.Count == 5 &&
                Tiles[0].type == TileType.Number &&
                Tiles[1].type == TileType.Operator &&
                Tiles[2].type == TileType.Number &&
                Tiles[3].type == TileType.Operator &&
                Tiles[4].type == TileType.Number) {
                return true;
            }

            return false;
        }

        public override string ToString() {
            return mathString;
        }

        public double GetValue() {
            return mathParser.Parse(mathString);
        }
    }

    public enum TileType {
        Number,
        Operator,
        None
    }

    public void MarkToBeChanged() {
        Debug.Log(name + ": marked to be changed");
        ToBeChanged = true;

        StartCoroutine(ColorFade(baseColor, ToBeChangedColor, 1));
    }
}